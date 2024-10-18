module Antidote.FormStudio.UI.Components.ButtonMinimal

open Feliz
open Fable.Core.JsInterop
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.FormStudio.Types

let private classes: CssModules.UI.Components.ButtonMinimal =
    import "default" "./ButtonMinimal.module.scss"

type private ButtonMinimalProps =
    {|
        Text: string
        OnClick: unit -> unit
    |}

[<ReactComponent>]
let ButtonMinimal (props: ButtonMinimalProps) =
    Html.button [
        prop.className classes.``button-minimal``
        prop.text props.Text
        prop.onClick (fun _ -> props.OnClick())
    ]
