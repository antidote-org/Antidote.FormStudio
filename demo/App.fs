module Demo.App

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Fable.Form.Studio
open Fable.Form.Studio.Bulma
open Fable.Form.Studio.View
open Antidote.FormStudio
open Browser

type FieldValues =
    {
        FieldType: string // TODO: Make it typed?
        Label: string
    }

type private Values =
    {
        Title: string
        Fields: FieldValues list
        IsChecked : bool
    }

type private Model = Form.View.Model<Values>

type private Msg =
    // Used when a change occure in the form
    | FormChanged of Model
    // Used when the user submit the form
    // | LogIn of Specification.Form
    | LogIn of bool

let private init () =
    {
        Title = ""
        Fields = []
        IsChecked = false
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

let private form: Form.Form<Values, Msg, _> =
    let checkField =
        Form.checkboxField
            {
                Parser = Ok
                Value = fun values -> values.IsChecked
                Update =
                    fun newValue values ->
                        { values with
                            IsChecked = newValue
                        }
                Error = fun _ -> None
                Attributes =
                    {
                        Text = "This is a checkbox"
                    }
            }


    // let onSubmit =
    //     fun title fields ->
    //         ({
    //             Title = title
    //             Fields = fields
    //         }
    //         : Specification.Form)
    //         |> LogIn
    let onSubmit =
        fun isChecked ->
            LogIn isChecked

    Form.succeed onSubmit |> Form.append checkField

[<ReactComponent>]
let App () =
    let model, dispatch = React.useElmish (init, update)

    let portalDest =
        let dest = document.getElementById("field-properties-portal")
        if isNull dest then
            None
        else
            Some dest

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

        if portalDest.IsSome then
            ReactDOM.createPortal(
                Html.div "Hello from portal", portalDest.Value
            )


        if portalDest.IsSome then
            ReactDOM.createPortal(
                Html.div "Hello from portal2sqsq", portalDest.Value
            )

        Html.div [
            prop.draggable true
            prop.children [
                Html.div "Hello"
                Html.div "World"
            ]
        ]
    ]

// type IFieldDesigner =
//     abstract PropertyEditor : Specification.Fields -> ReactElement
//     abstract ToSpecification : obj -> Specification.Fields
//     abstract ComponentSelector : unit -> ReactElement

// type IMapValues =
//     abstract MapValues : obj -> obj

// type TextFieldDesigner () =
//     interface IFieldDesigner with

//         member this.PropertyEditor field =
//             Html.div [
//                 prop.children [
//                     Html.div "Label"
//                     Html.input [
//                         // prop.value field.Label
//                         // prop.onChange (fun e -> field.Label <- e.target?value)
//                     ]
//                 ]
//             ]

//         member this.ToSpecification obj =
//             Specification.Fields.Input
//                 {
//                     Guid = System.Guid.NewGuid()
//                     Label = "Hello"
//                     IsRequired = false
//                     Condition = None
//                 }

//         member this.ComponentSelector () =
//             Html.div "TextField"

// let test = TextFieldDesigner()

// let test2 (field : IFieldDesigner) =
//     match field with
//     | :? IMapValues as designer -> ()
//     | _ -> ()
