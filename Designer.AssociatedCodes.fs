module Antidote.React.FormDesigner.Designer.AssociatedCodes

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Spec.Types

let private classes : CssModules.DynamicFormDesigner = import "default" "./DynamicFormDesigner.module.scss"


type AssociatedCodesProps = {|
    FormSpec: FormSpec
    OnChange: FormSpec -> unit
|}

[<ReactComponent>]
let AssociatedCodes(props:AssociatedCodesProps) =
    let newAssociatedCode, setNewAssociatedCode = React.useState ""

    // let exists code = props.FormSpec.AssociatedCodes |> List.contains code

    React.fragment [
        Bulma.field.div [
            field.isHorizontal
            field.hasAddons
            prop.children [
                Bulma.fieldLabel [
                    fieldLabel.isNormal
                    prop.children [
                        Bulma.label "Codes"
                    ]
                ]
                Bulma.fieldBody [
                    Bulma.field.div [
                        field.isNarrow
                        prop.children [
                            Bulma.input.text [
                                input.isStatic
                                prop.value newAssociatedCode
                                prop.placeholder "Enter Code"
                                prop.onChange setNewAssociatedCode
                                // prop.onKeyPress (fun e ->
                                //     if e.code = "13" then
                                //         let newAssociatedCodes =
                                //         // if isChecked then
                                //         // props.FormSpec.AssociatedCodes
                                //         // |> List.filter (fun x -> x <> associatedCode)
                                //         // else
                                //             newAssociatedCode :: props.FormSpec.AssociatedCodes
                                //         props.OnChange {
                                //             props.FormSpec with
                                //                 AssociatedCodes = newAssociatedCodes
                                //         }
                                //         setNewAssociatedCode ""
                                //     else
                                //         setNewAssociatedCode e.code

                                // )
                            ]
                        ]
                    ]
                    Bulma.button.a [
                        // button.isPrimary
                        button.isSmall

                        prop.text "Add"
                        prop.onClick (fun _ ->
                            // Compute the new state
                            let newAssociatedCodes =
                                if props.FormSpec.AssociatedCodes |> List.contains newAssociatedCode
                                then
                                    props.FormSpec.AssociatedCodes
                                else
                                    newAssociatedCode :: props.FormSpec.AssociatedCodes

                            // Save the new state
                            props.OnChange {
                                props.FormSpec with
                                    AssociatedCodes = newAssociatedCodes
                            }
                            setNewAssociatedCode ""
                        )
                    ]

                ]
            ]
        ]
        Bulma.field.div [
            field.isHorizontal
            prop.children [
                Bulma.fieldLabel [
                    fieldLabel.isNormal
                    prop.children [
                        Bulma.label ""
                    ]
                ]
                Bulma.fieldBody [
                    Bulma.field.div [
                        prop.children [
                            let toOption (associatedCode:string) =
                                let exists =
                                    List.contains associatedCode props.FormSpec.AssociatedCodes
                                Bulma.field.div [
                                    prop.children [
                                        Bulma.tag [
                                            tag.isRounded
                                            prop.style [
                                                style.cursor.pointer
                                            ]
                                            prop.key associatedCode
                                            color.hasBackgroundInfo
                                            color.hasTextWhite
                                            // if exists then
                                            //     color.isAntidoteBluePrimary

                                            prop.children [
                                                Html.text associatedCode
                                                Html.button [
                                                    prop.classes [
                                                        "delete is-small"
                                                    ]

                                                    prop.onClick (fun _ ->
                                                        // Compute the new state
                                                        let newAssociatedCodes =
                                                            props.FormSpec.AssociatedCodes
                                                            |> List.filter (fun x -> x <> associatedCode)

                                                        // Save the new state
                                                        props.OnChange {
                                                            props.FormSpec with
                                                                AssociatedCodes = newAssociatedCodes
                                                        }
                                                    )
                                                ]
                                            ]

                                        ]
                                    ]
                                ]


                            Bulma.tags [
                                yield! props.FormSpec.AssociatedCodes
                                |> List.sort
                                |> List.map toOption
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
