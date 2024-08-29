module Demo.App

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Fable.Form.Studio
open Fable.Form.Studio.Bulma
open Fable.Form.Studio.View
open Antidote.FormStudio
open Fable.Core.JsInterop
open System

let private classes: CssModules.App = import "default" "./App.module.scss"

let private formExample1: Specification.Form =
    {
        Title = "Create a new account"
        Fields =
            [
                Specification.Fields.Input
                    {
                        Guid = Guid "0982e4bc-2e5a-492d-9166-29bbcc4da3f0"
                        Label = "First name"
                        IsRequired = true
                        Condition = None
                    }
                Specification.Fields.Input
                    {
                        Guid = Guid "0982e4bc-2e5a-492d-9166-29bbcc4da3f1"
                        Label = "Last name"
                        IsRequired = false
                        Condition = None
                    }
                Specification.Fields.Checkbox
                    {
                        Guid = Guid "0982e4bc-2e5a-492d-9166-29bbcc4da3f2"
                        Label = "Can I ask your age?"
                        IsRequired = true
                        Condition = None
                    }
                Specification.Fields.Input
                    {
                        Guid = Guid "0982e4bc-2e5a-492d-9166-29bbcc4da3f3"
                        Label = "Age"
                        IsRequired = true
                        Condition =
                            ({
                                RefGuid = Guid "0982e4bc-2e5a-492d-9166-29bbcc4da3f2"
                                Value = Specification.FormValues.Checkbox true
                                Operation = Specification.Operation.Equal
                            }
                            : Specification.Condition)
                            |> Some
                    }
            ]
    }

type private Values = Map<Guid, Specification.FormValues>

type private Model = Form.View.Model<Values>

type private Msg =
    // Used when a change occure in the form
    | FormChanged of Model
    // Used when the user submit the form
    | LogIn of Specification.FilledFields list

let private init () = Map.empty |> Form.View.idle, Cmd.none

let private update (msg: Msg) (model: Model) =
    match msg with
    // We received a new form model, store it
    | FormChanged newModel -> newModel, Cmd.none

    // The form has been submitted
    // Here, we have access to the value submitted from the from
    | LogIn filledFields ->
        printfn "Form submitted with values: %A" filledFields
        // For this example, we just set a message in the Form view
        { model with
            State = Form.View.Success "You have been logged in successfully"
        },
        Cmd.none

module Form =

    let optionalIf
        (guid: Guid)
        (isRequired: bool)
        (form: Form.Form<'Values, Specification.FilledFields, 'Attributes>)
        // : Form.Form<'Values, Specification.FilledFields, 'Attributes>
        =
        if isRequired then
            Some form
        else
            None
    // TODO: Should we return a typed NotFilled or work with option?
    // This question applies to the condition application too
    // form
    // |> Form.optional
    // |> Form.andThen (fun x ->
    //     match x with
    //     | Some value -> Form.succeed value
    //     | None -> guid |> Specification.FilledFields.NotFilled |> Form.succeed
    // )

    let merge fields =
        fields
        |> List.map (fun form -> Form.succeed List.singleton |> Form.append form)
        |> List.reduce (fun form1 form2 ->
            // Merge forms by taking two, appending them and concatenating the
            // two lists of key-value pairs they produce (to get result of the same type)
            Form.succeed (fun r1 r2 -> r1 @ r2) |> Form.append form1 |> Form.append form2
        )

let private applyCondition
    (conditionOpt: Specification.Condition option)
    (values: Values)
    (form: Form.Form<'Values, Specification.FilledFields, 'Attributes>)
    =
    match conditionOpt with
    | Some condition ->
        match condition.Operation with
        | Specification.Operation.Equal ->
            match Map.tryFind condition.RefGuid values with
            | Some(Specification.FormValues.Checkbox checkboxValue) ->
                match condition.Value with
                | Specification.FormValues.Checkbox refValue ->
                    if checkboxValue = refValue then
                        Some form
                    else
                        None
                | _ -> None // Invalid specification, should we throw an exception?
            | Some(Specification.FormValues.Input inputValue) ->
                match condition.Value with
                | Specification.FormValues.Input refValue ->
                    if inputValue = refValue then
                        Some form
                    else
                        None
                | _ -> None // Invalid specification, should we throw an exception?
            | None -> None

    | None -> Some form

let private form (specFields: Specification.Fields list) : Form.Form<Values, Msg, IReactProperty> =
    let mappedFields =
        Form.meta (fun values ->
            specFields
            |> List.choose (fun specField ->
                match specField with
                | Specification.Fields.Input input ->
                    Form.textField
                        {
                            Parser =
                                fun value ->
                                    ({
                                        Guid = input.Guid
                                        Value = value
                                    }
                                    : Specification.FilledFields.Input)
                                    |> Specification.FilledFields.Input
                                    |> Ok
                            Value =
                                fun values ->
                                    match Map.tryFind input.Guid values with
                                    | Some(Specification.FormValues.Input value) -> value
                                    | None
                                    | _ -> ""
                            Update =
                                fun value values ->
                                    Map.add
                                        input.Guid
                                        (Specification.FormValues.Input value)
                                        values
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = input.Label
                                    Placeholder = ""
                                    HtmlAttributes = []
                                }
                        }
                    |> Form.optionalIf input.Guid input.IsRequired
                    |> Option.map (applyCondition input.Condition values)
                    |> Option.flatten

                | Specification.Fields.Checkbox checkbox ->
                    Form.checkboxField
                        {
                            Parser =
                                fun value ->
                                    ({
                                        Guid = checkbox.Guid
                                        Value = value
                                    }
                                    : Specification.FilledFields.Checkbox)
                                    |> Specification.FilledFields.Checkbox
                                    |> Ok
                            Value =
                                fun values ->
                                    match Map.tryFind checkbox.Guid values with
                                    | Some(Specification.FormValues.Checkbox value) -> value
                                    | None
                                    | _ -> false
                            Update =
                                fun value values ->
                                    Map.add
                                        checkbox.Guid
                                        (Specification.FormValues.Checkbox value)
                                        values
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Text = checkbox.Label
                                }
                        }
                    |> Form.optionalIf checkbox.Guid checkbox.IsRequired
                    |> Option.map (applyCondition checkbox.Condition values)
                    |> Option.flatten
            )
            |> Form.merge
        )

    let onSubmit = fun filledFields -> LogIn filledFields

    Form.succeed onSubmit |> Form.append mappedFields

[<ReactComponent>]
let private FillForm (formToFill: Specification.Form) =
    let model, dispatch = React.useElmish (init, update)

    Html.div [
        Bulma.text.div [
            prop.text formToFill.Title
        ]

        Html.br []

        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation = Form.View.ValidateOnSubmit
            }
            (form formToFill.Fields)
            model
    ]

[<ReactComponent>]
let App () =
    Html.div [
        Bulma.section []
        Bulma.container [
            FillForm formExample1
        ]

    ]
