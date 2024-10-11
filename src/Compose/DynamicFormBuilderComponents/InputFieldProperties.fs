module Antidote.React.Components.FormWizard.DynamicFormBuilderComponents.InputFieldProperties

open Fable.Form.Antidote

open Feliz
open Feliz.Bulma
open Elmish
open Feliz.UseElmish
// open Antidote.Core.V2.Types
open Antidote.FormStudio.i18n.Util
open System
// open Antidote.Core.V2.Domain.Form
// open Antidote.Core.V2.Types
open Browser
open Fable.Form.Antidote
open type Feliz.Toastify.Exports
open Antidote.React.Components.FormWizard
open Antidote.Core.V2.Utils.JS
open Feliz.ReactRouterDom
open Fable.Core.JsInterop
open Antidote.Core.V2.Communications
open Antidote.Core.V2.Domain
open Fable.Form.Antidote.Form.View
// open Antidote.Core.V2.Utils
open System.Text
open Thoth.Json
open Feliz.Iconify
open type Offline.Exports
open Glutinum.IconifyIcons.Mdi

open Antidote.Core.FormProcessor.Types
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.Core.FormProcessor.Helper
open Antidote.Core.FormProcessor
open Antidote.Core.FormProcessor.Spec.v2_0_1

let private classes: CssModules.FormWizard.Compose.DynamicFormBuilder =
    import "default" "../FormWizard/Compose/DynamicFormBuilder.module.scss"

type InputFieldPropertiesProps =
    {
        FormSpec: FormSpec
        ActiveField: FormField
        FormSpecChanged: FormSpec -> unit
    }

[<ReactComponent>]
let InputFieldProperties () =
    Bulma.panelBlock.div
        [
            Html.label
                [
                    prop.className "checkbox"
                    prop.children
                        [
                            Html.input
                                [
                                    prop.isChecked props.FormField.IsOptional
                                    prop.onChange (fun (e: bool) ->
                                        let newFormField = { formField with IsOptional = e }

                                        let newFormSpec =
                                            { props.FormSpec with
                                                Steps =
                                                    props.FormSpec.Steps
                                                    |> List.map (fun s ->
                                                        if
                                                            s.StepOrder = props.ActiveField.FormStepNumber
                                                        then
                                                            { s with
                                                                Fields =
                                                                    s.Fields
                                                                    |> List.map (fun f ->
                                                                        if
                                                                            f.FieldOrder = props.ActiveField.FormFieldNumber
                                                                        then
                                                                            newFormField
                                                                        else
                                                                            f
                                                                    )
                                                            }
                                                        else
                                                            s
                                                    )
                                            }

                                        props.FormSpecChanged newFormSpec
                                    )
                                    prop.type' "checkbox"
                                ]
                            Html.text " Optional"
                        ]
                ]
        ]

    Bulma.panelBlock.div
        [
            Html.label
                [
                    prop.className "checkbox"
                    prop.children
                        [
                            Html.input
                                [
                                    prop.isChecked formField.IsDeprecated
                                    prop.onChange (fun (e: bool) ->
                                        let newFormField = { formField with IsDeprecated = e }

                                        let newFormSpec =
                                            { props.FormSpec with
                                                Steps =
                                                    props.FormSpec.Steps
                                                    |> List.map (fun s ->
                                                        if
                                                            s.StepOrder = props.ActiveField.FormStepNumber
                                                        then
                                                            { s with
                                                                Fields =
                                                                    s.Fields
                                                                    |> List.map (fun f ->
                                                                        if
                                                                            f.FieldOrder = props.ActiveField.FormFieldNumber
                                                                        then
                                                                            newFormField
                                                                        else
                                                                            f
                                                                    )
                                                            }
                                                        else
                                                            s
                                                    )
                                            }

                                        props.FormSpecChanged newFormSpec
                                    )
                                    prop.type' "checkbox"
                                ]
                            Html.text " Deprecated"
                        ]
                ]
        ]
// match isDeprecatedPending with
// | false -> Html.none
// | true ->
//     Bulma.panelBlock.div [
//         Html.label [
//             prop.className "checkbox"
//             prop.children [
//                 Html.input [
//                     prop.isChecked true
//                     prop.onChange (fun (e:bool) ->
//                         setIsDeprecatedPending (not isDeprecatedPending)
//                         match formField

//                     )
//                     prop.type' "checkbox"
//                 ]
//                 Html.text " Deprecated"
//             ]
//         ]
//     ]
