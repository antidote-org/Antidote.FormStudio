module Demo.App

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Fable.Form.Studio
open Fable.Form.Studio.Bulma
open Fable.Form.Studio.View
open Antidote.FormStudio

type FieldValues =
    {
        FieldType: string // TODO: Make it typed?
        Label: string
    }

type private Values =
    {
        Title: string
        Fields: FieldValues list
    }

type private Model = Form.View.Model<Values>

type private Msg =
    // Used when a change occure in the form
    | FormChanged of Model
    // Used when the user submit the form
    | LogIn of Specification.Form

let private init () =
    {
        Title = ""
        Fields = []
    }
    |> Form.View.idle,
    Cmd.none

let private update (msg: Msg) (model: Model) =
    match msg with
    // We received a new form model, store it
    | FormChanged newModel -> newModel, Cmd.none

    // The form has been submitted
    // Here, we have access to the value submitted from the from
    | LogIn formSpec ->
        printfn "Form submitted: %A" formSpec
        // For this example, we just set a message in the Form view
        { model with
            State = Form.View.Success "You have been logged in successfully"
        },
        Cmd.none

[<RequireQualifiedAccess>]
type private FieldType =
    | Input
    | Checkbox

module private FieldType =

    let tryParse (value: string) =
        match value with
        | "input" -> Ok FieldType.Input
        | "checkbox" -> Ok FieldType.Checkbox
        | _ -> Error "Invalid field type"

let private inputFieldForm: Form.Form<FieldValues, Specification.Fields, IReactProperty> =
    let labelField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Label
                Update =
                    fun newValue values ->
                        { values with
                            Label = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Label"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    // let onSubmit guid label isOptional condition =
    let onSubmit label =
        Specification.Fields.Input
            {
                Guid = System.Guid.NewGuid()
                Label = label
                IsRequired = false
                Condition = None
            }

    Form.succeed onSubmit |> Form.append labelField

let private checkboxFieldForm: Form.Form<FieldValues, Specification.Fields, IReactProperty> =
    let labelField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Label
                Update =
                    fun newValue values ->
                        { values with
                            Label = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Label"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    // let onSubmit guid label isOptional condition =
    let onSubmit label =
        Specification.Fields.Checkbox
            {
                Guid = System.Guid.NewGuid()
                Label = label
                IsRequired = false
                Condition = None
            }

    Form.succeed onSubmit |> Form.append labelField

let private fieldForm (index: int) =
    let formTypeField =
        Form.selectField
            {
                Parser = FieldType.tryParse
                Value = fun values -> values.FieldType
                Update =
                    fun newValue values ->
                        { values with
                            FieldType = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = ""
                        Placeholder = "Choose a field type"
                        Options =
                            [
                                "input", "Input"
                                "checkbox", "Checkbox"
                            ]
                    }
            }

    formTypeField
    |> Form.andThen (
        function
        | FieldType.Input -> inputFieldForm
        | FieldType.Checkbox -> checkboxFieldForm
    )

let private listOfFieldForm: Form.Form<Values, Specification.Fields list, IReactProperty> =
    Form.list
        {
            Default =
                {
                    FieldType = ""
                    Label = ""
                }
            Value = fun values -> values.Fields
            Update =
                fun newValue values ->
                    { values with
                        Fields = newValue
                    }
            Attributes =
                {
                    Label = "Fields"
                    Add = Some "Add field"
                    Delete = Some "Remove field"
                }
        }
        fieldForm

let private form: Form.Form<Values, Msg, _> =
    let titleField =
        Form.textField
            {
                Parser = Ok
                Value = fun values -> values.Title
                Update =
                    fun newValue values ->
                        { values with
                            Title = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Label = "Title"
                        Placeholder = ""
                        HtmlAttributes = []
                    }
            }

    let onSubmit =
        fun title fields ->
            ({
                Title = title
                Fields = fields
            }
            : Specification.Form)
            |> LogIn

    Form.succeed onSubmit |> Form.append titleField |> Form.append listOfFieldForm

[<ReactComponent>]
let App () =
    let model, dispatch = React.useElmish (init, update)

    let portalDest = React.useElementRef ()

    Bulma.container [
        Bulma.section []
        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = Form.View.Action.SubmitOnly "Sign in"
                Validation = Form.View.ValidateOnSubmit
            }
            form
            model
        Html.div [
            prop.id "test-portal"
            prop.ref portalDest
        ]

        if portalDest.current.IsSome then
            ReactDOM.createPortal(
                Html.div "Hello from portal", portalDest.current.Value
            )


        if portalDest.current.IsSome then
            ReactDOM.createPortal(
                Html.div "Hello from portal2sqsq", portalDest.current.Value
            )
    ]
