module Antidote.FormStudio.UI.Designer.FieldEditorContainer

open Feliz
open Antidote.FormStudio
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper
open Browser
open Fable.Core.JsInterop
open Antidote.FormStudio.UI.Components.LabelEditor
open Antidote.FormStudio.UI.FieldToolbar
open Antidote.FormStudio.UI.Designer.MockField

let private classes: CssModules.UI.Designer_FieldEditorContainer =
    import "default" "./Designer.FieldEditorContainer.module.scss"

type FieldEditorContainerProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        FormField: FormField<'UserField>
        ActiveField: ActiveField
        OnChange: FormSpec<'UserField> -> unit
        SelectedStepNumber: int
        SetStepNumber: int -> unit
        SetActiveField: ActiveField -> unit
        IsFieldDragging: bool
        SetFieldDragging: bool -> unit
        RegisteredFields: RegisteredFields<'UserField>
    |}

[<ReactComponent>]
let FieldEditorContainer (props: FieldEditorContainerProps<'UserField>) =
    let isBeingDraggedOver, setIsBeingDraggedOver = React.useState false

    Html.div [
        prop.name "field-container"
        prop.draggable true

        prop.onDragStart (fun e ->
            let data =
                (DragSource.Designer_FormField_FieldKey props.FormField.FieldKey).ToPlainText

            e.dataTransfer.setData ("text/plain", data) |> ignore

            props.SetFieldDragging true
        )

        prop.onDragEnter (fun (e: Types.DragEvent) ->
            e.preventDefault ()
            props.SetFieldDragging true
        )

        prop.onDragLeave (fun (e: Types.DragEvent) ->
            e.preventDefault ()
            setIsBeingDraggedOver false
        // props.SetFieldDragging false
        )

        prop.onDragEnd (fun (e: Types.DragEvent) ->
            e.preventDefault ()
            props.SetFieldDragging false
        )

        prop.onDragOver (fun (e: Types.DragEvent) ->
            e.preventDefault ()
            setIsBeingDraggedOver true
        )

        prop.onDrop (fun (e: Types.DragEvent) ->
            e.preventDefault ()
            setIsBeingDraggedOver false
            props.SetFieldDragging false

            let dragSource = e.dataTransfer.getData ("text/plain") |> tryGetDragSourceFromData

            match dragSource with
            | Some(DragSource.Designer_FormFieldType_Key key) ->
                let outFormSpec =
                    insertDesignerFieldTypeToStepAt
                        key
                        props.FormStep.StepOrder
                        props.FormField.FieldOrder
                        props.FormSpec
                        props.RegisteredFields

                props.OnChange outFormSpec

            | Some(DragSource.Designer_FormField_FieldKey fieldKey) ->
                let fields = props.FormStep.Fields
                let fieldBeingInserted = fields |> List.find (fun f -> f.FieldKey = fieldKey)
                let thisField = props.FormField

                let fields =
                    props.FormSpec.Steps
                    |> List.find (fun s -> s.StepOrder = props.FormStep.StepOrder)
                    |> fun step -> step.Fields

                let outFields =
                    fields
                    |> List.map (fun f ->
                        if f.FieldKey = fieldBeingInserted.FieldKey then
                            thisField
                        elif f.FieldKey = thisField.FieldKey then
                            fieldBeingInserted
                        else
                            f
                    )
                    |> List.mapi (fun i f ->
                        { f with
                            FieldOrder = i + 1
                        }
                    )

                let outFormSpec =
                    { props.FormSpec with
                        Steps =
                            props.FormSpec.Steps
                            |> List.map (fun s ->
                                if s.StepOrder = props.FormStep.StepOrder then
                                    { s with
                                        Fields = outFields
                                    }
                                else
                                    s
                            )
                    }

                props.OnChange outFormSpec

                // props.OnChange (props.FormSpec |> moveFieldByKeyToPositionInFormStepSpec fieldKey props.FormField.FieldOrder props.FormStep.StepOrder )
                ()
            | _ -> ()
        )

        prop.children [
            //TODO: The before and after field container divs will serve as the drop targets for adding fields before and after the current field.
            Html.div [
                prop.className classes.``field-before-and-after``
            ]
            Html.div [
                prop.style [
                    style.width (length.perc 100)
                ]
                prop.classes [
                    classes.field
                    if props.IsFieldDragging then
                        classes.``field-dragging-hint``

                    if isBeingDraggedOver then
                        classes.``field-is-being-dragged-over``

                    if
                        props.FormField.FieldOrder = props.ActiveField.FormFieldNumber
                        && props.FormStep.StepOrder = props.SelectedStepNumber
                    then
                        classes.``field-selected``
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormField.FieldOrder < props.ActiveField.FormFieldNumber
                        && props.FormStep.StepOrder = props.SelectedStepNumber
                    then
                        classes.``field-drag``
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormField.FieldOrder > props.ActiveField.FormFieldNumber
                    then //&& step.StepOrder < props.SelectedStepNumber then
                        GlobalCSS.classes.disabled
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormStep.StepOrder <> props.SelectedStepNumber
                    then
                        GlobalCSS.classes.disabled
                ]

                prop.onClick (fun _ ->
                    props.SetActiveField
                        {
                            FormStepNumber = props.FormStep.StepOrder
                            FormFieldNumber = props.FormField.FieldOrder
                            State = Idle
                        }

                    props.SetStepNumber props.FormStep.StepOrder
                )
                prop.children [

                    Html.div [
                        prop.style [
                            style.display.flex
                            style.justifyContent.spaceBetween
                        ]
                        prop.children [
                            LabelEditor
                                {|
                                    FormSpec = props.FormSpec
                                    FormStep = props.FormStep
                                    FormField = props.FormField
                                    OnChange = props.OnChange
                                    ActiveField = props.ActiveField
                                |}

                            FieldToolbar
                                {|
                                    FormSpec = props.FormSpec
                                    FormStepNumber = props.ActiveField.FormStepNumber
                                    FormField = props.FormField
                                    OnChange = props.OnChange
                                    ActiveField = props.ActiveField
                                    SetActiveField = props.SetActiveField
                                |}
                        ]
                    ]
                    MockField
                        {|
                            FormSpec = props.FormSpec
                            FormStep = props.FormStep
                            FormField = props.FormField
                            ActiveField = props.ActiveField
                            SetActiveField = props.SetActiveField
                            OnChange = props.OnChange
                            RegisteredFields = props.RegisteredFields
                        |}
                ]
            ]
            Html.div [
                prop.className classes.``field-before-and-after``
            ]
        ]
    ]
