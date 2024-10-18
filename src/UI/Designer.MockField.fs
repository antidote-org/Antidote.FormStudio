module Antidote.FormStudio.UI.Designer.MockField

open Fable.Form.Antidote
open Feliz
open Feliz.Bulma
open Antidote.FormStudio.Types

let renderMockField mockModel mockField =
    let mockForm = Form.succeed (fun b -> ()) |> Form.append mockField

    let view =
        Form.View.asHtml
            {
                Dispatch = (fun _ -> ())
                OnChange = (fun _ -> ())
                Action = Form.View.Action.Custom(fun _ _ -> Html.none)
                Validation = Form.View.ValidateOnSubmit
            }

    view mockForm mockModel

type MockFieldProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        FormField: FormField<'UserField>
        ActiveField: ActiveField
        RegisteredFields: RegisteredFields<'UserField>
        SetActiveField: ActiveField -> unit
        OnChange: FormSpec<'UserField> -> unit
    |}

[<ReactComponent>]
let MockField (props: MockFieldProps<'UserField>) =
    let designerFieldOpt = props.RegisteredFields.TryGetByKey props.FormField.FieldKey

    React.fragment [
        match designerFieldOpt with
        | Some designerField ->
            designerField.RenderPreview
                {
                    FormSpec = props.FormSpec
                    FormStep = props.FormStep
                    FormField = props.FormField
                    ActiveField = props.ActiveField
                    SetActiveField = props.SetActiveField
                    OnChange = props.OnChange
                }
        | None ->
            Bulma.message [
                color.isDanger
                prop.children [
                    Bulma.messageBody [
                        prop.text $"Field not found for key: %s{props.FormField.FieldKey}"
                    ]
                ]
            ]

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
                            if System.String.IsNullOrWhiteSpace dep.FieldValue then
                                style.color "red"
                            else
                                style.color "#26619b"
                        ]
                        prop.className "fas fa-link"
                    ]
                ]
            ]
        | None -> Html.none
    ]
