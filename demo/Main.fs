module Demo.Main

open Fable.Core.JsInterop
open Antidote.FormStudio.Types
open Antidote.FormStudio.DynamicFormDesigner
open Feliz
open Feliz.Bulma
open Browser
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Fable.Form.Simple.Bulma.Fields
open Fable.Form.Simple.Fields.Html

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

type RadioItem =
    {
        Key: string
        Value: string
    }

[<RequireQualifiedAccess>]
type FieldType =
    | Text of TextInfo
    | Checkbox of CheckboxInfo
    | Radio of RadioItem list

let fieldTypePropertyEditor (fieldType : FieldType) : Form<FieldType, FieldType> =
    match fieldType with
    | FieldType.Text _
    | FieldType.Checkbox _ -> Form.succeed fieldType
    | FieldType.Radio items ->
        let radioItemField (context: FormList.ElementContext) =
            let keyField: Form<RadioItem,string> =
                Form.textField
                    {
                        Parser = Ok
                        Value = fun value -> value.Key
                        Update = fun value values -> { values with Key = value }
                        Error = fun _ -> None
                        Attributes =
                            TextField.create $"{context.Index}-key"
                            |> TextField.withLabel "Key"
                    }

            let valueField =
                Form.textField
                    {
                        Parser = Ok
                        Value = fun value -> value.Value
                        Update = fun value values -> { values with Value = value }
                        Error = fun _ -> None
                        Attributes =
                            TextField.create $"{context.Index}-value"
                            |> TextField.withLabel "Value"
                    }

            let onSubmit key value : RadioItem =
                { Key = key; Value = value }

            Form.succeed onSubmit
            |> Form.append keyField
            |> Form.append valueField
            |> Form.group

        let onSubmit result : FieldType =
            FieldType.Radio result

        Form.succeed onSubmit
        |> Form.append (
            Form.list
                {
                    Default =
                        {
                            Key = ""
                            Value = ""
                        }
                    Value =
                        fun values ->
                            match values with
                            | FieldType.Radio items -> items
                            | _ -> failwith "Invalid field type"
                    Update =
                        fun newValue values ->
                            match values with
                            | FieldType.Radio items -> FieldType.Radio newValue
                            | _ -> failwith "Invalid field type"
                    Attributes =
                        FormList.create "items-list"
                        |> FormList.withLabel "Options"
                        |> FormList.withAdd "Add item"
                        |> FormList.withDelete "Delete item"
                }
                radioItemField
        )

let private defaultDesignerFields =
    [
        { new IDesignerField<FieldType> with
            member _.Icon = "fas fa-font"
            member _.Key = "Text"

            member _.FieldType =
                FieldType.Text
                    {
                        Value = Some "dwdwdw"
                    }

            member _.RenderDesignerPreview props =
                match props.FormField.FieldType with
                | FieldType.Text info ->
                    Bulma.input.text [
                        prop.readOnly true
                        info.Value
                        |> Option.defaultValue ""
                        |> prop.value
                    ]
                | _ ->
                    Html.div [
                        prop.text "Invalid field type"
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

        { new IDesignerField<FieldType> with
            member _.Icon = "fas fa-list"
            member _.Key = "Option"

            member _.FieldType =
                FieldType.Radio
                    [
                        { Key = "option-1"; Value = "Option 1" }
                        { Key = "option-2"; Value = "Option 2" }
                    ]

            member _.RenderDesignerPreview props =
                match props.FormField.FieldType with
                | FieldType.Radio items ->
                    items
                    |> List.map (fun item ->
                        Bulma.input.labels.radio [
                            Bulma.input.radio [
                                prop.disabled true
                            ]

                            Html.text item.Value
                        ]
                    )
                    |> Bulma.control.div
                | _ ->
                    Html.div [
                        prop.text "Invalid field type"
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

    let renderFieldTypeFromAntidote
        (readOnly: bool)
        (dependencyMatch:
            DependsOn option
                -> Form<DynamicStepValues, string>
                -> Form<'a, string>)
        (specField: FormField<FieldType>)
        =

        let optionalMatch isOptional (field: Form<DynamicStepValues, string>) =
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
                            TextField.create specField.Label // TODO: Use a real unique field id?
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
                            CheckboxField.create specField.Label // TODO: Use a real unique field id?
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
                fieldTypePropertyEditor

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
