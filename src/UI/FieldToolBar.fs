module Antidote.FormStudio.UI.FieldToolbar

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Fields.Html

type FieldToolbarProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        FormStepNumber: int
        FormField: FormField<'UserField>
        OnChange: FormSpec<'UserField> -> unit
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        RegisteredFields: RegisteredFields<'UserField>
        FieldTypePropertyEditor: 'UserField -> Form<'UserField, 'UserField>
    |}

let private renderPropertyEditorModal
    (showModal: bool)
    (closeModal: unit -> unit)
    (content: ReactElement)
    =

    Bulma.modal [
        if showModal then
            modal.isActive
        prop.children [
            Bulma.modalBackground [
                prop.onClick (fun _ -> closeModal ())
            ]
            Bulma.modalClose [
                prop.onClick (fun _ -> closeModal ())
            ]
            Bulma.modalContent [
                Bulma.box [
                    content
                ]
            ]
        ]
    ]

let private formEditor
    (originalField: FormField<_>)
    (fieldTypePropertyEditor: 'UserField -> Form<'UserField, 'UserField>)
    : Form<FormField<'UserField>, FormField<'UserField>>
    =
    let labelField: Form<FormField<'UserField>, string> =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Label
                Update =
                    fun newValue values ->
                        { values with
                            Label = newValue
                        }
                Error = fun _ -> None
                Attributes = TextField.create "label"
            }

    let isOptionalField: Form<FormField<'UserField>, bool> =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.IsOptional
                Update =
                    fun newValue values ->
                        { values with
                            IsOptional = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    CheckboxField.create "is-optional" |> CheckboxField.withText "Is Optional"
            }

    let isDeprecatedField: Form<FormField<'UserField>, bool> =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.IsDeprecated
                Update =
                    fun newValue values ->
                        { values with
                            IsDeprecated = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    CheckboxField.create "is-deprecated" |> CheckboxField.withText "Is Deprecated"
            }

    let fieldTypeField: Form<FormField<'UserField>, 'UserField> =
        fieldTypePropertyEditor originalField.FieldType
        |> Form.mapValues
            {
                Value = fun value -> value.FieldType
                Update =
                    fun fieldType values ->
                        { values with
                            FieldType = fieldType
                        }
            }

    let onSubmit label isOptional isDeprecated userFieldType =
        { originalField with
            Label = label
            IsOptional = isOptional
            IsDeprecated = isDeprecated
            FieldType = userFieldType
        }

    Form.succeed onSubmit
    |> Form.append labelField
    |> Form.append isOptionalField
    |> Form.append isDeprecatedField
    |> Form.append fieldTypeField

[<ReactComponent>]
let private RenderPropertyEditor (props: FieldToolbarProps<'UserField>) (closeModal: unit -> unit) =
    let formState, setFormState = props.FormField |> Form.View.idle |> React.useState

    match props.RegisteredFields.TryGetByKey props.FormField.DesignerFieldKey with
    | Some designerField ->
        Form.View.asHtml
            {
                OnChange = setFormState
                OnSubmit =
                    fun state ->
                        closeModal ()

                        updateFormFieldInFormSpecStep state props.FormStep props.FormSpec
                        |> props.OnChange

                Action = Form.View.Action.SubmitOnly "Save"
                Validation = Form.View.ValidateOnSubmit
            }
            (formEditor props.FormField props.FieldTypePropertyEditor)
            formState

    | None -> Html.none

[<ReactComponent>]
let FieldToolbar (props: FieldToolbarProps<'UserField>) =
    let step = props.FormSpec |> tryFindFormStepByStepNumber props.FormStepNumber
    let showModal, setShowModal = React.useState false

    if
        props.ActiveField.State = AddingDependantKeys
        && props.FormField.FieldOrder < props.ActiveField.FormFieldNumber
    then
        Bulma.tag [
            tag.isRounded
            color.hasBackgroundSuccess
            color.hasTextWhite
            text.hasTextWeightBold

            prop.style [
                style.marginTop 5
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
                style.flexWrap.nowrap
            ]
            prop.children [
                Bulma.button.button [
                    color.isPrimary
                    button.isSmall

                    button.isText
                    prop.style [
                        style.textDecoration.none
                        style.marginTop 5
                    ]
                    prop.disabled (props.FormField.FieldOrder <= 1)

                    prop.onClick (fun e ->
                        e.stopPropagation ()

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
                    color.isPrimary
                    button.isText
                    button.isSmall

                    prop.style [
                        style.textDecoration.none
                        style.marginTop 5
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
                    button.isText
                    button.isSmall
                    color.isPrimary

                    prop.style [
                        style.textDecoration.none
                        style.marginTop 5
                    ]

                    prop.onClick (fun e ->
                        e.stopPropagation ()

                        setShowModal true
                    )

                    prop.children [
                        Html.span [
                            prop.className "icon"
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-edit"
                                ]
                            ]
                        ]
                    ]
                ]

                Bulma.button.button [
                    button.isText
                    button.isSmall
                    color.isDanger

                    prop.style [
                        style.textDecoration.none
                        style.marginTop 5
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

                let closePropertyEditorModal () = setShowModal false

                renderPropertyEditorModal
                    showModal
                    closePropertyEditorModal
                    (RenderPropertyEditor props closePropertyEditorModal)
            ]
        ]
    else
        Html.none
