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

    // open Fable.Form.Antidote

    let renderFieldTypeFromAntidote
        (readOnly: bool)
        (dependencyMatch:
            DependsOn option
                -> Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>
                -> Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>)
        (specField: FormField<FieldType>)
        =

        let optionalMatch isOptional (field: Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>) =
            if isOptional then
                field
                |> Fable.Form.Base.optional
                |> Fable.Form.Base.andThen (
                    function
                    | Some v -> Fable.Form.Simple.Bulma.Form.succeed v
                    | None -> Fable.Form.Simple.Bulma.Form.succeed ""
                )
            else
                field

        let emptyField = Fable.Form.Simple.Bulma.Form.succeed ""

        match specField.FieldType with
        | FieldType.Text info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Fable.Form.Simple.Bulma.Form.textField
                    {
                        Parser = Ok
                        Value = fun values -> Helpers.readValue specField values
                        Update = Helpers.updateSingleFunc id specField
                        Error = fun _ -> None
                        Attributes =
                            {
                                FieldId = specField.FieldKey
                                Label = specField.Label
                                Placeholder = None
                                AutoComplete = None
                                SpellCheck = Fable.Form.Simple.Fields.Html.TextField.SpellCheck.Default
                                AutoFocus = false
                            }
                    }
                // |> Fable.Form.Base.disable .disableIf readOnly
                |> optionalMatch specField.IsOptional
                |> dependencyMatch specField.DependsOn

        | FieldType.Checkbox info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Fable.Form.Simple.Bulma.Form.checkboxField
                    {
                        Parser = string >> Ok
                        Value = fun value -> snd (bool.TryParse(Helpers.readValue specField value))
                        Update =
                            fun value values ->
                                Helpers.updateSingleFunc id specField (string value) values

                        Error = fun _ -> None
                        Attributes =
                            {
                                FieldId = specField.FieldKey
                                Text = specField.Label
                            }
                    }
                // |> Fable.Form.Base.disableIf readOnly

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

        ]
    ]

root.render (App())
