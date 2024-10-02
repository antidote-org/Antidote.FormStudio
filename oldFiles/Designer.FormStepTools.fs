module Antidote.FormDesigner.FormStepTools

open Feliz
open Feliz.Bulma
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Fable.Core.JsInterop

// open Feliz.Iconify
// open type Offline.Exports
// open Glutinum.IconifyIcons.Mdi
open Helper


let private classes : CssModules.DynamicFormDesigner = import "default" "./DynamicFormDesigner.module.scss"

type FormStepTools = {|
    FormSpec: FormSpec
    FormStep: FormStep
    OnChange: FormSpec -> unit
|}

[<ReactComponent>]
let FormStepTools (props: FormStepTools) =
    Html.div [
        prop.style [style.display.flex; style.justifyContent.spaceBetween; style.margin 5]
        prop.children [
            Html.input [
                prop.classes [
                    classes.inputAsLabel
                    classes.inputStep
                ]
                prop.type' "text"
                prop.value props.FormStep.StepLabel
                prop.onChange (fun (e:string) ->
                    let newStep = {
                        props.FormStep with
                            StepLabel = e
                    }
                    let newFormSpec = {
                        props.FormSpec with
                            Steps =
                                props.FormSpec.Steps
                                |> List.map (fun s ->
                                    if s.StepOrder = props.FormStep.StepOrder then
                                        newStep
                                    else
                                        s
                                )
                    }
                    props.OnChange newFormSpec
                )
            ]
            Bulma.buttons [
                prop.style [style.paddingBottom 10]
                prop.children [
                    Bulma.button.button [
                        Bulma.color.isPrimary
                        Bulma.button.isText
                        prop.style [ style.textDecoration.none; style.marginTop 10]
                        prop.onClick (fun _ ->
                            let outFormSpec = props.FormSpec |> moveFormStepUpInFormSpec props.FormStep.StepOrder
                            props.OnChange outFormSpec
                        )
                        prop.disabled (props.FormStep.StepOrder = 1 )
                        prop.classes [ "button"; "is-small"; "is-primary" ]
                        // prop.type' "button"
                        prop.children [
                            Html.span [
                                prop.className "icon"
                                prop.children [
                                    Html.i [ prop.className "fas fa-caret-square-up" ]
                                    // Icon [
                                    //     icon.icon mdi.arrowUpBold
                                    //     icon.color "#FFFFFF"
                                    //     icon.width 35
                                    //     icon.height 35
                                    // ]
                                ]
                            ]
                        ]
                    ]
                    Bulma.button.button [
                        Bulma.color.isPrimary
                        Bulma.button.isText
                        prop.style [ style.textDecoration.none; style.marginTop 10]
                        prop.onClick (fun _ ->
                            let outFormSpec = props.FormSpec |> moveFormStepDownInFormSpec props.FormStep.StepOrder
                            props.OnChange outFormSpec
                        )
                        prop.disabled (props.FormStep.StepOrder = (props.FormSpec.Steps |> List.length))
                        prop.classes [ "button"; "is-small"; "is-primary" ]
                        // prop.type' "button"
                        prop.children [
                            Html.span [
                                prop.className "icon"
                                prop.children [
                                    Html.i [ prop.className "fas fa-caret-square-down" ]
                                    // Icon [
                                    //     icon.icon mdi.arrowDownBold
                                    //     icon.color "#FFFFFF"
                                    //     icon.width 35
                                    //     icon.height 35
                                    // ]
                                ]
                            ]
                        ]
                    ]
                    Bulma.button.button [
                        Bulma.color.isPrimary
                        Bulma.button.isText
                        prop.style [ style.textDecoration.none; style.marginTop 10]
                        prop.onClick (fun _ ->
                            let outFormSpec = props.FormSpec |> removeFormStepFromFormSpec props.FormStep.StepOrder
                            props.OnChange outFormSpec
                        )
                        prop.classes [ "button"; "is-small"; "is-danger" ]
                        // prop.type' "button"
                        prop.children [
                            Html.span [
                                prop.className "icon"
                                prop.children [
                                    Html.i [ prop.className "fas fa-trash" ]
                                    // Icon [
                                    //     icon.icon mdi.trashCan
                                    //     icon.color "#FFFFFF"
                                    //     icon.width 35
                                    //     icon.height 35
                                    // ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
