module Antidote.FormStudio.DynamicFormDesigner

open Fable.Form.Antidote

open System
open Browser
open Feliz
open Feliz.Bulma
open Antidote.FormStudio.i18n.Util
open type Feliz.Toastify.Exports
open Antidote.React.Components.FormWizard
open Feliz.ReactRouterDom
open Fable.Core.JsInterop
open Antidote.Core.FormProcessor.Spec.v2_0_1

open Antidote.FormStudio.Compose.Types
open Antidote.Core.FormProcessor.Spec
open Antidote.FormDesigner.Types
open Antidote.FormDesigner.PropertyEditor
open Antidote.FormDesigner.FormStepTools
open Antidote.FormDesigner.Helper
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.React.Components.FormWizard.SinglePageReview
open Antidote.React.FormDesigner.Designer.SpecScore
open Antidote.FormDesigner

let private classes: CssModules.DynamicFormDesigner =
    import "default" "./DynamicFormDesigner.module.scss"

type StepBreakProps =
    {|
        FormSpec: FormSpec
        OnChange: FormSpec -> unit
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let StepBreak (props: StepBreakProps) =
    Html.div [
        prop.className [
            classes.stepBreak
            if props.ActiveField.State = AddingDependantKeys then
                classes.disabled
            else
                ""
        ]
        prop.children [
            Html.button [
                prop.className classes.buttonMinimal
                prop.text "+ ADD STEP"
                prop.onClick (fun _ ->
                    //add new step
                    let newStep =
                        {
                            StepOrder = props.FormSpec.Steps |> List.length
                            StepLabel = "New Step"
                            // Icon = "fas fa-font"
                            Fields =
                                [
                                    {
                                        FieldOrder = 0
                                        FieldKey = Guid.NewGuid().ToString()
                                        FieldType =
                                            FieldType.Text
                                                {
                                                    Value = None
                                                }
                                        Label = "New Field"
                                        IsOptional = false
                                        IsDeprecated = false
                                        // Flags = []
                                        DependsOn = None
                                    }
                                ]
                        }

                    let outFormSpec =
                        { props.FormSpec with
                            Steps =
                                props.FormSpec.Steps
                                @ [
                                    newStep
                                ]
                        }

                    props.OnChange outFormSpec
                )
            ]
        ]
    ]

[<RequireQualifiedAccess>]
type private BulmaFleldLayout =
    | Horizontal
    | Vertical

type private BulmaHorizontalFieldProps =
    {|
        Label: string option
        Placeholder: string
        Value: string
        Layout: BulmaFleldLayout
        OnChange: string -> unit
        LeftIcon: string option
        RightIcon: string option
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let private BulmaHorizontalField (props: BulmaHorizontalFieldProps) =
    Bulma.field.div [
        prop.style [
            style.display.flex
            style.alignItems.center
        ]
        if props.Layout = BulmaFleldLayout.Horizontal then
            field.isHorizontal
        prop.className [
            if props.ActiveField.State = AddingDependantKeys then
                classes.disabled
        ]
        prop.children [
            match props.Label with
            | Some label ->
                Bulma.fieldLabel [
                    fieldLabel.isNormal
                    prop.children [
                        Bulma.label label
                    ]
                ]
            | None -> ()

            Bulma.fieldBody [
                Bulma.field.div [
                    prop.children [
                        Bulma.control.p [
                            if props.LeftIcon |> Option.isSome then
                                control.hasIconsLeft
                            if props.RightIcon |> Option.isSome then
                                control.hasIconsRight
                            prop.children [
                                Bulma.input.text [
                                    input.isSmall
                                    prop.placeholder props.Placeholder
                                    prop.value props.Value
                                    prop.onChange props.OnChange
                                ]

                                match props.LeftIcon with
                                | Some iconClass ->
                                    Bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        prop.children [
                                            Html.i [
                                                prop.classes [
                                                    iconClass
                                                ]
                                            ]
                                        ]
                                    ]
                                | None -> ()

                                match props.RightIcon with
                                | Some iconClass ->
                                    Bulma.icon [
                                        icon.isSmall
                                        icon.isRight
                                        prop.children [
                                            Html.i [
                                                prop.classes [
                                                    iconClass
                                                ]
                                            ]
                                        ]
                                    ]
                                | None -> ()
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

type LabelEditProps =
    {|
        FormSpec: FormSpec
        FormStep: FormStep
        FormField: FormField
        OnChange: FormSpec -> unit
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let LabelEdit (props: LabelEditProps) =
    if props.ActiveField.State = AddingDependantKeys then
        Html.p [
            prop.text props.FormField.Label
        ]
    else
        Html.input [
            if not (System.String.IsNullOrWhiteSpace(props.FormField.Label)) then
                prop.className classes.inputAsLabel
            else
                prop.className "input is-danger"

            prop.onChange (fun (e: string) ->
                let newFormField =
                    { props.FormField with
                        Label = e
                    }

                let outFormSpec =
                    props.FormSpec |> updateFormFieldInFormSpecStep newFormField props.FormStep

                props.OnChange outFormSpec
            )
            prop.value props.FormField.Label
        ]

type FieldToolbarProps =
    {|
        FormSpec: FormSpec
        FormStepNumber: int
        FormField: FormField
        OnChange: FormSpec -> unit
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
    |}

[<ReactComponent>]
let FieldToolbar (props: FieldToolbarProps) =
    let step = props.FormSpec |> tryFindFormStepByStepNumber props.FormStepNumber

    if
        props.ActiveField.State = AddingDependantKeys
        && props.FormField.FieldOrder < props.ActiveField.FormFieldNumber
    then
        Bulma.tag [
            Bulma.tag.isRounded
            prop.style [
                style.marginTop 5
            ]
            prop.classes [
                "has-background-success"
                "has-text-white"
                "has-text-weight-bold"
            ]
            prop.children [
                Html.span [
                    prop.className "icon"
                    prop.children [
                        Html.i [
                            prop.className "fas fa-arrows-alt"
                        ]
                    ]
                ]
                Html.span [
                    prop.text "Drag Me!"
                ]
            ]

        ]
    else if props.FormField.FieldOrder = props.ActiveField.FormFieldNumber then
        Bulma.buttons [
            prop.style [
                style.paddingBottom 10
            ]
            prop.children [
                Bulma.button.button [
                    Bulma.color.isPrimary
                    Bulma.button.isText
                    prop.style [
                        style.textDecoration.none
                        style.marginTop 10
                    ]
                    prop.disabled (props.FormField.FieldOrder <= 1)

                    prop.onClick (fun e ->
                        e.stopPropagation ()

                        let step =
                            props.FormSpec |> tryFindFormStepByStepNumber props.FormStepNumber

                        let outFormSpec =
                            props.FormSpec
                            |> moveFormFieldUpInFormSpec props.FormStepNumber props.FormField

                        props.OnChange outFormSpec

                        props.SetActiveField
                            { props.ActiveField with
                                FormFieldNumber = props.FormField.FieldOrder - 1
                                State = Idle
                            }
                    )
                    prop.classes [
                        "button"
                        "is-small"
                        "is-primary"
                    ]
                    // prop.type' "button"
                    prop.children [
                        Html.span [
                            prop.className "icon"
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-caret-square-up"
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.button.button [
                    Bulma.color.isPrimary
                    Bulma.button.isText
                    prop.style [
                        style.textDecoration.none
                        style.marginTop 10
                    ]
                    prop.disabled (
                        let currentStep =
                            match step with
                            | Some step -> (step.Fields |> List.length) - 1
                            | None -> 0

                        props.FormField.FieldOrder = currentStep
                    )
                    prop.onClick (fun e ->
                        e.stopPropagation ()

                        let outFormSpec =
                            props.FormSpec
                            |> moveFormFieldDownInFormSpec props.FormStepNumber props.FormField

                        props.OnChange outFormSpec

                        props.SetActiveField
                            { props.ActiveField with
                                FormFieldNumber = props.FormField.FieldOrder + 1
                                State = Idle
                            }
                    )
                    prop.classes [
                        "button"
                        "is-small"
                        "is-primary"
                    ]
                    // prop.type' "button"
                    prop.children [
                        Html.span [
                            prop.className "icon"
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-caret-square-down"
                                ]
                            ]
                        ]
                    ]
                ]
                Bulma.button.button [
                    Bulma.color.isPrimary
                    Bulma.button.isText
                    prop.style [
                        style.textDecoration.none
                        style.marginTop 10
                    ]
                    prop.onClick (fun e ->
                        e.stopPropagation ()

                        let outFormSpec =
                            props.FormSpec
                            |> removeFormFieldFromFormSpec props.FormStepNumber props.FormField

                        props.OnChange outFormSpec

                        props.SetActiveField
                            { props.ActiveField with
                                FormFieldNumber = props.FormField.FieldOrder - 1
                                State = Idle
                            }
                    )
                    prop.classes [
                        "button"
                        "is-small"
                        "is-danger"
                    ]
                    // prop.type' "button"
                    prop.children [
                        Html.span [
                            prop.className "icon"
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-trash"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    else
        Html.none

type FieldContainerProps =
    {|
        FormSpec: FormSpec
        FormStep: FormStep
        FormField: FormField
        ActiveField: ActiveField
        OnChange: FormSpec -> unit
        SelectedStepNumber: int
        SetStepNumber: int -> unit
        SetActiveField: ActiveField -> unit
        IsFieldDragging: bool
        SetFieldDragging: bool -> unit
        RegisteredFields: RegisteredFields
    |}

[<ReactComponent>]
let FieldContainer (props: FieldContainerProps) =
    let isBeingDraggedOver, setIsBeingDraggedOver = React.useState false

    Html.div [
        prop.name "field-container"
        // prop.style [
        //     style.display.flex
        //     // style.justifyContent.start
        //     style.alignItems.center
        // ]
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
                prop.className classes.fieldBeforeAndAfter
            ]
            Html.div [
                prop.style [
                    style.width (length.perc 100)
                ]
                prop.classes [
                    classes.field
                    if props.IsFieldDragging then
                        classes.fieldDraggingHint

                    if isBeingDraggedOver then
                        classes.fieldIsBeingDraggedOver

                    if
                        props.FormField.FieldOrder = props.ActiveField.FormFieldNumber
                        && props.FormStep.StepOrder = props.SelectedStepNumber
                    then
                        classes.fieldSelected
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormField.FieldOrder < props.ActiveField.FormFieldNumber
                        && props.FormStep.StepOrder = props.SelectedStepNumber
                    then
                        classes.fieldDrag
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormField.FieldOrder > props.ActiveField.FormFieldNumber
                    then //&& step.StepOrder < props.SelectedStepNumber then
                        classes.disabled
                    else if
                        props.ActiveField.State = AddingDependantKeys
                        && props.FormStep.StepOrder <> props.SelectedStepNumber
                    then
                        classes.disabled
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
                            LabelEdit
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
                    MockField.MockField
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
                prop.className classes.fieldBeforeAndAfter
            ]
        // Html.span [
        //     // prop.onDragStart (fun e ->
        //     //     let data = (DragSource.Designer_FormField_FieldKey props.FormField.FieldKey).ToPlainText
        //     //     e.dataTransfer.setData("text/plain", data)
        //     //     |> ignore
        //     // )
        //     prop.children [
        //         Bulma.icon [
        //             Html.i [
        //                 prop.style [
        //                     style.color "#bfbfbf"
        //                 ]
        //                 prop.className "fas fa-bars"
        //             ]
        //         ]
        //     ]

        // ]
        ]
    ]

type DynamicFormSpecDetailsProps =
    {|
        FormSpec: FormSpec
        OnChange: FormSpec -> unit
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let DynamicFormSpecDetails (props: DynamicFormSpecDetailsProps) =
    let formSpecJson, setFormSpecJson = React.useState ""

    let formSpecBase64, setFormSpecBase64 = React.useState ""

    React.fragment [
        Html.hr []
        Bulma.columns [
            Bulma.column [

                BulmaHorizontalField
                    {|
                        Label = Some "Title"
                        Placeholder = "Name as it will appear in the assessments list"
                        Value = props.FormSpec.Title
                        Layout = BulmaFleldLayout.Horizontal
                        OnChange =
                            (fun e ->
                                props.OnChange(
                                    { props.FormSpec with
                                        Id = Guid.NewGuid()
                                        Title = e
                                    }
                                )
                            )
                        LeftIcon = Some("fas fa-heading")
                        RightIcon = None
                        ActiveField = props.ActiveField
                    |}
                BulmaHorizontalField
                    {|
                        Label = Some "Abstract"
                        Placeholder = "Short description of the form, and its purpose"
                        Value = props.FormSpec.Abstract
                        Layout = BulmaFleldLayout.Horizontal
                        OnChange =
                            (fun e ->
                                props.OnChange(
                                    { props.FormSpec with
                                        Abstract = e
                                    }
                                )
                            )
                        LeftIcon = Some("fas fa-paragraph")
                        RightIcon = None
                        ActiveField = props.ActiveField
                    |}

                Antidote.React.FormDesigner.Designer.CategoryTags.CategoryTags
                    {|
                        FormSpec = props.FormSpec
                        OnChange = props.OnChange
                    |}
                Antidote.React.FormDesigner.Designer.AssociatedCodes.AssociatedCodes
                    {|
                        FormSpec = props.FormSpec
                        OnChange = props.OnChange
                    |}
                SpecScore
                    {|
                        FormSpec = props.FormSpec
                        OnChange = props.OnChange
                    |}
            ]
        // Bulma.column [
        // ]
        ]
    ]

// type FormSpecCategory =
//     | Template
//     | Draft
//     | Published
//     | UnPublished
//     | All

//     member x.Key =
//         match x with
//         | Template -> "Template"
//         | Draft -> "Draft"
//         | Published -> "Published"
//         | UnPublished -> "Unpublished"
//         | All -> "All"

// // we need to be able to trigger a reload
// type LoadExistingFormProps = {|
//     FormSpecCategory: FormSpecCategory
//     SetFormSpec: FormSpec -> unit
// |}

// [<ReactComponent>]
// let LoadExistingForm(props: LoadExistingFormProps) =
//     let activeTab, setActiveTab = React.useState<FormSpecCategory> props.FormSpecCategory
//     // let formSpecsFromStore, setFormSpecsFromStore = React.useState<Map<FormSpecCategory, FormSpecDto list>>(Map.empty)

//     // let getFormSpecsByCategory category: Async<Antidote.Core.V2.Types.FormSpecDto list> =
//     //     async {
//     //         let existingCategory =
//     //             formSpecsFromStore
//     //             |> Map.tryFind category
//     //             // |> Option.isSome

//     //         match existingCategory with
//     //         | Some cat ->
//     //             return cat
//     //         | None ->
//     //             let request : Antidote.Core.V2.Domain.Form.Request.ReadAllFormSpecs = { FormSpecCategory = category }
//     //             let! specs =
//     //                 Antidote.Client.User.UserSession.Instance.RemotingRequest
//     //                     Antidote.Client.API.EndPoints.form
//     //                     (fun services -> services.ReadAllFormSpecs)
//     //                     request

//     //             match specs with
//     //             | Form.Response.ReadAllFormSpecs.ReadFormSpecs formSpecDtos ->
//     //                 let replaceTemplateGuid =
//     //                     formSpecDtos
//     //                     |> List.map (
//     //                         fun x ->
//     //                             match x.Status with
//     //                             | Antidote.Core.V2.Types.SpecStatus.Template ->
//     //                                 { x with Id = Guid.NewGuid(); Code = None }
//     //                             | _ -> x
//     //                     )
//     //                     |> List.sortBy (fun x -> x.Title)
//     //                 return replaceTemplateGuid
//     //             | _ ->
//     //                 return (failwith $"Failed to load form specs for category: {category}")
//     //     }

//     // React.useEffect(
//     //     (fun _ ->
//     //        async {
//     //         match activeTab with
//     //         | Antidote.Core.V2.Types.FormSpecCategory.All ->
//     //             let! drafts = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Draft
//     //             let! templates = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Template
//     //             let! published = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Published

//     //             formSpecsFromStore
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Draft drafts
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Template templates
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Published published
//     //             |> setFormSpecsFromStore

//     //         | Antidote.Core.V2.Types.FormSpecCategory.Draft ->
//     //             let! a = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Draft
//     //             formSpecsFromStore
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Draft a
//     //             |> setFormSpecsFromStore

//     //         | Antidote.Core.V2.Types.FormSpecCategory.Template ->
//     //             let! a = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Template
//     //             formSpecsFromStore
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Template a
//     //             |> setFormSpecsFromStore

//     //         | Antidote.Core.V2.Types.FormSpecCategory.Published ->
//     //             let! a = getFormSpecsByCategory Antidote.Core.V2.Types.FormSpecCategory.Published
//     //             formSpecsFromStore
//     //             |> Map.add Antidote.Core.V2.Types.FormSpecCategory.Published a
//     //             |> setFormSpecsFromStore

//     //         | Antidote.Core.V2.Types.FormSpecCategory.UnPublished ->
//     //             ()

//     //         } |> Async.StartImmediate

//     //     ) , [| box activeTab |]
//     // )

//     Html.nav [
//         prop.className "panel"
//         prop.children [
//             Html.p [
//                 prop.className "panel-heading"
//                 prop.text "Load Existing Form"
//             ]
//             Html.div [
//                 prop.className "panel-block"
//                 prop.children [
//                     Html.div [
//                         prop.classes [ "control"; "has-icons-left" ]
//                         prop.children [
//                             Html.input [
//                                 prop.className "input"
//                                 prop.type' "text"
//                                 prop.placeholder "Search"
//                             ]
//                             Html.span [
//                                 prop.classes [ "icon"; "is-left" ]
//                                 prop.children [
//                                     Html.i [
//                                         prop.classes [ "fas"; "fa-search" ]
//                                     ]
//                                 ]
//                             ]

//                             let tabs = [
//                                 FormSpecCategory.Draft
//                                 FormSpecCategory.Template
//                                 FormSpecCategory.Published
//                                 FormSpecCategory.All
//                             ]
//                             // PANEL TABS HERE
//                             Bulma.panelTabs [
//                                 tabs
//                                 |> List.map (fun tab ->
//                                     Html.a [
//                                         if activeTab = tab
//                                         then prop.className "is-active"
//                                         prop.onClick ( fun _ -> setActiveTab tab)
//                                         prop.text (tab.Key)
//                                     ]
//                                 ) |> React.fragment
//                             ]
//                         ]
//                     ]
//                 ]
//             ]

//             // formSpecsFromStore
//             // //if active tab = all, do not filter. Otherwise, filter by active tab
//             // |> fun a ->
//             //     if activeTab = FormSpecCategory.All
//             //     then a
//             //     else a |> Map.filter (fun k _ -> k = activeTab)
//             // |> Map.map (fun category formSpecDtoList ->
//             //     formSpecDtoList
//             //     |> List.map (fun formSpecDto ->
//             //         let categoryColor =
//             //             match category with
//             //             | FormSpecCategory.Draft -> "orange"
//             //             | FormSpecCategory.Template -> "#0041b9"
//             //             | FormSpecCategory.Published -> "#ff4caa"
//             //             | _ -> "is-danger"
//             //         Html.a [
//             //             prop.onClick (fun _ ->
//             //                 formSpecDto.FormSpecJson
//             //                 |> Migrator.FormSpec.decodeFormSpec Migrator.FormSpec.FormSpecInput.Unknown
//             //                 |> Migrator.FormSpec.migrateTo Migrator.FormSpec.FormSpecOutput.Latest
//             //                 |> fun x ->
//             //                     match x with
//             //                     | Migrator.FormSpec.FormSpecVersion.V2_0_1_FormSpec formSpec ->
//             //                         { formSpec with Id = formSpecDto.Id; Code = formSpecDto.Code }
//             //                     | _ ->
//             //                         failwith $"This build is incompatible with the form spec: {x}"
//             //                 |> props.SetFormSpec
//             //             )
//             //             prop.className "panel-block"
//             //             prop.children [
//             //                 Html.span [
//             //                     prop.style [
//             //                         style.color categoryColor
//             //                     ]
//             //                     prop.className "panel-icon"
//             //                     prop.children [
//             //                         Html.i [
//             //                             prop.classes [ "fas"; "fa-book" ]
//             //                         ]
//             //                     ]
//             //                 ]

//             //                 Html.span [ prop.text formSpecDto.Title ]
//             //             ]
//             //         ]
//             //     )
//             // )
//             // |> Seq.toList
//             // |> Seq.collect (fun a -> a.Value)
//             // |> React.fragment
//         ]
//     ]

type NavigationMenuProps =
    {|
        FormSpec: FormSpec
        OnChange: FormSpec -> unit
        SetIsPreview: bool -> unit
        ActiveField: ActiveField
        SaveFormSpec: bool -> unit
        SetIsLoadExistingFormShowing: bool -> unit
    |}

[<ReactComponent>]
let NavigationMenu (props: NavigationMenuProps) =
    let navbarBurgerIsActive, setNavbarBurgerIsActive = React.useState false
    // let showLoadExistingForm, setShowLoadExistingForm = React.useState false

    Html.nav [
        prop.className [
            if props.ActiveField.State = AddingDependantKeys then
                classes.disabled
        ]

        prop.className "navbar is-dark"
        prop.style [
            style.custom ("zIndex", "unset")
        ]
        prop.children [
            Html.div [
                prop.className "container"
                prop.children [

                    Html.div [
                        prop.className "navbar-brand"
                        prop.children [
                            // Html.a [
                            //     prop.className "navbar-item"
                            //     prop.href "https://bulma.io"
                            //     prop.children [
                            //         Html.img [
                            //             prop.src "https://bulma.io/images/bulma-logo.png"
                            //             prop.alt "Bulma: a modern CSS framework based on Flexbox"
                            //             prop.width 112
                            //             prop.height 28
                            //         ]
                            //     ]
                            // ]
                            Html.a [
                                prop.text "NEW"
                                prop.onClick (fun _ -> props.OnChange defaultFormSpec)
                                prop.classes [
                                    "navbar-item"
                                    "is-hidden-desktop"
                                ]
                            ]
                            Html.a [
                                prop.text "SAVE"
                                prop.onClick (fun _ -> props.SaveFormSpec false)
                                prop.classes [
                                    "navbar-item"
                                    "is-hidden-desktop"
                                ]
                            ]

                            Html.a [
                                prop.text "PUBLISH"
                                prop.onClick (fun _ -> props.SaveFormSpec true)
                                prop.classes [
                                    "navbar-item"
                                    "is-hidden-desktop"
                                ]
                            ]

                            Html.a [
                                prop.onClick (fun _ -> props.SetIsLoadExistingFormShowing true)
                                prop.text "LOAD"
                                prop.classes [
                                    "navbar-item"
                                    "is-hidden-desktop"
                                ]
                            ]

                            Html.div [
                                prop.classes [
                                    "navbar-item"
                                    "is-hidden-desktop"
                                ]
                                prop.children [
                                    Html.div [
                                        prop.classes [
                                            "field"
                                            "is-grouped"
                                        ]
                                        prop.children [
                                            Html.p [
                                                prop.className "control"
                                                prop.children [
                                                    Html.a [
                                                        prop.classes [
                                                            "button"
                                                            "is-primary"
                                                        ]
                                                        if
                                                            props.FormSpec.Steps
                                                            |> allStepsHaveFields
                                                        then
                                                            prop.onClick (fun _ ->
                                                                props.SetIsPreview true
                                                            )
                                                        else
                                                            prop.disabled true

                                                        prop.children [
                                                            Html.span [
                                                                prop.className "icon"
                                                                prop.children [
                                                                    Html.i [
                                                                        prop.classes [
                                                                            "fas"
                                                                            "fa-eye"
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                            Html.span "PREVIEW"
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]

                        // Html.div [
                        //     prop.onClick (fun _ -> setNavbarBurgerIsActive (not navbarBurgerIsActive) )
                        //     prop.classes [
                        //         "navbar-burger"
                        //         "burger"

                        //         if navbarBurgerIsActive
                        //         then "is-active"
                        //         else ""
                        //     ]
                        //     prop.children [
                        //         Html.span []
                        //         Html.span []
                        //         Html.span []
                        //     ]
                        // ]
                        ]
                    ]
                    Html.div [
                        prop.className "navbar-menu"
                        prop.id "navMenuExample11"
                        prop.children [
                            Html.div [
                                prop.classes [
                                    "navbar-start"
                                    if navbarBurgerIsActive then
                                        "is-active"
                                    else
                                        ""
                                ]
                                prop.children [
                                    Html.a [
                                        prop.text "NEW"
                                        prop.onClick (fun _ -> props.OnChange defaultFormSpec)
                                        prop.classes [
                                            "navbar-item"
                                            ""
                                        ]

                                    ]

                                    Html.a [
                                        prop.text "SAVE"
                                        prop.onClick (fun _ -> props.SaveFormSpec false)
                                        prop.classes [
                                            "navbar-item"
                                            ""
                                        ]
                                    ]

                                    Html.a [
                                        prop.text "PUBLISH"
                                        prop.onClick (fun _ -> props.SaveFormSpec true)
                                        prop.classes [
                                            "navbar-item"
                                            ""
                                        ]
                                    ]

                                    Html.a [
                                        prop.onClick (fun _ ->
                                            props.SetIsLoadExistingFormShowing true
                                        )
                                        prop.text "LOAD"
                                        prop.classes [
                                            "navbar-item"
                                            ""
                                        ]
                                    ]

                                    Html.div [
                                        prop.classes [
                                            "navbar-item"
                                            "has-dropdown"
                                            "is-hoverable"
                                        ]
                                        prop.children [
                                            Html.div [
                                                prop.className "navbar-link"
                                                prop.text "More "
                                            ]
                                            Html.div [
                                                prop.classes [
                                                    "navbar-dropdown"
                                                    ""
                                                ]
                                                prop.id "moreDropdown"
                                                prop.children [
                                                    Html.a [
                                                        prop.text "Log Spec"
                                                        prop.onClick (fun _ ->
                                                            printfn
                                                                $"FormSpec: {Thoth.Json.Encode.Auto.toString (props.FormSpec)}"
                                                        // printfn $"Encoded: {Base64.encode(Thoth.Json.Encode.Auto.toString(props.FormSpec))}"
                                                        )
                                                        prop.classes [
                                                            "navbar-item"
                                                            ""
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.className "navbar-end"
                                prop.children [
                                    Html.div [
                                        prop.className "navbar-item"
                                        prop.children [
                                            Html.div [
                                                prop.classes [
                                                    "field"
                                                    "is-grouped"
                                                ]
                                                prop.children [
                                                    Html.p [
                                                        prop.className "control"
                                                        prop.children [
                                                            Html.a [
                                                                prop.classes [
                                                                    "button"
                                                                    "is-primary"
                                                                ]
                                                                if
                                                                    props.FormSpec.Steps
                                                                    |> allStepsHaveFields
                                                                then
                                                                    prop.onClick (fun _ ->
                                                                        props.SetIsPreview true
                                                                    )
                                                                else
                                                                    prop.disabled true

                                                                prop.children [
                                                                    Html.span [
                                                                        prop.className "icon"
                                                                        prop.children [
                                                                            Html.i [
                                                                                prop.classes [
                                                                                    "fas"
                                                                                    "fa-eye"
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    ]
                                                                    Html.span "PREVIEW"
                                                                ]
                                                            ]
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

type LoadExistingFormModalProps =
    {|
        IsShowing: bool
        SetIsShowing: bool -> unit
        SetStepNumber: int -> unit
        SetFormSpec: FormSpec -> unit
    |}

[<ReactComponent>]
let LoadExistingFormModal (props: LoadExistingFormModalProps) =
    Html.div [
        prop.classes [
            "modal"
            if props.IsShowing then
                "is-active"
            else
                "is-hidden"
        ]
        prop.children [
            Html.div [
                prop.className "modal-background"
                prop.onClick (fun _ -> props.SetIsShowing false)
            ]
            Html.div [
                prop.className "modal-content"
                prop.style [
                    style.backgroundColor "white"
                ]

                prop.children [
                // LoadExistingForm {|
                //     FormSpecCategory =
                //         FormSpecCategory.Draft
                //     SetFormSpec =
                //         fun formSpec ->
                //             props.SetStepNumber 1
                //             props.SetIsShowing false
                //             props.SetFormSpec formSpec
                // |}
                ]
            ]
            Html.button [
                prop.classes [
                    "modal-close"
                    "is-large"
                ]
                prop.ariaLabel "close"
            ]
        ]
    ]

type FormSpecLayoutProps =
    {|
        FormSpec: FormSpec
        OnChange: FormSpec -> unit
        SetIsPreview: bool -> unit
        SelectedStepNumber: int
        SetStepNumber: int -> unit
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        IsFieldDragging: bool
        SetFieldDragging: bool -> unit
        RegisteredFields: RegisteredFields
    |}

[<ReactComponent>]
let FormSpecLayout (props: FormSpecLayoutProps) =
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
                        Html.article [
                            prop.classes [
                                classes.dropFieldsContainer
                            ]
                            prop.onDragOver (fun (e: Types.DragEvent) -> e.preventDefault ())
                            prop.onDrop (fun (e: Types.DragEvent) ->
                                e.preventDefault ()

                                let dragSource =
                                    e.dataTransfer.getData ("text/plain")
                                    |> tryGetDragSourceFromData

                                match dragSource with
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
                            )
                            prop.children [

                                Html.div [
                                    prop.className ""
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
                                        Html.span $"Drag fields for step {step.StepOrder} here"
                                    ]
                                ]
                            ]
                        ]
                    | _ ->
                        Html.div [
                            prop.children [
                                step.Fields
                                |> List.sortBy (fun f -> f.FieldOrder)
                                |> List.map (fun formField ->
                                    // let isFieldSelected = selectField.IsSome && selectField.Value = formField

                                    FieldContainer
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
                            OnChange =
                                (fun formSpec ->
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

type FormSpecToolsProps =
    {|
        FormSpec: FormSpec
        SelectedStepNumber: int
        FormSpecChanged: FormSpec -> unit
        IsPreview: bool
        ActiveField: ActiveField
        RegisteredFields: RegisteredFields
    |}

[<ReactComponent>]
let FormSpecTools (props: FormSpecToolsProps) =
    Html.div [
        prop.className "panel"
        prop.children [
            props.RegisteredFields.AsList
            |> List.map (fun designerFieldType ->
                Html.a [
                    prop.onClick (fun e ->
                        //add designer field type to selected step via CLICK
                        let outFormSpec =
                            addDesignerFieldTypeToStep
                                designerFieldType.Key
                                props.SelectedStepNumber
                                props.FormSpec
                                props.RegisteredFields

                        props.FormSpecChanged outFormSpec
                    )
                    prop.draggable true
                    prop.onDragStart (fun e ->
                        let data =
                            (DragSource.Designer_FormFieldType_Key designerFieldType.Key)
                                .ToPlainText

                        e.dataTransfer.setData ("text/plain", data) |> ignore
                    )
                    prop.className "panel-block"
                    prop.children [
                        Html.span [
                            prop.className "panel-icon"
                            prop.children [
                                Html.i [
                                    prop.className designerFieldType.Icon
                                ]
                            ]
                        ]
                        Html.text designerFieldType.Key
                    ]
                ]
            )
            |> React.fragment
        ]
    ]

type DynamicFormPreviewProps =
    {|
        FormSpec: FormSpec
        SetIsPreview: bool -> unit
        RenderUserField:
            bool
                -> FormCompose.ComposerFunc
                -> FormField
                -> Form.Form<DynamicStepValues, string, IReactProperty>
    |}

[<ReactComponent>]
let DynamicFormPreview (props: DynamicFormPreviewProps) =
    let showValuesReview, setShowValuesReview = React.useState false
    let dynamicForm, setDynamicForm = React.useState None

    Bulma.columns [

        Bulma.column [
            if showValuesReview then
                column.is6
            else
                column.is12

            prop.children [
                Bulma.button.a [
                    color.isPrimary
                    prop.style [
                        style.marginBottom 15
                        style.marginTop 15
                    ]
                    prop.text "Exit Preview Mode"
                    prop.onClick (fun _ -> props.SetIsPreview false)
                ]

                FormCompose.FormCompose(
                    {|
                        FormSpec = props.FormSpec
                        DynamicForm = None
                        Mode = FormComposeMode.Editable
                        NavigateToStep = (fun _ -> ())
                        FormChanged = (fun _ -> ())
                        SaveFormValuesCallback =
                            fun dynamicFormValues ->
                                setShowValuesReview true
                                setDynamicForm (Some dynamicFormValues)

                        SubmissionSuccess = false
                        RenderUserField = props.RenderUserField
                    |}
                )
            ]
        ]

        if showValuesReview && dynamicForm.IsSome then
            let formResultData: DynamicFormResultData =
                dynamicForm.Value |> Antidote.FormDesigner.Helper.extractDataFromFableFormsModel

            let dynamicFormResultDataJsonEncoded =
                formResultData
                |> Antidote.Core.FormProcessor.JSON.Values.v2_0_1.DynamicFormResultData.encode
                |> Thoth.Json.Encode.toString 4

            let formCode =
                match dynamicForm.Value.DynamicFormSpecDetails.FormSpecCode with
                | Some formCode -> formCode
                | None -> ""

            Bulma.column [
                column.is6
                prop.children [
                    Bulma.button.a [
                        color.isPrimary
                        prop.style [
                            style.marginBottom 15
                            style.marginTop 15
                        ]
                        prop.text "Hide Form Review"
                        prop.onClick (fun _ -> setShowValuesReview false)
                    ]

                    match props.FormSpec.Score with
                    | Some _ ->
                        // let score, flags =
                        //     dynamicForm.Value
                        //     |> Antidote.FormDesigner.Helper.flattenFormSteps
                        //     |> (Antidote.Core.FormProcessor.Processors.Default.getCalculator formCode)

                        // formCode
                        // |> Antidote.React.Components.FormWizard.Processors.Components.getResultOutput
                        // |> fun resultOutputFunc -> resultOutputFunc score flags
                        // |> Antidote.React.Components.FormWizard.Processors.Components.formResultRenderer
                        Html.span "IMPLEMENT CALCULATORS"

                    | None ->
                        Html.div [
                            Html.span [
                                prop.style [
                                    style.fontSize (length.px 16)
                                    style.fontWeight 600
                                    style.color "#bfbfbf"
                                ]
                                prop.text "** THIS FORM IS NOT SCORED **"
                            ]
                        ]

                    SinglePageReview
                        {|
                            FormSpec = props.FormSpec
                            DynamicForm = dynamicForm.Value
                            RenderUserField = props.RenderUserField
                        |}
                ]
            ]
    ]

[<ReactComponent>]
let DynamicFormDesigner
    (formSpec: FormSpec)
    (setFormSpec: FormSpec -> unit)
    (registeredFields: IDesignerField list)
    renderUserField
    =
    let defaultActiveField =
        {
            FormStepNumber = 1
            FormFieldNumber = 0
            State = Idle
        }

    let selectedStepNumber, setSelectedStepNumber =
        React.useState (formSpec.Steps.[0].StepOrder)

    let activeField, setActiveField = React.useState defaultActiveField

    let isFieldDragging, setFieldDragging = React.useState false

    let setActiveFieldWrapper activeField =
        if activeField.FormFieldNumber < 1 then
            setActiveField defaultActiveField
        else
            setActiveField activeField

    let isPreviewing, setIsPreviewing = React.useState false

    let setFormSpecWrapper spec = setFormSpec spec

    let isLoadExistingFormShowing, setIsLoadExistingFormShowing = React.useState false

    let registeredFields = RegisteredFields registeredFields

    React.fragment [
        LoadExistingFormModal
            {|
                IsShowing = isLoadExistingFormShowing
                SetIsShowing = setIsLoadExistingFormShowing
                SetStepNumber = setSelectedStepNumber
                SetFormSpec = setFormSpec
            |}

        if isPreviewing then
            DynamicFormPreview
                {|
                    FormSpec = formSpec
                    SetIsPreview = setIsPreviewing
                    RenderUserField = renderUserField
                |}
        else
            Html.header [
                prop.children [
                    NavigationMenu
                        {|
                            FormSpec = formSpec
                            OnChange = setFormSpecWrapper
                            SetIsPreview = setIsPreviewing
                            ActiveField = activeField
                            SetIsLoadExistingFormShowing = setIsLoadExistingFormShowing
                            SaveFormSpec =
                                fun isPublish ->
                                    // let validationRes = Antidote.Core.V2.Validator.FormSpec.validateFormSpec formSpec
                                    // match validationRes with
                                    // | Ok formSpec ->
                                    //     Antidote.FormDesigner.Helper.saveFormSpec
                                    //         formSpec
                                    //         {
                                    //             IsPrivate = false
                                    //             Status =
                                    //                 if isPublish
                                    //                 then SpecStatus.Types.SpecStatus.Published
                                    //                 else SpecStatus.Types.SpecStatus.Draft
                                    //         }
                                    //         setFormSpecWrapper

                                    // | Error errs ->
                                    //     for err in errs do
                                    //         toast ( Html.div $"Validation error: {err}" ) |> ignore
                                    printfn "IMPLEMENT SAVE!!!#"
                        |}
                ]
            ]

            Html.section [
                prop.className "section"
                prop.children [
                    Html.div [
                        prop.className "container"
                        prop.children [
                            Html.div [
                                prop.className "columns"
                                prop.children [
                                    Html.div [
                                        prop.className "colum is-4"
                                        prop.children [
                                            FormSpecTools
                                                {|
                                                    FormSpec = formSpec
                                                    SelectedStepNumber = selectedStepNumber
                                                    IsPreview = isPreviewing
                                                    FormSpecChanged = setFormSpecWrapper
                                                    ActiveField = activeField
                                                    RegisteredFields = registeredFields
                                                |}
                                        ]
                                    ]

                                    Html.div [
                                        prop.className "column is-8"
                                        prop.children [
                                            FormSpecLayout
                                                {|
                                                    RegisteredFields = registeredFields
                                                    FormSpec = formSpec
                                                    OnChange = setFormSpecWrapper
                                                    SetIsPreview = setIsPreviewing
                                                    SelectedStepNumber = selectedStepNumber
                                                    SetStepNumber = setSelectedStepNumber
                                                    ActiveField = activeField
                                                    SetActiveField = setActiveFieldWrapper
                                                    IsFieldDragging = isFieldDragging
                                                    SetFieldDragging = setFieldDragging
                                                |}
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
    ]
