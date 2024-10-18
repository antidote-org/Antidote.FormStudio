module Antidote.FormStudio.GlobalCSS

open Fable.Core.JsInterop

let internal classes: CssModules.GlobalCSS =
    import "default" "./GlobalCSS.module.scss"
