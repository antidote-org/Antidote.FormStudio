module Antidote.FormStudio.UI.FieldToolbar

open Feliz
open Feliz.Bulma
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper

type FieldToolbarProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStepNumber: int
        FormField: FormField<'UserField>
        OnChange: FormSpec<'UserField> -> unit
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
    |}

[<ReactComponent>]
let FieldToolbar (props: FieldToolbarProps<'UserField>) =
    let step = props.FormSpec |> tryFindFormStepByStepNumber props.FormStepNumber

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
                        style.marginTop 10
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
                    color.isDanger

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
