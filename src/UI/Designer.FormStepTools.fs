module Antidote.FormStudio.UI.Designer.FormStepTools

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Antidote.FormStudio
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper

let private classes : CssModules.UI.Designer_FormStepTools =
    import "default" "./Designer.FormStepTools.module.scss"

type FormStepTools<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        OnChange: FormSpec<'UserField> -> unit
    |}

[<ReactComponent>]
let FormStepTools (props: FormStepTools<'UserField>) =
    Html.div [
        prop.style [
            style.display.flex
            style.justifyContent.spaceBetween
            style.margin 5
        ]
        prop.children [
            Html.input [
                prop.classes [
                    GlobalCSS.classes.``input-as-label``
                    classes.``input-step``
                ]
                prop.type' "text"
                prop.value props.FormStep.StepLabel
                prop.onChange (fun (e: string) ->
                    let newStep =
                        { props.FormStep with
                            StepLabel = e
                        }

                    let newFormSpec =
                        { props.FormSpec with
                            Steps =
                                props.FormSpec.Steps
                                |> List.map (fun step ->
                                    if step.StepOrder = props.FormStep.StepOrder then
                                        newStep
                                    else
                                        step
                                )
                        }

                    props.OnChange newFormSpec
                )
            ]
            Bulma.buttons [
                prop.style [
                    style.paddingBottom 10
                ]
                prop.children [
                    Bulma.button.button [
                        color.isPrimary
                        button.isText
                        prop.style [
                            style.textDecoration.none
                            style.marginTop 10
                        ]
                        prop.onClick (fun _ ->
                            let outFormSpec =
                                props.FormSpec |> moveFormStepUpInFormSpec props.FormStep.StepOrder

                            props.OnChange outFormSpec
                        )
                        prop.disabled (props.FormStep.StepOrder = 1)
                        prop.classes [
                            "button"
                            "is-small"
                            "is-primary"
                        ]
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
                        prop.onClick (fun _ ->
                            let outFormSpec =
                                props.FormSpec
                                |> moveFormStepDownInFormSpec props.FormStep.StepOrder

                            props.OnChange outFormSpec
                        )
                        prop.disabled (
                            props.FormStep.StepOrder = (props.FormSpec.Steps |> List.length)
                        )
                        prop.classes [
                            "button"
                            "is-small"
                            "is-primary"
                        ]
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
                        color.isPrimary
                        button.isText
                        prop.style [
                            style.textDecoration.none
                            style.marginTop 10
                        ]
                        prop.onClick (fun _ ->
                            let outFormSpec =
                                props.FormSpec
                                |> removeFormStepFromFormSpec props.FormStep.StepOrder

                            props.OnChange outFormSpec
                        )
                        prop.classes [
                            "button"
                            "is-small"
                            "is-danger"
                        ]
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
        ]
    ]
