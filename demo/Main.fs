module Demo.Main

open Feliz
open Browser
open Demo.App
open Fable.Core.JsInterop

importSideEffects "./index.scss"

let root = ReactDOM.createRoot (document.getElementById "root")

root.render (App())
