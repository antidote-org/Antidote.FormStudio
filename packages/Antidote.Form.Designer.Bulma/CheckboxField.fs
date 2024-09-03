namespace Antidote.Form.Designer.Bulma.Fields

open System
open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Antidote.Form.Designer
open Antidote.Form.Designer.Bulma

module CheckboxField =

    [<AllowNullLiteral>]
    type Attributes = class end

    type Value =
        {
            Id: Guid
            DefaultValue: bool
            Text: string
        }

    [<NoComparison; NoEquality>]
    type CheckboxFieldConfig<'Msg> =
        {
            Dispatch: Dispatch<'Msg>
            OnChange: Value -> 'Msg
            OnBlur: 'Msg option
            Disabled: bool
            Value: Value
            Error: Error.Error option
            ShowError: bool
        }

    type InnerField<'Values> = Field.Field<Attributes, Value, 'Values>

    let form<'Values, 'Field, 'Output>
        : ((InnerField<'Values> -> 'Field)
              -> Base.FieldConfig<Attributes, Value, 'Values, 'Output>
              -> Base.Form<'Values, 'Output, 'Field>) =
        Base.field (fun value -> String.IsNullOrEmpty value.Text)

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

                let previewContent =
                    Bulma.control.div [
                        Bulma.input.labels.checkbox [
                            prop.children [
                                Bulma.input.checkbox [
                                    // We are in preview mode
                                    prop.readOnly true
                                    prop.isChecked innerField.Value.DefaultValue
                                ]

                                Html.text innerField.Value.Text
                            ]
                        ]
                    ]
                    |> List.singleton
                    |> Internal.View.wrapInFieldContainer

                Internal.View.PreviewContainer activeFieldId config.Value.Id previewContent

            member this.RenderPropertiesEditor
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                : ReactElement
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                React.fragment [
                    Internal.View.PropertyEditor.inputText
                        config.ShowError
                        config.Error
                        (fun newText ->
                            { config.Value with
                                Text = newText
                            }
                            |> config.OnChange
                            |> config.Dispatch
                        )
                        config.Value.Text
                        "Text"

                    Internal.View.PropertyEditor.checkbox
                        config.ShowError
                        config.Error
                        (fun newValue ->
                            { config.Value with
                                DefaultValue = newValue
                            }
                            |> config.OnChange
                            |> config.Dispatch
                        )
                        config.Value.DefaultValue
                        "Default Value"
                ]
