module Antidote.FormStudio.UI.Components.DropFieldsContainer

open Feliz
open Fable.Core.JsInterop
open Browser
open Antidote.FormStudio.Helper

let private classes: CssModules.UI.Components.DropFieldsContainer =
    import "default" "./DropFieldsContainer.module.scss"

type DropFieldsContainerProps =
    {|
        OnDrop: DragSource option -> unit
        Children: ReactElement list
    |}

[<ReactComponent>]
let DropFieldsContainer (props: DropFieldsContainerProps) =
    Html.div [
        prop.className classes.``drop-fields-container``
        prop.children props.Children
        prop.onDragOver (fun (event: Types.DragEvent) -> event.preventDefault ())
        prop.onDrop (fun event ->
            event.preventDefault ()

            event.dataTransfer.getData ("text/plain")
            |> tryGetDragSourceFromData
            |> props.OnDrop
        )
    ]
