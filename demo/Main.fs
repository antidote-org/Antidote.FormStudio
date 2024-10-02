module Demo.Main

open Fable.Core.JsInterop
open Antidote.FormStudio.DynamicFormDesigner
open Feliz
open Browser

importSideEffects "../node_modules/bulma/css/bulma.min.css"

let root = ReactDOM.createRoot(document.getElementById "root")
root.render(
    DynamicFormDesigner StandAlone
)
