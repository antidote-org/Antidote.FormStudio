module Demo.App

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Fable.Form.Studio
open Fable.Form.Studio.Bulma
open Fable.Form.Studio.View
open Antidote.FormStudio
open Fable.Core.JsInterop

let private classes: CssModules.App = import "default" "./App.module.scss"

[<ReactComponent>]
let FormDesigner (fields: Specification.DesignerField list) =
    let specification, setSpecification =
        React.useStateWithUpdater (
            {
                // Id : Guid
                Title = "Form"
                Fields = []
            }
            : Specification.Form
        )

    Html.div [
        prop.className classes.``form-designer-container``

        prop.children [
            Html.div [
                prop.className classes.``form-designer-container__component-list``
                prop.children [
                    for field in fields do
                        Bulma.button.button [
                            button.isFullWidth
                            button.isInverted
                            color.isInfo
                            color.hasTextBlack

                            prop.children [
                                Bulma.icon [
                                    Html.i [
                                        prop.className field.Icon
                                    ]
                                ]
                                Html.span field.Label
                            ]
                        ]
                ]
            ]

            Html.div [
                prop.className classes.``form-designer-container__designer``
            ]

            Html.div [
                prop.className classes.``form-designer-container__property-editor``
            ]
        ]
    ]

[<ReactComponent>]
let App () =
    Html.div [
        Bulma.section []
        Bulma.container [
            FormDesigner [
                Specification.inputTextDesignField
                Specification.checkboxDesignField
            ]
        ]

    ]
