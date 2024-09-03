namespace Antidote.Form.Designer.Bulma.Fields

open System
open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Antidote.Form.Designer
open Antidote.Form.Designer.Bulma

module CheckboxField =

    type Attributes =
        {
            Text: string
        }

    type Value =
        {
            Id: Guid
            IsFocused: bool
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
                OnBlur = onBlur innerField.Attributes.Text
                Disabled = filledField.IsDisabled || fieldConfig.Disabled
                Value = innerField.Value
                Error = filledField.Error
                ShowError = fieldConfig.ShowError innerField.Attributes.Text
            }

        interface IField<'Values, 'Attributes> with

            member this.Id = innerField.Value.Id

            member this.RenderPreview
                (activeFieldId: Guid)
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                let previewContent =
                    Bulma.control.div
                        [
                            Bulma.input.labels.checkbox
                                [
                                    prop.children
                                        [
                                            Bulma.input.checkbox
                                                [
                                                    // We are in preview mode
                                                    prop.disabled true
                                                    prop.isChecked innerField.Value.DefaultValue
                                                ]

                                            Html.text innerField.Value.Text
                                        ]

                                    prop.onClick (fun _ ->
                                        printfn "Clicked!"

                                        { config.Value with
                                            Text = config.Value.Text + "!"
                                        }
                                        |> config.OnChange
                                        |> config.Dispatch
                                    )
                                ]
                        ]
                    |> List.singleton
                    |> Internal.View.wrapInFieldContainer

                let updateFocus (isFocused: bool) =
                    { config.Value with
                        IsFocused = isFocused
                    }
                    |> config.OnChange
                    |> config.Dispatch

                Internal.View.PreviewContainer
                    activeFieldId
                    config.Value.Id
                    updateFocus
                    previewContent

            member this.RenderPropertiesEditor
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                : ReactElement
                =

                let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

                Html.div
                    [
                        prop.children
                            [
                                Bulma.input.text
                                    [
                                        prop.onChange (fun (newText: string) ->
                                            { config.Value with
                                                Text = newText
                                            }
                                            |> config.OnChange
                                            |> config.Dispatch
                                        )

                                        match config.OnBlur with
                                        | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                                        | None -> ()

                                        prop.disabled config.Disabled
                                        prop.value config.Value.Text
                                        // prop.placeholder config.Attributes.Placeholder
                                        if config.ShowError && config.Error.IsSome then
                                            color.isDanger

                                    // yield! config.Attributes.HtmlAttributes
                                    ]
                                |> Internal.View.withLabelAndError
                                    "Text"
                                    config.ShowError
                                    config.Error

                                Bulma.control.div
                                    [
                                        Bulma.input.labels.checkbox
                                            [
                                                prop.children
                                                    [
                                                        Bulma.input.checkbox
                                                            [
                                                                prop.onChange (fun (newValue: bool) ->
                                                                    { config.Value with
                                                                        DefaultValue = newValue
                                                                    }
                                                                    |> config.OnChange
                                                                    |> config.Dispatch
                                                                )
                                                                match config.OnBlur with
                                                                | Some onBlur ->
                                                                    prop.onBlur (fun _ ->
                                                                        dispatch onBlur
                                                                    )

                                                                | None -> ()
                                                                prop.disabled config.Disabled
                                                                prop.isChecked
                                                                    config.Value.DefaultValue
                                                            ]

                                                        Html.text "Default Value"
                                                    ]
                                            ]
                                    ]
                                |> List.singleton
                                |> Internal.View.wrapInFieldContainer

                            ]
                    ]
