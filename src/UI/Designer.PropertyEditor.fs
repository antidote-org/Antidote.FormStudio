module Antidote.FormStudio.UI.Designer.PropertyEditor

open Antidote.FormStudio.Types
open Feliz
open Feliz.Bulma
open Antidote.FormStudio.UI.Components.Switch
open Antidote.FormStudio.UI.Components.DropFieldsContainer
open Antidote.FormStudio
open Antidote.FormStudio.Helper

let rec private tryFindFieldByFieldKeyInSpec (fieldKey: string) (spec: FormSpec<'UserField>) =
    match spec.Steps with
    | [] -> None
    | step :: steps ->
        match step.Fields with
        | [] ->
            tryFindFieldByFieldKeyInSpec
                fieldKey
                { spec with
                    Steps = steps
                }
        | field :: fields ->
            if field.FieldKey = fieldKey then
                Some field
            else
                tryFindFieldByFieldKeyInSpec
                    fieldKey
                    { spec with
                        Steps =
                            { step with
                                Fields = fields
                            }
                            :: steps
                    }

type CommonFieldPropertiesEditProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField> option
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        FormField: FormField<'UserField>
        OnFormSpecChanged: FormSpec<'UserField> -> unit
    |}

[<ReactComponent>]
let CommonFieldPropertiesEdit (props: CommonFieldPropertiesEditProps<'UserField>) =
    // let isAddingDependentFields, setIsAddingDepenentFields = React.useState false

    React.fragment [
        Bulma.panelBlock.div [
            Html.label [
                prop.style [
                    style.marginRight 5
                ]
                prop.className classes.switch
                prop.children [
                    Html.input [
                        prop.style [
                            style.display.flex
                            style.flexDirection.row
                        ]
                        prop.isChecked props.FormField.IsOptional
                        prop.onChange (fun (e: bool) ->
                            let newFormField =
                                { props.FormField with
                                    IsOptional = e
                                }

                            let newFormSpec =
                                match props.FormStep with
                                | Some formStep ->
                                    updateFormFieldInFormSpecStep
                                        newFormField
                                        formStep
                                        props.FormSpec
                                | None -> failwith "FormStep not found"

                            props.OnFormSpecChanged newFormSpec
                        )
                        prop.type' "checkbox"

                    ]
                    Html.span [
                        prop.classes [
                            classes.slider
                            classes.round
                        ]
                    ]
                ]
            ]
            Html.text "Optional"
        ]
        Bulma.panelBlock.div [
            Html.label [
                prop.style [
                    style.marginRight 5
                ]
                prop.className classes.switch
                prop.children [
                    Html.input [
                        prop.style [
                            style.display.flex
                            style.flexDirection.row
                        ]
                        prop.isChecked props.FormField.IsDeprecated
                        prop.onChange (fun (e: bool) ->
                            let newFormField =
                                { props.FormField with
                                    IsDeprecated = e
                                }

                            let newFormSpec =
                                match props.FormStep with
                                | Some formStep ->
                                    updateFormFieldInFormSpecStep
                                        newFormField
                                        formStep
                                        props.FormSpec
                                | None -> failwith "FormStep not found"

                            props.OnFormSpecChanged newFormSpec
                        )
                        prop.type' "checkbox"

                    ]
                    Html.span [
                        prop.classes [
                            classes.slider
                            classes.round
                        ]
                    ]
                ]
            ]
            Html.text "Deprecated"
        ]

        Bulma.panelBlock.div [
            Switch
                {|
                    OnChange =
                        fun e ->
                            props.SetActiveField
                                { props.ActiveField with
                                    State =
                                        if not e then
                                            Idle
                                        else
                                            AddingDependantKeys
                                }

                            if not e then
                                let newFormField =
                                    { props.FormField with
                                        DependsOn = None
                                    }

                                let newFormSpec =
                                    match props.FormStep with
                                    | Some formStep ->
                                        updateFormFieldInFormSpecStep
                                            newFormField
                                            formStep
                                            props.FormSpec
                                    | None -> failwith "FormStep not found"

                                props.OnFormSpecChanged newFormSpec
                    IsEnabled =
                        props.FormField.DependsOn.IsSome
                        || props.ActiveField.State = AddingDependantKeys
                |}

            Html.text "Depends on fields"
        ]

        match props.FormField.DependsOn with
        | None -> Html.none
        | Some dep ->
            let specFieldOpt = props.FormSpec |> tryFindFieldByFieldKeyInSpec dep.FieldKey

            match specFieldOpt with
            | Some specField ->
                Bulma.panelBlock.div [
                    Bulma.label (specField.Label)
                ]

                Bulma.panelBlock.div [
                    Bulma.field.div [
                        Bulma.label [
                            size.isSize6
                            prop.text "Evaluation"
                        ]
                        Bulma.control.div [
                            Bulma.control.hasIconsLeft
                            prop.children [
                                Bulma.select [
                                    prop.value dep.Evaluator.Key
                                    prop.onChange (fun (e: string) ->
                                        let newDependsOn =
                                            { dep with
                                                Evaluator =
                                                    match (tryEvaluationKeyToEvaluation e) with
                                                    | Some e -> e
                                                    | None -> Evaluator.Equals //If no key, default to "Equals". Makes sense for most cases.
                                            }

                                        let newFormField =
                                            { props.FormField with
                                                DependsOn = Some newDependsOn
                                            }

                                        let newFormSpec =
                                            match props.FormStep with
                                            | Some formStep ->
                                                updateFormFieldInFormSpecStep
                                                    newFormField
                                                    formStep
                                                    props.FormSpec
                                            | None -> failwith "FormStep not found"

                                        props.OnFormSpecChanged newFormSpec
                                    )
                                    prop.children [
                                        Html.option "Select an evaluator"
                                        evaluators
                                        |> List.map (fun e -> Html.option (e.Key))
                                        |> React.fragment
                                    ]
                                ]
                                Bulma.icon [
                                    Bulma.icon.isSmall
                                    Bulma.icon.isLeft
                                    prop.children [
                                        Html.i [
                                            prop.className "fas fa-square-root-alt"
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]

                Bulma.panelBlock.div [
                    Bulma.input.text [
                        prop.placeholder "Enter a value to depend on"
                        prop.value dep.FieldValue
                        prop.onChange (fun (e: string) ->
                            let newDependsOn =
                                { dep with
                                    FieldValue = e
                                }

                            let newFormField =
                                { props.FormField with
                                    DependsOn = Some newDependsOn
                                }

                            let newFormSpec =
                                match props.FormStep with
                                | Some formStep ->
                                    updateFormFieldInFormSpecStep
                                        newFormField
                                        formStep
                                        props.FormSpec
                                | None -> failwith "FormStep not found"

                            props.OnFormSpecChanged newFormSpec
                        )
                    ]
                ]
            | None ->
                Html.div [
                    Html.span "Field Not Found"
                ]

        if
            props.ActiveField.State = AddingDependantKeys
            || props.FormField.DependsOn.IsSome
        then
            Bulma.panelBlock.div [
                Html.div [
                    prop.children [
                        DropFieldsContainer
                            {|
                                Children =
                                    [
                                        Html.i [
                                            prop.className "fas arrows-alt"
                                        ]
                                        Html.span "Drag the dependent fields here"
                                    ]
                                OnDrop =
                                    (fun dragSourceOpt ->
                                        match dragSourceOpt with
                                        | None -> ()
                                        | Some(DragSource.Designer_FormField_FieldKey key) ->
                                            let newFormField =
                                                { props.FormField with
                                                    DependsOn =
                                                        Some
                                                            {
                                                                FieldKey = key
                                                                FieldValue = ""
                                                                Evaluator = Evaluator.Equals
                                                            }
                                                }

                                            let newFormSpec =
                                                match props.FormStep with
                                                | Some formStep ->
                                                    updateFormFieldInFormSpecStep
                                                        newFormField
                                                        formStep
                                                        props.FormSpec
                                                | None -> failwith "FormStep not found"

                                            props.OnFormSpecChanged newFormSpec
                                        | _ -> ()
                                    )
                            |}
                        Html.div [
                            prop.className [
                                if props.FormField.DependsOn.IsSome then
                                    ""
                                else
                                    GlobalCSS.classes.disabled
                            ]
                            prop.style [
                                style.display.flex
                                style.justifyContent.center
                                style.marginTop 10
                                style.marginBottom 10
                            ]
                            prop.children [
                                Html.button [
                                    prop.onClick (fun _ ->
                                        props.SetActiveField
                                            {
                                                FormStepNumber = 1
                                                FormFieldNumber = 0
                                                State = Idle
                                            }
                                    )
                                    prop.classes [
                                        "button"
                                        "is-success"
                                        "is-rounded"
                                    ]
                                    prop.children [
                                        Html.span [
                                            prop.classes [
                                                "icon"
                                                "is-small"
                                            ]
                                            prop.children [
                                                Html.i [
                                                    prop.classes [
                                                        "fas"
                                                        "fa-check"
                                                    ]
                                                ]
                                            ]
                                        ]
                                        Html.span "Save"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
    ]

type PropertyEditorProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        IsPreview: bool
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        FormSpecChanged: FormSpec<'UserField> -> unit
    |}

[<ReactComponent>]
let PropertyEditor (props: PropertyEditorProps<'UserField>) =
    let isShown, setIsShown = React.useState true
    let isDeprecatedPending, setIsDeprecatedPending = React.useState false

    // Html.aside [
    //     prop.classes [
    //         classes.toolbarComponentRight
    //         // if not isShown then
    //         if props.IsPreview then
    //             classes.toolbarComponentRightHidden

    //     ]
    //     prop.children [
    Html.nav [
        prop.className "panel"
        prop.children [
            // Html.p [
            //     prop.className "panel-heading"
            //     prop.children[
            //         Bulma.iconText [
            //             prop.onClick (
            //                 fun _ ->
            //                     setIsShown false
            //             )
            //             prop.children [
            //                 // Bulma.icon [
            //                 //     Icon [
            //                 //         icon.icon mdi.closeCircle
            //                 //         icon.width 24
            //                 //     ]
            //                 // ]
            //                 Html.span "Properties"
            //             ]
            //         ]
            //     ]
            // ]

            let formFieldOpt =
                props.FormSpec
                |> tryFindFieldInSpec
                    props.ActiveField.FormFieldNumber
                    props.ActiveField.FormStepNumber

            match formFieldOpt with
            | None ->
                Html.div [
                    prop.style [
                        style.display.flex
                        style.justifyContent.center
                        style.flexDirection.column
                        style.alignItems.center
                    ]
                    prop.children [
                        Html.img [
                            prop.style [
                                style.width 100
                            ]
                            prop.src "/images/empty_form_state.png"
                        ]
                        Html.strong [
                            prop.style [
                                style.fontSize 18
                            ]
                            prop.children [
                                Html.span "Select a Field"
                            ]
                        ]
                        Html.div [
                            prop.style [
                                style.fontSize 14
                                style.color.gray
                                style.marginBottom 15
                            ]
                            prop.children [
                                Html.text "Add and select a form to get started!"
                            ]
                        ]
                    ]
                ]
            | Some formField ->

                Bulma.panelBlock.div [
                    Html.span formField.Label
                ]

                CommonFieldPropertiesEdit
                    {|
                        FormSpec = props.FormSpec
                        FormStep =
                            Helper.tryGetStepByNumber
                                props.ActiveField.FormStepNumber
                                props.FormSpec
                        ActiveField = props.ActiveField
                        SetActiveField = props.SetActiveField
                        FormField = formField
                        OnFormSpecChanged = props.FormSpecChanged
                    |}
        ]
    ]
//     ]
// ]
