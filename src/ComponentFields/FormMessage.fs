module Antidote.React.Components.FormMessage

open System
open Feliz
open Fable.Form.Antidote
open Feliz.UseElmish
open Feliz.Bulma
open Fable.Core
open Fable.Core.JsInterop

open Elmish
open Fable.Core.JS
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.FormStudio.Compose.Types

let private classes: CssModules.ComponentFields.FormMessage =
    import "default" "./FormMessage.module.scss"

[<ReactComponent>]
let FormMessageField
    (messageInfo: MessageInfo)
    (props: Field.ReactComponentField.ReactComponentFieldProps)
    =

    let typeClass =
        match messageInfo.MessageType with
        | MessageType.Error ->
            [
                "is-danger"
                "is-light"
                "has-background-danger-light"
            ]
        | MessageType.Warning ->
            [
                "is-warning"
                "is-light"
                "has-background-warning-light"
            ]
        | MessageType.Info ->
            [
                "is-info"
                "is-light"
                "has-background-info-light"
            ]
        | MessageType.Success ->
            [
                "is-success"
                "is-light"
                "has-background-success-light"
            ]
        | MessageType.Danger ->
            [
                "is-danger"
                "is-light"
                "has-background-danger-light"
            ]

    React.useEffect ((fun _ -> props.OnChange("N/A")), [||])

    Bulma.card [
        prop.className classes.``center-width``
        prop.children [
            Bulma.cardHeader [
                prop.classes [
                    "is-danger"
                    "is-light"
                    "has-background-danger"
                ]
                // prop.classes typeClass
                prop.children [
                    Html.div [
                        prop.className [
                            classes.align
                        ]
                        prop.classes [
                            "align"
                        ]
                        prop.style [
                            style.display.flex
                            style.alignItems.center
                        ]
                        prop.children [
                            Html.span [
                                prop.className [
                                    "icon"
                                ]
                                prop.style [
                                    style.marginRight 7
                                ]
                                prop.children [
                                    Html.i [
                                        prop.className "fas fa-exclamation-triangle"
                                    ]
                                ]
                            ]
                            Html.text (
                                match messageInfo.MessageType with
                                | MessageType.Error -> "Error"
                                | MessageType.Warning -> "Warning"
                                | MessageType.Info -> "Info"
                                | MessageType.Success -> "Success"
                                | MessageType.Danger -> "Danger"
                            )
                        ]
                    ]
                ]
                prop.style [
                    style.display.flex
                    style.justifyContent.center
                    style.alignItems.center
                    style.color "#FFFFFF"
                    style.fontSize 25
                    style.fontWeight.bold
                    style.paddingTop 10
                ]
            ]

            Bulma.cardContent [
                prop.classes typeClass
                // prop.classes [ "is-danger"; "is-light"; "has-background-danger-light" ]

                prop.style [
                    style.display.flex
                    style.justifyContent.center
                ]
                prop.children [
                    Html.div [
                        prop.className [
                            classes.``text-container``
                        ]
                        prop.style [
                            style.fontSize 17
                            style.color "#FF3366"
                        ]
                        prop.children [
                            Html.text messageInfo.Message
                        // Html.text "If the patient acknowledges any form of abuse in response to the following questions with a 'Yes', except for the last question, please promptly report the incident to the Department of Children and Families (DCF)."
                        ]
                    ]
                ]
            ]
            Bulma.level [
                prop.classes [
                    "has-background-danger-light"
                ]
                prop.children [
                    Bulma.levelItem [
                        prop.style [
                            style.marginRight 10
                            style.marginBottom 20
                        ]
                        prop.children [
                            Html.div [
                                prop.className [
                                    classes.align
                                ]
                                prop.classes [
                                    "align"
                                ]
                                prop.style [
                                    style.display.flex
                                    style.alignItems.center
                                    style.color "#FF3366"
                                    style.fontWeight.bold
                                ]
                                prop.children [
                                    Html.span [
                                        prop.style [
                                            style.display.flex
                                            style.alignItems.center
                                        ]
                                        prop.className [
                                            "icon"
                                        ]
                                        prop.style [
                                            style.marginRight 7
                                        ]
                                        prop.children [
                                            Html.i [
                                                prop.className "fas fa-phone"
                                            ]
                                        ]
                                    ]
                                    Html.text (
                                        match messageInfo.Footer with
                                        | Some f -> f
                                        | None -> ""
                                    )
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
