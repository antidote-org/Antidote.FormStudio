module Antidote.FormStudio.UI.Components.Switch

open Feliz
open Fable.Core.JsInterop

let internal classes: CssModules.UI.Components.Switch =
    import "default" "./Switch.module.scss"

type SwitchProps =
    {|
        IsEnabled: bool
        OnChange: bool -> unit
    |}

[<ReactComponent>]
let Switch (props: SwitchProps) =
    Html.label [
        prop.style [
            style.display.flex
            style.alignItems.center
        ]
        prop.className classes.switch
        prop.children [
            Html.input [
                prop.style [
                    style.display.flex
                    style.flexDirection.row
                    style.alignItems.center
                ]
                prop.isChecked props.IsEnabled
                prop.onChange props.OnChange
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
