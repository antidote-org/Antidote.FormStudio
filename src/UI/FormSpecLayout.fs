module Antidote.FormStudio.UI.FormSpecLayout

open Feliz
open Fable.Core.JsInterop

open Antidote.FormStudio
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper
open Antidote.FormStudio.UI.DynamicFormSpecDetails
open Antidote.FormStudio.UI.Designer.FormStepTools
open Antidote.FormStudio.UI.Designer.FieldEditorContainer
open Antidote.FormStudio.UI.StepBreak
open Antidote.FormStudio.UI.Components.DropFieldsContainer

let private classes: CssModules.UI.FormSpecLayout =
    import "default" "./FormSpecLayout.module.scss"

type FormSpecLayoutProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        OnChange: FormSpec<'UserField> -> unit
        SetIsPreview: bool -> unit
        SelectedStepNumber: int
        SetStepNumber: int -> unit
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        IsFieldDragging: bool
        SetFieldDragging: bool -> unit
        RegisteredFields: RegisteredFields<'UserField>
    |}

[<ReactComponent>]
let FormSpecLayout (props: FormSpecLayoutProps<'UserField>) =
    React.fragment [
        DynamicFormSpecDetails
            {|
                FormSpec = props.FormSpec
                OnChange = props.OnChange
                ActiveField = props.ActiveField
            |}

        props.FormSpec.Steps
        |> List.sortBy (fun s -> s.StepOrder)
        |> List.map (fun step ->
            Html.div [
                prop.className classes.step
                prop.children [
                    if props.FormSpec.Steps.Length > 1 then
                        FormStepTools
                            {|
                                FormSpec = props.FormSpec
                                FormStep = step
                                OnChange = props.OnChange
                            |}

                    match step.Fields with
                    | [] ->
                        // Empty Fields. Show the dashed box to drop fields.
                        DropFieldsContainer
                            {|
                                Children =
                                    [
                                        Html.div [
                                            prop.style [
                                                style.display.flex
                                                style.flexDirection.column
                                                style.justifyContent.center
                                                style.alignItems.center
                                                style.height (length.perc 100)
                                            ]
                                            prop.children [
                                                Html.i [
                                                    prop.className "fas fa-arrows-alt"
                                                ]
                                                Html.span
                                                    $"Drag fields for step {step.StepOrder} here"
                                            ]
                                        ]
                                    ]
                                OnDrop =
                                    fun dragSourceOpt ->
                                        match dragSourceOpt with
                                        | Some(DragSource.Designer_FormFieldType_Key key) ->
                                            //add designer field type to selected step via DROP INTO EMPTY STEP
                                            props.OnChange(
                                                addDesignerFieldTypeToStep
                                                    key
                                                    step.StepOrder
                                                    props.FormSpec
                                                    props.RegisteredFields
                                            )
                                        | _ -> ()
                            |}
                    | _ ->
                        Html.div [
                            prop.children [
                                step.Fields
                                |> List.sortBy (fun f -> f.FieldOrder)
                                |> List.map (fun formField ->
                                    FieldEditorContainer
                                        {|
                                            FormSpec = props.FormSpec
                                            FormStep = step
                                            FormField = formField
                                            ActiveField = props.ActiveField
                                            OnChange = props.OnChange
                                            SelectedStepNumber = props.SelectedStepNumber
                                            SetStepNumber = props.SetStepNumber
                                            SetActiveField = props.SetActiveField
                                            IsFieldDragging = props.IsFieldDragging
                                            SetFieldDragging = props.SetFieldDragging
                                            RegisteredFields = props.RegisteredFields
                                        |}
                                )
                                |> React.fragment
                            ]
                        ]

                    StepBreak
                        {|
                            FormSpec = props.FormSpec
                            ActiveField = props.ActiveField
                            AddStep =
                                (fun () ->
                                    let newStep =
                                        {
                                            StepOrder = (props.FormSpec.Steps |> List.length) + 1
                                            StepLabel = $"Untitled Step"
                                            Fields = []
                                        }

                                    let newFormSpec =
                                        { props.FormSpec with
                                            Steps =
                                                props.FormSpec.Steps
                                                @ [
                                                    newStep
                                                ]
                                        }

                                    props.OnChange newFormSpec
                                    props.SetStepNumber(newStep.StepOrder)

                                )
                        |}
                ]
            ]
        )
        |> React.fragment
    ]
