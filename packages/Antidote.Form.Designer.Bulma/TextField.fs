namespace Antidote.Form.Designer.Bulma.Fields

open System
open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Antidote.Form.Designer
open Antidote.Form.Designer.Bulma

module TextField =

    [<AllowNullLiteral>]
    type Attributes = class end

    type Value =
        {
            Id: Guid
            Label: string
            Placeholder: string
        }

    [<NoComparison; NoEquality>]
    type TextFieldConfig<'Msg> =
        {
            Dispatch: Dispatch<'Msg>
            OnChange: Value -> 'Msg
            OnBlur: 'Msg option
            Disabled: bool
            Value: Value
            Error: Error.Error option
            ShowError: bool
        }

    /// <summary>
    /// Represents the type of a TextField
    /// </summary>
    type TextType =
        | TextColor
        | TextDate
        | TextDateTimeLocal
        | TextEmail
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextMonth
        | TextNumber
        | TextPassword
        // TODO:
        // | TextRange
        | TextSearch
        | TextTel
        // Match for input="text"
        | TextRaw
        | TextTime
        // Not supported yet because there are not cross browser support Firefox doesn't support it for example
        // and there is no polyfill for it
        // | TextWeek
        | TextArea

    type InnerField<'Values> = Field.Field<Attributes, Value, 'Values>

    let form<'Values, 'Attributes, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, Value, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun value -> String.IsNullOrEmpty value.Label)

    type Field<'Values, 'Field, 'Output, 'Value, 'Attributes, 'HtmlAttribute>
        (inputType: TextType, innerField: InnerField<'Values>)
        =

        member _.GetRenderConfig
            (onBlur: OnBlur<'Msg>)
            (dispatch: Dispatch<'Msg>)
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (filledField: FilledField<'Values, 'Attributes>)
            : TextFieldConfig<'Msg>
            =

            {
                Dispatch = dispatch
                OnChange = innerField.Update >> fieldConfig.OnChange
                OnBlur = innerField.Value.Id.ToString() |> onBlur
                Disabled = filledField.IsDisabled || fieldConfig.Disabled
                Value = innerField.Value
                Error = filledField.Error
                ShowError = innerField.Value.Id.ToString() |> fieldConfig.ShowError
            }

        interface IField<'Values, 'Attributes> with

            member _.Id = innerField.Value.Id

            member this.RenderPreview
                (activeFieldId: ActiveFieldId)
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                let inputFunc =
                    match inputType with
                    | TextRaw -> Bulma.input.text

                    | TextPassword -> Bulma.input.password

                    | TextEmail -> Bulma.input.email

                    | TextColor -> Bulma.input.color

                    | TextDate -> Bulma.input.date

                    | TextDateTimeLocal -> Bulma.input.datetimeLocal

                    | TextNumber -> Bulma.input.number

                    | TextSearch -> Bulma.input.search

                    | TextTel -> Bulma.input.tel

                    | TextTime -> Bulma.input.time

                    | TextArea -> Bulma.textarea

                let label =
                    if String.IsNullOrEmpty config.Value.Label then
                        "Text field"
                    else
                        config.Value.Label

                let previewContent =
                    inputFunc [
                        prop.readOnly true
                        prop.placeholder innerField.Value.Placeholder
                    ]
                    |> Internal.View.withLabelAndError label config.ShowError config.Error

                Internal.View.PreviewContainer activeFieldId config.Value.Id previewContent

            member this.RenderPropertiesEditor
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                : ReactElement
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                let updateValue = config.OnChange >> config.Dispatch

                React.fragment [
                    Internal.View.PropertyEditor.inputText
                        config.ShowError
                        config.Error
                        (fun newLabel ->
                            { config.Value with
                                Label = newLabel
                            }
                            |> updateValue
                        )
                        config.Value.Label
                        "Label"

                    Internal.View.PropertyEditor.inputText
                        config.ShowError
                        config.Error
                        (fun newText ->
                            { config.Value with
                                Placeholder = newText
                            }
                            |> updateValue
                        )
                        config.Value.Placeholder
                        "Placeholder"
                ]
