module Antidote.React.Components.SignatureCanvas

open Feliz
open Feliz.Bulma
open Fable.Form.Antidote
open Feliz.SignatureCanvas
open type SignatureCanvas.Exports
open Fable.Core.JsInterop

let private classes: CssModules.ComponentFields.SignatureCanvas =
    import "default" "./SignatureCanvas.module.scss"

// [<ReactComponent>]
// let SignatureField (props : Field.ReactComponentField.ReactComponentFieldProps) =
//     let canvasRef = React.useRef null
//     Html.div [
//         prop.className classes.signatureCanvas
//         prop.children [
//             signatureCanvas [
//                 signatureCanvas.maxWidth 300
//                 signatureCanvas.clearOnResize false
//                 signatureCanvas.penColor "#05435C"
//                 signatureCanvas.ref canvasRef
//                 signatureCanvas.canvasProps [
//                     prop.className classes.signatureCanvas
//                     prop.width (unbox<int> "inherit") // Missing property in Feliz
//                     prop.height (unbox<int> "inherit") // Missing property in Feliz
//                 ]
//                 signatureCanvas.onEnd (fun _ ->
//                     props.OnChange (canvasRef.current.toDataURL("image/png"))
//                 )
//             ]
//         ]
//     ]

[<ReactComponent>]
let SignatureField (key: string) (props: Field.ReactComponentField.ReactComponentFieldProps) =
    let signature, setSignature = React.useState ""
    let canvasRef = React.useRef null

    React.useEffectOnce (fun _ ->
        match canvasRef.current, System.String.IsNullOrWhiteSpace(props.Value) with
        | null, _ -> ()
        | _, false -> canvasRef.current?fromDataURL (props.Value)
        | _ -> ()
    )

    Html.div [
        prop.style [
            style.textAlign.center
        ]
        prop.children [
            Html.div [
                prop.className classes.``signature-field``
                prop.children [
                    if props.Disabled then
                        if props.Value <> "" then
                            Html.img [
                                prop.src props.Value
                            ]
                        else
                            Html.none
                    else
                        signatureCanvas [
                            signatureCanvas.maxWidth 400
                            signatureCanvas.clearOnResize false
                            signatureCanvas.penColor "rgb(18 111 231)"
                            signatureCanvas.ref canvasRef
                            signatureCanvas.canvasProps [
                                prop.key key
                                prop.className classes.``signature-canvas``
                                prop.width (unbox<int> "inherit") // Missing property in Feliz
                                prop.height (unbox<int> "inherit") // Missing property in Feliz
                            ]
                            // signatureCanvas.onBegin (fun a ->
                            //     canvasRef.current?fromDataURL("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASwAAACWCAYAAABkW7XSAAAAAXNSR0IArs4c6QAABYJJREFUeF7t2MFplGEYBOB/RW3LiCkjxSS9pIyIqUvFlT3k4EEdAstk4PEi6Gt2fOZjiJ4OPwgQIDAicBrJKSYBAgQOg+URECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqoSlAABg+UNECAwI2CwZqr6M+j727ubd8dx8+s4ni+/8/Pp8fnya5efR/9KYhP4r4DB+i/R2zz48OXu/Ldkp9Px8P3p8f5tJpeKwOsFDNbr7Wp/8vKd1Ol8fPtHgOcfXx8/1wL6YAJXEjBYV4K99pd9+Sfh5XPO5+PTcRw3L5/pO6xr6/v6LQGD1ZK/wud+vL27v4zX+XQ8+L+sKwD7knUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBdwGDVKxCAAIFUwGClUu4IEKgLGKx6BQIQIJAKGKxUyh0BAnUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBdwGDVKxCAAIFUwGClUu4IEKgLGKx6BQIQIJAKGKxUyh0BAnUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBdwGDVKxCAAIFUwGClUu4IEKgLGKx6BQIQIJAKGKxUyh0BAnUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBdwGDVKxCAAIFUwGClUu4IEKgLGKx6BQIQIJAKGKxUyh0BAnUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBdwGDVKxCAAIFUwGClUu4IEKgLGKx6BQIQIJAKGKxUyh0BAnUBg1WvQAACBFIBg5VKuSNAoC5gsOoVCECAQCpgsFIpdwQI1AUMVr0CAQgQSAUMVirljgCBuoDBqlcgAAECqYDBSqXcESBQFzBY9QoEIEAgFTBYqZQ7AgTqAgarXoEABAikAgYrlXJHgEBd4DfzqSSXRcPfOgAAAABJRU5ErkJggg==")
                            // )
                            signatureCanvas.onEnd (fun a ->
                                // Browser.Dom.console.log (canvasRef.current)
                                Browser.Dom.window?signature <- canvasRef.current
                                setSignature (canvasRef.current.toDataURL ())
                                props.OnChange(canvasRef.current.toDataURL ())
                            )
                        ]
                ]
            ]
            Bulma.label [
                prop.style [
                    style.fontSize 10
                    style.color "#adadad"
                ]
                prop.text "I hereby confirm that the information provided is correct"

            ]
            Bulma.label [
                button.isSmall
                prop.style [
                    style.cursor.pointer
                    style.color "#adadad"
                ]
                prop.children [
                    // Html.span [
                    //     prop.className "icon" // is-small"
                    //     prop.children [
                    //         Html.i [ prop.className "fas fa-times" ]
                    //     ]
                    // ]
                    Html.span [
                        prop.style [
                            style.borderBottom (1, borderStyle.solid, "#F191AC")

                        ]
                        prop.text "reset signature"
                    ]
                ]
                prop.onClick (fun _ ->
                    canvasRef.current?clear ()
                    props.OnChange ""
                )
            ]

        // Bulma.button.a [
        //     Bulma.icon [ Html.i [ prop.className "fas fa-times" ] ]
        //     Html.span [ prop.text "CLEAR" ]
        // ]
        ]
    ]

//{"Id":"5e4ecf67-830a-46ee-b0f4-414a430126da","Code":"PLAY","Title":"Playground Form","Abstract":"This is a playground form.","Version":"1.0.0","Steps":[{"StepOrder":1,"StepLabel":"Playground Step 1","Fields":[["SingleChoice",{"FieldOrder":1,"Label":"Have you ever felt you needed to cut down on your drinking?","Options":[{"Description":"Yes","Value":"1"},{"Description":"No","Value":"0"}],"IsOptional":false}],["Message",{"FieldOrder":2,"Label":"This is an information message.","MessageType":"Info","Message":"This is a message.","Footer":"999.999.9999","IsOptional":false}],["Signature",{"FieldOrder":3,"Label":"Signature","IsOptional":false}]]}]}
