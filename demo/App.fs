module Demo.App

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Antidote.Form.Designer
open Antidote.Form.Designer.Bulma
open Antidote.Form.Designer.Bulma.Fields
open Browser

type private Values =
    {
        CheckboxField: CheckboxField.Value
        CheckboxField2: CheckboxField.Value
    }

type private Model =
    // Used when the form is being filled
    | FillingForm of Form.View.Model<Values>
    // Used when the form has been submitted with success
    | FormFilled of unit

type private Msg =
    // Message to react to form change
    | FormChanged of Form.View.Model<Values>
    // Message sent when the form is submitted
    | Submit of unit
    // Message sent when the user ask to reset the demo
    | ResetDemo

let private init () =
    {
        CheckboxField =
            {
                Id = System.Guid.Parse "beb671f5-790e-4c37-8ad9-5656b679db0b"
                IsFocused = false
                DefaultValue = false
                Text = ""
            }
        CheckboxField2 =
            {
                Id = System.Guid.Parse "beb671f5-790e-4c37-8ad9-5656b679db0a"
                IsFocused = false
                DefaultValue = false
                Text = ""
            }
    }
    |> Form.View.idle
    |> FillingForm,
    Cmd.none

let private update (msg: Msg) (model: Model) =
    match msg with
    // Update our model to it's new state
    | FormChanged newModel ->
        match model with
        | FillingForm _ -> FillingForm newModel, Cmd.none

        | FormFilled _ -> model, Cmd.none

    | Submit() ->
        printfn "Form submitted"
        // match model with
        // | FillingForm _ -> FormFilled(name, books), Cmd.none

        // | FormFilled _ -> model, Cmd.none
        model, Cmd.none

    | ResetDemo -> init ()

/// <summary>
/// Define the form logic
///
/// We need to define each field logic first and then define how the fields are wired together to make the form
/// </summary>
/// <returns>The form ready to be used in the view</returns>
let private form: Form<Values, Msg, IReactProperty> =
    let checkbox1 =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.CheckboxField
                Update =
                    fun newValue values ->
                        { values with
                            CheckboxField = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Text = "Checkbox 1"
                    }
            }

    let checkbox2 =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.CheckboxField2
                Update =
                    fun newValue values ->
                        { values with
                            CheckboxField2 = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Text = "Checkbox 2"
                    }
            }

    let onSubmit checkbox1 checkbox2 =
        printfn "Checkbox1: %A" checkbox1
        Submit()

    Form.succeed onSubmit |> Form.append checkbox1 |> Form.append checkbox2

let private view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | FillingForm values ->
        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Submit"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            values

    | FormFilled() -> Html.div "Form submitted"

[<ReactComponent>]
let App () =
    let model, dispatch = React.useElmish (init, update)

    Bulma.container [
        Bulma.section []

        view model dispatch
    ]
