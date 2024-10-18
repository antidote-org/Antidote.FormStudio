module Demo.Main

open Fable.Core.JsInterop
open Antidote.FormStudio.Types
open Antidote.FormStudio.DynamicFormDesigner
open Feliz
open Feliz.Bulma
open Browser

importSideEffects "../node_modules/bulma/css/bulma.min.css"

type TextInfo =
    {
        Value: string option
    }

type CheckboxInfo =
    {
        DefaultValue: bool option
        Selection: bool option
    }

[<RequireQualifiedAccess>]
type FieldType =
    | Text of TextInfo
    | Checkbox of CheckboxInfo

let private defaultDesignerFields =
    [
        { new IDesignerField<FieldType> with
            member _.Icon = "fas fa-font"
            member _.Key = "Text"

            member _.FieldType =
                FieldType.Text
                    {
                        Value = None
                    }

            member _.RenderDesignerPreview props =
                Bulma.input.text [
                    prop.readOnly true
                ]
        }

        { new IDesignerField<FieldType> with
            member _.Icon = "fas fa-check-square"
            member _.Key = "Checkbox"

            member _.FieldType =
                FieldType.Checkbox
                    {
                        DefaultValue = None
                        Selection = None
                    }

            member _.RenderDesignerPreview props =
                Bulma.control.div [
                    Bulma.input.labels.checkbox [

                        Bulma.input.checkbox [
                            prop.disabled true
                        ]

                        Html.text " Checkbox"
                    ]
                ]
        }
    ]

let private root = ReactDOM.createRoot (document.getElementById "root")

// TODO: More work is needed to remove tied integration with Antidote specific types
module Helpers =

    let updateSingleFunc
        formatter
        (specField: FormField<FieldType>)
        (newValue: string)
        (values: DynamicStepValues)
        : DynamicStepValues
        =
        let newFieldDetails: FieldDetails =
            {
                FieldOrder = specField.FieldOrder
                Key = FieldKey specField.FieldKey
                // Value = Single (formatter newValue)
                FieldValue =
                    Single
                        {
                            // FieldType = specField.FieldType
                            FieldKey = specField.FieldKey
                            Value = formatter newValue
                            Description = newValue
                        }
                Label = specField.Label
                Options = []
            }

        match values.Keys |> Seq.tryFind (fun k -> k = newFieldDetails.Key) with
        | None -> values.Add(newFieldDetails.Key, newFieldDetails)
        | Some key -> values.Remove key |> (fun f -> values.Add(key, newFieldDetails))

    let readValue (field: FormField<FieldType>) (values: DynamicStepValues) : string =
        //read the value from the values map
        match values.Keys |> Seq.tryFind (fun k -> k = (FieldKey field.FieldKey)) with
        | None -> ""
        | Some key ->
            let fv = values.Item key

            match fv.FieldValue with
            | Single v -> v.Value
            | _ -> "" //Should never happen

module FormSpecRender =

    open Fable.Form.Antidote

    let renderFieldTypeFromAntidote
        (readOnly: bool)
        (dependencyMatch:
            DependsOn option
                -> Form.Form<DynamicStepValues, string, IReactProperty>
                -> Form.Form<'a, string, 'b>)
        (specField: FormField<FieldType>)
        =

        let optionalMatch isOptional (field: Form.Form<DynamicStepValues, string, IReactProperty>) =
            if isOptional then
                field
                |> Form.optional
                |> Form.andThen (
                    function
                    | Some v -> Form.succeed v
                    | None -> Form.succeed ""
                )
            else
                field

        let emptyField = Form.succeed ""

        match specField.FieldType with
        | FieldType.Text info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Form.textField
                    {
                        Parser = Ok
                        Value = fun values -> Helpers.readValue specField values
                        Update = Helpers.updateSingleFunc id specField
                        Error = fun _ -> None
                        Attributes =
                            {
                                Label = specField.Label
                                Placeholder = ""
                                HtmlAttributes = []
                            }
                    }
                |> Form.disableIf readOnly
                |> optionalMatch specField.IsOptional
                |> dependencyMatch specField.DependsOn

        | FieldType.Checkbox info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Form.checkboxField
                    {
                        Parser = string >> Ok
                        Value = fun value -> snd (bool.TryParse(Helpers.readValue specField value))
                        Update =
                            fun value values ->
                                Helpers.updateSingleFunc id specField (string value) values

                        Error = fun _ -> None
                        Attributes =
                            {
                                Text = specField.Label
                            }
                    }
                |> Form.disableIf readOnly

                |> optionalMatch specField.IsOptional
                |> dependencyMatch specField.DependsOn

[<ReactComponent>]
let App () =
    let formSpec, setFormSpec =
        React.useState Antidote.FormStudio.Helper.defaultFormSpec

    Bulma.section [
        Bulma.container [
            DynamicFormDesigner
                formSpec
                setFormSpec
                defaultDesignerFields
                FormSpecRender.renderFieldTypeFromAntidote

            Bulma.field.p [
                field.isGrouped
                field.isGroupedRight
                prop.children [
                    Bulma.control.div [
                        Bulma.button.button [
                            prop.text "Reset"
                            prop.onClick (fun _ ->
                                setFormSpec Antidote.FormStudio.Helper.defaultFormSpec
                            )
                        ]
                    ]
                    Bulma.control.div [
                        Bulma.button.button [
                            prop.text "Preview"
                            prop.onClick (fun _ ->
                                window.alert "TODO: See console for log"
                                console.log formSpec
                            )
                        ]
                    ]
                ]
            ]
        ]
    ]

root.render (App())
