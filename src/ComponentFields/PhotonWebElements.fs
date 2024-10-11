module Antidote.React.Components.PhotonWebElements

open Browser
open Feliz
open Feliz.Bulma
open Fable
open Fable.Core.JsInterop
open Feliz.ReactRouterDom
open Antidote.Components.ReactLibrary.TS.ePrescribeBindings
open Fable.Form.Antidote
open Fable.Form.Antidote.Field.ReactComponentField
open Photon.Types

type EPrescribeOrderCreatedPayload = {| PhotonOrderId: string; PhotonOrder: PhotonOrder |}

[<ReactComponent>]
let ePrescribeComponent (props: ReactComponentFieldProps) =

    let navigate = useNavigate ()

    let ePrescribeRef = React.useRef None

    Html.div
        [
            // PageHeader.PageHeader
            //     {|
            //         Title = "ePrescribe"
            //         LeftAction = Some (fun t -> navigate.Invoke "/")
            //         HasSearch = true
            //         IsNative = false
            //         RightAction = None
            //     |}

            Html.div
                [
                    ePrescribe
                        {|
                            OnOrderCreated =
                                (fun o ->
                                    let payload: EPrescribeOrderCreatedPayload =
                                        {| PhotonOrderId = o.id; PhotonOrder = o |}

                                    props.OnChange(Thoth.Json.Encode.Auto.toString payload)
                                )
                        |}
                ]
        ]

let ePrescribeFieldField: Form.Form<string, _, IReactProperty> =
    Form.reactComponentField
        {
            Parser = Ok
            Value = (fun value -> value)
            Update = fun newValue values -> newValue
            Error = fun _ -> None
            Attributes = { Label = "ePrescribe Field"; Render = ePrescribeComponent }
        }
