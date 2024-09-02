namespace Fable.Form.Studio.Bulma.Fields

open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Fable.Form.Studio
open Fable.Form.Studio.Bulma

module CheckboxField =

    type Attributes =
        {
            Text: string
        }

    [<NoComparison; NoEquality>]
    type CheckboxFieldConfig<'Msg> =
        {
            Dispatch: Dispatch<'Msg>
            OnChange: bool -> 'Msg
            OnBlur: 'Msg option
            Disabled: bool
            Value: bool
            Error: Error.Error option
            ShowError: bool
            Attributes: Attributes
        }

    type InnerField<'Values> = Field.Field<Attributes, bool, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, bool, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun _ -> false)

    type Field<'Values, 'Field, 'Output, 'Value, 'Attributes>(innerField: InnerField<'Values>) =

        member _.GetRenderConfig
            (onBlur: OnBlur<'Msg>)
            (dispatch: Dispatch<'Msg>)
            (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
            (filledField: FilledField<'Values, 'Attributes>)
            : CheckboxFieldConfig<'Msg>
            =

            {
                Dispatch = dispatch
                OnChange = innerField.Update >> fieldConfig.OnChange
                OnBlur = onBlur innerField.Attributes.Text
                Disabled = filledField.IsDisabled || fieldConfig.Disabled
                Value = innerField.Value
                Error = filledField.Error
                ShowError = fieldConfig.ShowError innerField.Attributes.Text
                Attributes = innerField.Attributes
            }

        interface IField<'Values, 'Attributes> with

            member _.MapFieldValues
                (update: 'Values -> 'NewValues -> 'NewValues)
                (values: 'NewValues)
                : IField<'NewValues, 'Attributes>
                =
                let newUpdate oldValues = update oldValues values

                Field(Field.mapValues newUpdate innerField)

            member this.RenderField
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                Bulma.control.div
                    [
                        Bulma.input.labels.checkbox
                            [
                                prop.children
                                    [
                                        Bulma.input.checkbox
                                            [
                                                prop.onChange (config.OnChange >> config.Dispatch)
                                                match config.OnBlur with
                                                | Some onBlur ->
                                                    prop.onBlur (fun _ -> dispatch onBlur)

                                                | None -> ()
                                                prop.disabled config.Disabled
                                                prop.isChecked config.Value
                                            ]

                                        Html.text config.Attributes.Text
                                    ]
                            ]
                    ]
                |> List.singleton
                |> Internal.View.wrapInFieldContainer
