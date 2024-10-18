module Demo.Main

open Fable.Core.JsInterop
open Antidote.FormStudio.UI.Components.ChoiceField
open Antidote.FormStudio.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.FormStudio.DynamicFormDesigner
open Feliz
open Feliz.Bulma
open Browser

importSideEffects "../node_modules/bulma/css/bulma.min.css"

let private defaultDesignerFields =
    [
        { new IDesignerField with
            member _.Icon = "fas fa-font"
            member _.Key = "Text"

            member _.FieldType =
                FieldType.Text
                    {
                        Value = None
                    }

            member _.RenderPreview props =
                Bulma.input.text [
                    prop.readOnly true
                ]
        }

        { new IDesignerField with
            member _.Icon = "fas fa-check-square"
            member _.Key = "Checkbox"

            member _.FieldType =
                FieldType.Checkbox
                    {
                        DefaultValue = None
                        Selection = None
                    }

            member _.RenderPreview props =
                Bulma.control.div [
                    Bulma.input.labels.checkbox [

                        Bulma.input.checkbox [
                            prop.disabled true
                        ]

                        Html.text " Checkbox"
                    ]
                ]
        }

        { new IDesignerField with
            member _.Icon = "fas fa-dot-circle"
            member _.Key = "Single Choice"

            member _.FieldType =
                (FieldType.SingleChoice
                    {
                        Options =
                            [
                                {
                                    Description = "Option 1"
                                    Value = "1"
                                    OptionKey = System.Guid.NewGuid().ToString()
                                }
                                {
                                    Description = "Option 2"
                                    Value = "2"
                                    OptionKey = System.Guid.NewGuid().ToString()
                                }
                            ]
                    })

            member _.RenderPreview props =
                ChoiceFieldComponent
                    {|
                        FormSpec = props.FormSpec
                        FormStep = props.FormStep
                        FormField = props.FormField
                        ActiveField = props.ActiveField
                        SetActiveField = props.SetActiveField
                        OnChange = props.OnChange
                    |}
        }
    ]

let private root = ReactDOM.createRoot (document.getElementById "root")

module FormSpecRender =

    open Fable.Form
    open Fable.Form.Antidote
    open Antidote.Core.FormProcessor.Values.v2_0_1
    open Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec
    open Antidote.FormStudio.i18n.Util

    let renderFieldTypeFromAntidote
        (readOnly: bool)
        (dependencyMatch:
            DependsOn option
                -> Form.Form<DynamicStepValues, string, IReactProperty>
                -> Form.Form<'a, string, 'b>)
        (specField: FormField)
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
        | Text info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Form.textField
                    {
                        Parser = Ok
                        Value =
                            fun values ->
                                Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.readValue
                                    specField
                                    values
                        Update =
                            Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.updateSingleFunc
                                id
                                specField
                        Error = fun _ -> None
                        Attributes =
                            {
                                Label = (t specField.Label)
                                Placeholder = ""
                                HtmlAttributes = []
                            }
                    }
                |> Form.disableIf readOnly
                |> optionalMatch specField.IsOptional
                |> dependencyMatch specField.DependsOn

        | Checkbox info ->
            if specField.IsDeprecated && not readOnly then
                emptyField
            else
                Form.checkboxField
                    {
                        Parser = (fun a -> Ok(string a))
                        Value = (fun value -> snd (bool.TryParse(readValue specField value)))
                        Update =
                            (fun value values ->
                                updateSingleFunc id specField (string value) values
                            )
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
