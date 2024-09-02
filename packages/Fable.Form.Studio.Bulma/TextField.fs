namespace Fable.Form.Studio.Bulma.Fields

open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Studio
open Fable.Form.Studio.Bulma

module TextField =

    type Attributes =
        {
            /// <summary>
            /// Label to display
            /// </summary>
            Label: string
            /// <summary>
            /// Placeholder to display when the field is empty
            /// </summary>
            Placeholder: string
            /// <summary>
            /// A list of HTML attributes to add to the generated field
            /// </summary>
            HtmlAttributes: IReactProperty list
        }

    [<NoComparison; NoEquality>]
    type TextFieldConfig<'Msg> =
        {
            Dispatch: Dispatch<'Msg>
            OnChange: string -> 'Msg
            OnBlur: 'Msg option
            Disabled: bool
            Value: string
            Error: Error.Error option
            ShowError: bool
            Attributes: Attributes
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

    type InnerField<'Values> = Field.Field<Attributes, string, 'Values>

    let form<'Values, 'Attributes, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
            -> Base.FieldConfig<Attributes, string, 'Values, 'Output>
            -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field System.String.IsNullOrEmpty

    type Field<'Values, 'Field, 'Output, 'Value, 'Attributes, 'HtmlAttribute>
        (inputType : TextType, innerField : InnerField<'Values>) =

        member _.GetRenderConfig
            (onBlur: OnBlur<'Msg>)
            (dispatch: Dispatch<'Msg>)
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (filledField: FilledField<'Values, 'Attributes>)
            : TextFieldConfig<'Msg> =

            {
                Dispatch = dispatch
                OnChange = innerField.Update >> fieldConfig.OnChange
                OnBlur = onBlur innerField.Attributes.Label
                Disabled = filledField.IsDisabled || fieldConfig.Disabled
                Value = innerField.Value
                Error = filledField.Error
                ShowError = fieldConfig.ShowError innerField.Attributes.Label
                Attributes = innerField.Attributes
            }

        interface IField<'Values, 'Attributes> with

            member _.MapFieldValues
                (update: 'Values -> 'NewValues -> 'NewValues)
                (values: 'NewValues)
                : IField<'NewValues, 'Attributes> =

                let newUpdate oldValues = update oldValues values

                Field(inputType, Field.mapValues newUpdate innerField)

            member this.RenderField
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)=

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

                inputFunc [
                    prop.onChange (config.OnChange >> config.Dispatch)

                    match config.OnBlur with
                    | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                    | None -> ()

                    prop.disabled config.Disabled
                    prop.value config.Value
                    prop.placeholder config.Attributes.Placeholder
                    if config.ShowError && config.Error.IsSome then
                        color.isDanger

                    yield! config.Attributes.HtmlAttributes
                ]
                |> Internal.View.withLabelAndError config.Attributes.Label config.ShowError config.Error
