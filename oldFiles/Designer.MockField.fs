module Antidote.FormDesigner.MockField

open Fable.Form.Antidote
open Feliz
open Feliz.Bulma
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.FormDesigner.Types
open Antidote.FormDesigner.ChoiceFieldComponent

let renderMockField mockModel mockField =
    let mockForm = Form.succeed (fun b -> ()) |> Form.append mockField

    let view =
        Form.View.asHtml
            {
                Dispatch = (fun _ -> ())
                OnChange = (fun _ -> ())
                Action = Form.View.Action.Custom (fun _ _ -> Html.none)
                Validation = Form.View.ValidateOnSubmit
            }

    view mockForm mockModel

type MockFieldProps = {|
    FormSpec: FormSpec
    FormStep: FormStep
    FormField: FormField
    ActiveField: ActiveField

    SetActiveField: ActiveField -> unit
    OnChange: FormSpec -> unit
|}

[<ReactComponent>]
let MockField (props:MockFieldProps) =
    React.fragment [
        match props.FormField.FieldType with
        // TODO: Unify these controls after extending the
        // FormSpec to include properties for determining the appropriate 'finder'
        | FieldType.AllergyFinder d ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/allergy-field.png"
                    ]
                ]
            ]

        | FieldType.CPTFinder _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/cpt-field.png"
                    ]
                ]
            ]
        | FieldType.ICD10Finder _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/icd10-field.png"
                    ]
                ]
            ]
        | FieldType.DrugFinderWithFrequency _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/eprescribing-banner-image.png"
                    ]
                ]
            ]
        | FieldType.DrugFinder _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/eprescribing-banner-image.png"
                    ]
                ]
            ]
        | FieldType.EPrescribe _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/eprescribing-banner-image.png"
                    ]
                ]
            ]
        | FieldType.Image _ ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/placeholder.png"
                    ]
                ]
            ]
        | FieldType.TextArea _
        | FieldType.Text _
        | FieldType.Tel _
        | FieldType.Date _
        | FieldType.Time _
        | FieldType.Number _ ->
            let fstub =
                match props.FormField.FieldType with
                | FieldType.Text _ -> Form.textField, "{{short text}}"
                | FieldType.TextArea _ -> Form.textareaField, "{{long text}}"
                | FieldType.Tel _ -> Form.telField, "(555)-555-5555"
                | FieldType.Date _ -> Form.dateField, ""
                | FieldType.Time _ -> Form.timeField, ""
                | FieldType.Number _ -> Form.numberField, "9999"
                | _ -> failwith $"Non implemented field type {props.FormField.FieldType}"

            (fst fstub)
                {
                    Parser = Ok
                    Value = fun v -> v
                    Update =  fun _ v -> v
                    Error = fun _ -> None
                    Attributes =
                        {
                            Label = ""
                            Placeholder = snd fstub
                            HtmlAttributes = [ ]
                        }
                }
                |> Form.disableIf true
                |> renderMockField ("" |> Form.View.idle )
        | FieldType.CheckboxList _
        | FieldType.MultiChoice _
        | FieldType.SingleChoice _ ->
            ChoiceFieldComponent {|
                FormSpec = props.FormSpec
                FormStep = props.FormStep
                FormField = props.FormField
                ActiveField = props.ActiveField
                SetActiveField = props.SetActiveField

                OnChange = props.OnChange
            |}

        | FieldType.TextAutoComplete _ ->
            ChoiceFieldComponent {|
                FormSpec = props.FormSpec
                FormStep = props.FormStep
                FormField = props.FormField
                ActiveField = props.ActiveField
                SetActiveField = props.SetActiveField

                OnChange = props.OnChange
            |}
        | FieldType.Switch _ ->
            Form.switchField
                {
                    Parser = (fun value -> Ok (string value))
                    Value = (fun value -> value)
                    Update = (fun value values -> values )
                    Error = fun _ -> None
                    Attributes =
                        {
                            Text = props.FormField.Label
                            Id = "12345"
                        }
                }
            |> renderMockField (true |> Form.View.idle)

        | FieldType.TagList _ ->
            ChoiceFieldComponent {|
                FormSpec = props.FormSpec
                FormStep = props.FormStep
                FormField = props.FormField
                ActiveField = props.ActiveField
                SetActiveField = props.SetActiveField

                OnChange = props.OnChange
            |}

        | FieldType.Radio _ ->
            ChoiceFieldComponent {|
                FormSpec = props.FormSpec
                FormStep = props.FormStep
                FormField = props.FormField
                ActiveField = props.ActiveField
                SetActiveField = props.SetActiveField

                OnChange = props.OnChange
            |}

        | FieldType.Dropdown _ ->
            ChoiceFieldComponent {|
                FormSpec = props.FormSpec
                FormStep = props.FormStep
                FormField = props.FormField
                ActiveField = props.ActiveField
                SetActiveField = props.SetActiveField

                OnChange = props.OnChange
            |}

        | FieldType.Checkbox _ ->
            Form.checkboxField
                {
                    Parser = Ok
                    Value = fun v -> v
                    Update = fun v vs -> vs
                    Error = fun _ -> None
                    Attributes = { Text = props.FormField.Label }
                }
            |> renderMockField (true |> Form.View.idle)

        | FieldType.Message info ->
            Form.reactComponentField {
                Parser = Ok
                Value = fun v -> v
                Update = fun v vs -> vs
                Error = fun _ -> None
                Attributes = {
                    Label = ""
                    Render = Antidote.React.Components.FormMessage.FormMessageField info
                }
            }
            |> renderMockField ("" |> Form.View.idle)

        | FieldType.SpeechToText info ->
            Bulma.image [
                prop.style [ style.width 250 ]
                prop.children [
                    Html.img [
                        prop.src "/images/stt-field.png"
                    ]
                ]
            ]
        | FieldType.Signature info ->
            //Base64 of a sample signature
            Form.reactComponentField
                {
                    Parser = Ok
                    Value = fun _ -> "data:image/png;base64,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"
                    Update = fun _ b -> b
                    Error = fun _ -> None
                    Attributes =
                        {
                            Label = ""
                            Render = Antidote.React.Components.SignatureCanvas.SignatureField "mock"
                        }
                }
            |> Form.disableIf true
            |> renderMockField ("" |> Form.View.idle)

        | FieldType.StateSelectorUSA _ ->
            Html.div [
                prop.style [
                    style.textAlign.center
                ]
                prop.children [
                    Bulma.label [
                        prop.text "US State Selector"
                    ]
                    Html.img [
                        prop.src "/images/map.png"
                        prop.style [
                            style.borderRadius 5
                            style.padding 15
                            style.border (2, Feliz.borderStyle.solid, "transparent")
                        ]
                    ]
                ]
            ]
        | FieldType.TrueFalse _ ->
            Form.twoChoiceField
                {
                    Parser = Ok
                    Value = fun _ -> "true"
                    Update = fun _ -> id
                    Error = fun _ -> None
                    Attributes =
                        {
                            Id = ""
                            Label = ""
                            Options1 = "true","True"
                            Options2 = "false", "False"
                        }
                }
                // |> Form.disableIf true
            |> renderMockField ("" |> Form.View.idle)

        | FieldType.YesNo info ->
            Form.twoChoiceField
                {
                    Parser = Ok
                    Value = fun _ -> "yes"
                    Update = fun _ -> id
                    Error = fun _ -> None
                    Attributes =
                        {
                            Id = ""
                            Label = ""
                            Options1 = "yes","Yes"
                            Options2 = "no", "No"
                        }
                }
                // |> Form.disableIf true
            |> renderMockField ("" |> Form.View.idle)

        match props.FormField.DependsOn with
        | Some dep ->
                Bulma.icon [
                    prop.style [
                        style.floatStyle.right
                        style.bottom 20
                        style.position.relative
                    ]
                    prop.children [
                        Html.i [
                            prop.style [
                                if System.String.IsNullOrWhiteSpace dep.FieldValue
                                then style.color "red"
                                else
                                    style.color "#26619b"
                            ]
                            prop.className "fas fa-link"
                        ]
                    ]
                ]
        | None -> Html.none
    ]
