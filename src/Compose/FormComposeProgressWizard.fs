module Antidote.React.Components.FormWizard.FormComposeProgressWizard

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop

let private classes: CssModules.FormWizard.Compose.FormComposeProgressWizard =
    import "default" "./FormComposeProgressWizard.module.scss"

type FormComposeProgressWizardProps =
    {|
        FormProgressTitle: string
        FormProgressSubTitle: string
        FormCurrentStep: int
        FormTotalSteps: int
        FormProgressStepAction: int -> unit
    |}

[<ReactComponent>]
let FormComposeProgressWizard (props: FormComposeProgressWizardProps) =
    Html.div
        [
            Bulma.media
                [
                    Bulma.mediaLeft
                        [
                            prop.className "media-left"
                            prop.children
                                [
                                    Html.i
                                        [
                                            prop.style [ style.color "#26619B" ]
                                            prop.className "fas fa-heartbeat fa-4x"
                                        ]
                                ]
                        ]
                    Bulma.mediaContent
                        [
                            prop.className "media-content"
                            prop.children
                                [
                                    Html.p
                                        [
                                            prop.className "subtitle is-5"
                                            prop.text props.FormProgressTitle
                                        ]
                                    Html.p
                                        [
                                            prop.className "subtitle is-6"
                                            prop.text props.FormProgressSubTitle
                                        ]
                                ]
                        ]
                ]

            if props.FormTotalSteps > 1 then
                Html.ul
                    [
                        prop.classes
                            [
                                "steps"
                                "is-thin"
                                (if props.FormTotalSteps > 8 then
                                     "is-small"
                                 else
                                     "is-medium")
                            ]
                        prop.style
                            [
                                style.custom ("maxWidth", "80%")
                                style.custom ("marginLeft", "auto")
                                style.custom ("marginRight", "auto")
                            ]
                        prop.children
                            [
                                for s in [ 1 .. props.FormTotalSteps ] do
                                    Html.li
                                        [
                                            prop.classes
                                                [
                                                    "steps-segment"
                                                    (if props.FormCurrentStep = s then
                                                         "is-active"
                                                     else
                                                         "")
                                                ]
                                            prop.children
                                                [
                                                    Html.a
                                                        [
                                                            prop.title (sprintf "Step %i" s)
                                                            prop.style (
                                                                if props.FormCurrentStep >= s then
                                                                    []
                                                                else
                                                                    [ style.cursor.defaultCursor ]
                                                            )
                                                            prop.onClick (
                                                                if props.FormCurrentStep >= s then
                                                                    (fun _ ->
                                                                        props.FormProgressStepAction
                                                                            s
                                                                    )
                                                                else
                                                                    ignore
                                                            )
                                                            prop.classes
                                                                [
                                                                    "steps-marker"
                                                                    classes.stepMarker
                                                                ]
                                                        ]
                                                ]
                                        ]
                            ]
                    ]
        ]
