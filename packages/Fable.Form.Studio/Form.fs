module Fable.Form.Studio.Form

open Feliz
open Fable.Form
open Fable.Form.Studio
open Fable.Form.Studio.View
open Fable.Form.Studio.Field

type RadioField<'Values> = RadioField.RadioField<'Values>
type CheckboxField<'Values> = CheckboxField.CheckboxField<'Values>

open Elmish
open Feliz.Bulma

module View =

    let fieldLabel (label: string) = Bulma.label [ prop.text label ]

    let errorMessage (message: string) =
        Bulma.help [
            color.isDanger
            prop.text message
        ]

    let errorMessageAsHtml (showError: bool) (error: Error.Error option) =
        match error with
        | Some(Error.External externalError) -> errorMessage externalError

        | _ ->
            if showError then
                error
                |> Option.map Form.View.errorToString
                |> Option.map errorMessage
                |> Option.defaultValue (Bulma.help [])

            else
                Bulma.help []

    let wrapInFieldContainer (children: ReactElement list) =
        Bulma.field.div [ prop.children children ]

    let withLabelAndError
        (label: string)
        (showError: bool)
        (error: Error.Error option)
        (fieldAsHtml: ReactElement)
        : ReactElement
        =
        [
            fieldLabel label
            Bulma.control.div [ fieldAsHtml ]
            errorMessageAsHtml showError error
        ]
        |> wrapInFieldContainer

type OnBlur<'Msg> = string -> 'Msg option

/// <summary>
/// DUs used to represents the different of Field supported by Fable.Form.Studio
/// </summary>
type Field<'Values, 'Attributes> =
    abstract MapFieldValues:
        update: ('Values -> 'NewValues -> 'NewValues) ->
        values: 'NewValues ->
            Field<'NewValues, 'Attributes>

    abstract RenderField:
        OnBlur<'Msg> ->
        Dispatch<'Msg> ->
        Form.View.FieldConfig<'Values, 'Msg> ->
        FilledField<'Values, 'Attributes> ->
            ReactElement

/// <summary>
/// Represents a FilledField using Fable.Form.Studio representation
/// </summary>
and FilledField<'Values, 'Attributes> = Base.FilledField<Field<'Values, 'Attributes>>

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
        Attributes: CheckboxField.Attributes
    }

/// <summary>
/// Represents a form using Fable.Form.Studio representation
/// </summary>
type Form<'Values, 'Output, 'Attributes> = Base.Form<'Values, 'Output, Field<'Values, 'Attributes>>

type CheckboxField<'Values, 'Field, 'Output, 'Value, 'Attributes>
    (innerField: CheckboxField.CheckboxField<'Values>)
    =

    member _.GetRenderConfig
        (onBlur: OnBlur<'Msg>)
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (filledField: FilledField<'Values, 'Attributes>)
        =
        let config: CheckboxFieldConfig<'Msg> =
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

        config

    interface Field<'Values, 'Attributes> with

        member _.MapFieldValues
            (update: 'Values -> 'NewValues -> 'NewValues)
            (values: 'NewValues)
            : Field<'NewValues, 'Attributes>
            =
            let newUpdate oldValues = update oldValues values

            CheckboxField(Field.mapValues newUpdate innerField)

        member this.RenderField onBlur dispatch fieldConfig filledField =
            let config = this.GetRenderConfig onBlur dispatch fieldConfig filledField

            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            prop.onChange (config.OnChange >> config.Dispatch)
                            match config.OnBlur with
                            | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                            | None -> ()
                            prop.disabled config.Disabled
                            prop.isChecked config.Value
                        ]

                        Html.text config.Attributes.Text
                    ]
                ]
            ]
            |> List.singleton
            |> View.wrapInFieldContainer

type FormListField<'Values, 'Field, 'Output, 'Value, 'Attributes>
    (innerField: FormList.FormList<'Values, Field<'Values, 'Attributes>>)
    =

    member _.GetRenderConfig
        (onBlur: OnBlur<'Msg>)
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (filledField: FilledField<'Values, 'Attributes>)
        =
        // let config: CheckboxFieldConfig<'Msg> =
        //     {
        //         Dispatch = dispatch
        //         OnChange = field.Update >> fieldConfig.OnChange
        //         OnBlur = onBlur field.Attributes.Text
        //         Disabled = filledField.IsDisabled || fieldConfig.Disabled
        //         Value = field.Value
        //         Error = filledField.Error
        //         ShowError = fieldConfig.ShowError field.Attributes.Text
        //         Attributes = field.Attributes
        //     }

        // config
        failwith "Not implemented"

    interface Field<'Values, 'Attributes> with
        member this.RenderField onBlur dispatch fieldConfig filledField =


            // Html.div config.Attributes.Text
            failwith "Not implemented"

        member _.MapFieldValues
            (update: 'Values -> 'NewValues -> 'NewValues)
            (values: 'NewValues)
            : Field<'NewValues, 'Attributes>
            =
            FormListField
                {
                    Forms =
                        List.map
                            (fun (form: FormList.Form<'Values, Field<'Values, 'Attributes>>) ->
                                {
                                    Fields =
                                        List.map
                                            (fun
                                                (filledField:
                                                    Base.FilledField<Field<'Values, 'Attributes>>) ->
                                                {
                                                    State =
                                                        filledField.State.MapFieldValues
                                                            update
                                                            values
                                                    Error = filledField.Error
                                                    IsDisabled = filledField.IsDisabled
                                                }
                                            )
                                            form.Fields
                                    Delete = fun _ -> update (form.Delete()) values
                                }
                            )
                            innerField.Forms
                    // formList.Forms
                    Add = fun _ -> update (innerField.Add()) values
                    Attributes = innerField.Attributes
                }

// CheckboxField(Field.mapValues newUpdate field)

// member this.ToForm _ =
//     CheckboxField.form
//     failwith "Not implemented"

// static member Create(config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>) : Form<'Values, 'Output, 'Attributes> =
//     CheckboxField.form (fun field -> CheckboxField field) config
/// <summary>
/// Build a variable list of forms
///
/// An example is available <a href="https://mangelmaxime.github.io/Fable.Form/#form-list">on this page</a>
/// </summary>
/// <param name="config">A record used to configure the field behaviour.
/// <para>
/// See <see cref="T:Fable.Form.Base.FormList.Config"/> for more informations
/// </para>
/// </param>
/// <param name="elementForIndex">A function taking an index and returning a new form</param>
/// <returns>A form representing the list of form as a single form</returns>
/// <example>
/// <code lang="fsharp">
/// let bookForm (index : int) : Form.Form&gt;BookValues,Book&lt; =
///     // ...
///
/// Form.succeed onSubmit
///     |> Form.append (
///         Form.list
///             {
///                 Default =
///                     {
///                         Title = ""
///                         Author = ""
///                         Summary = ""
///                     }
///                 Value =
///                     fun values -> values.Books
///                 Update =
///                     fun newValue values ->
///                         { values with Books = newValue }
///                 Attributes =
///                     {
///                         Label = "Books"
///                         Add = Some "Add book"
///                         Delete = Some "Remove book"
///                     }
///             }
///             bookForm
///     )
/// </code>
///
/// In this example, <c>append</c> is used to feed <c>onSubmit</c> function and combine it into a <c>Login</c> message when submitted.
/// </example>
let list
    (config: FormList.Config<'Values, 'ElementValues>)
    (elementForIndex: int -> Form<'ElementValues, 'Output, 'Attributes>)
    : Form<'Values, 'Output list, 'Attributes>
    =

    let fillElement
        (elementState: FormList.ElementState<'Values, 'ElementValues>)
        : Base.FilledForm<'Output, Field<'Values, 'Attributes>>
        =
        let filledElement =
            Base.fill (elementForIndex elementState.Index) elementState.ElementValues

        {
            Fields =
                filledElement.Fields
                |> List.map (fun filledField ->
                    {
                        State =
                            filledField.State.MapFieldValues
                                elementState.Update
                                elementState.Values
                        Error = filledField.Error
                        IsDisabled = filledField.IsDisabled
                    }
                )
            Result = filledElement.Result
            IsEmpty = filledElement.IsEmpty
        }

    let tagger
        (formList: FormList.FormList<'Values, Field<'Values, 'Attributes>>)
        : Field<'Values, 'Attributes>
        =
        FormListField formList

    FormList.form tagger config fillElement

let checkboxField
    (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
    : Form<'Values, 'Output, 'Attributes>
    =
    CheckboxField.form (fun field -> CheckboxField field) config

// member this.FieldInfo ()
//     (blur : string -> option<'Msg>)
//     (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
//     = failwith "Not implemented"

// type CheckboxField2<'Values, 'Field, 'Output, 'Value>
//     private (field: CheckboxField.CheckboxField<'Values>)
//     =
//     interface Field<'Values, CheckboxField.Attributes>
//     member this.RenderField attributes = Html.div "This is a checkbox"
//     member this.ToForm(field: CheckboxField.CheckboxField<'Values>) = CheckboxField.form

//     member this.Create(config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>) =
//         CheckboxField.form (fun field -> CheckboxField2 field) config

//     member this.FieldInfo() = field
// Redefined some function from the Base module so the user can access them transparently
// and they are also specifically typed for the Fable.Form.Studio absttraction

/// <summary>
/// Create a form that always succeeds when filled.
/// </summary>
/// <param name="output">The value to return when the form is filled</param>
/// <returns>The given <c>Output</c></returns>
let succeed (output: 'Output) : Form<'Values, 'Output, 'Attributes> = Base.succeed output

/// <summary>
/// Append a form to another one while <b>capturing</b> the output of the first one
/// </summary>
/// <param name="newForm">Form to append</param>
/// <param name="currentForm">Form to append to</param>
/// <returns>A new form resulting in the combination of <c>newForm</c> and <c>currentForm</c></returns>
/// <example>
/// <code lang="fsharp">
/// let emailField =
///     Form.emailField
///         {
///             // ...
///         }
///
/// let passwordField =
///     Form.passwordField
///         {
///             // ...
///         }
///
/// let onSubmit =
///     fun email password ->
///         LogIn (email, password)
///
/// Form.succeed onSubmit
///     |> Form.append emailField
///     |> Form.append passwordField
/// </code>
///
/// In this example, <c>append</c> is used to feed <c>onSubmit</c> function and combine it into a <c>Login</c> message when submitted.
/// </example>
let append
    (newForm: Form<'Values, 'A, 'Attributes>)
    (currentForm: Form<'Values, 'A -> 'B, 'Attributes>)
    : Form<'Values, 'B, 'Attributes>
    =

    Base.append newForm currentForm

/// <summary>
/// Disable a form
///
/// You can combine this with meta to disable parts of a form based on its own values.
/// </summary>
/// <param name="form">The form to disable</param>
/// <returns>A new form which has been marked as disabled</returns>
let disable (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A, 'Attributes> =

    Base.disable form

/// <summary>
/// Fill a form <c>andThen</c> fill another one.
///
/// This type of form is useful when some part of your form can dynamically change based on the value of another field.
/// </summary>
/// <param name="child">The child form</param>
/// <param name="parent">The parent form which is filled first</param>
/// <returns>A new form which is the result of filling the <c>parent</c> and then filling the <c>child</c> form</returns>
/// <example>
/// <para>Imagine you have a form to create a student or a teacher. Based on the type of user selected you can show the student form or the teacher form.</para>
/// <code lang="fsharp">
/// Form.selectField
///     {
///         Parser = function
///             | "student" ->
///                 Ok Student
///
///             | "teacher" ->
///                 Ok Teacher
///
///             | _ ->
///                 Error "Invalid user type"
///         Value =
///             fun values -> values.UserType
///         Update =
///             fun newValue values ->
///                 { values with UserType = newValue }
///         Error =
///             fun _ -> None
///         Attributes =
///             {
///                 Label = "Type of user"
///                 Placeholder = "Choose a user type"
///                 Options =
///                     [
///                         "student", "Student"
///                         "teacher", "Teacher"
///                     ]
///             }
///     }
/// |> Form.andThen (
///     function
///     | Student ->
///         let nameField =
///             Form.textField
///                 {
///                     // ...
///                 }
///
///         Form.succeed NewStudent
///             |> Form.append nameField
///
///     | Teacher ->
///         let nameField =
///             Form.textField
///                 {
///                     // ...
///                 }
///
///         let subjectField =
///             Form.textField
///                 {
///                     // ...
///                 }
///
///         let onSubmit name subject =
///             NewTeacher (name, subject)
///
///         Form.succeed onSubmit
///             |> Form.append nameField
///             |> Form.append subjectField
/// )
/// </code>
/// </example>
let andThen
    (child: 'A -> Form<'Values, 'B, 'Attributes>)
    (parent: Form<'Values, 'A, 'Attributes>)
    : Form<'Values, 'B, 'Attributes>
    =

    Base.andThen child parent

/// <summary>
/// Make a form be <b>optional</b>
///
/// If the form has values set, it will return <c>Some 'Value</c>.
///
/// Otherwise, it returns <c>None</c>.
/// </summary>
/// <param name="form">Form to make optional</param>
/// <example>
/// <code lang="fsharp">
/// let emailField =
///     Form.emailField
///         {
///             // ...
///         }
///
/// let ageField =
///     Form.numberField
///         {
///             // ...
///         }
///
/// let onSubmit =
///     fun email ageOpt ->
///         LogIn (email, ageOpt)
///
/// Form.succeed onSubmit
///     |> Form.append emailField
///     |> Form.append (Form.optional ageField)
/// </code>
/// </example>
/// <returns>An optional form</returns>
let optional (form: Form<'Values, 'A, 'Attributes>) : Form<'Values, 'A option, 'Attributes> =
    Base.optional form

// /// <summary>
// /// Create a form that contains a single checkbox field
// /// </summary>
// /// <param name="config">A record used to configure the field behaviour.
// /// <para>
// /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
// /// </para>
// /// </param>
// /// <returns>Returns a form representing a checkbox field</returns>
// let checkboxField
//     (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
//     : Form<'Values, 'Output, 'Attributes>
//     =
//     CheckboxField.form Field.Checkbox config

// /// <summary>
// /// Create a form that contains a single radio field
// /// </summary>
// /// <param name="config">A record used to configure the field behaviour.
// /// <para>
// /// See <see cref="T:Fable.Form.Base.FieldConfig"/> for more informations
// /// </para>
// /// </param>
// /// <returns>Returns a form representing a radio field</returns>
// let radioField
//     (config: Base.FieldConfig<RadioField.Attributes, string, 'Values, 'Output>)
//     : Form<'Values, 'Output, 'Attributes>
//     =
//     RadioField.form Field.Radio config

// TODO: Module to an internal or advanced module
// We need to expose the function so user can use it when creating their own field
// and re-using the field from this module
// let rec mapFieldValues
//     (update: 'A -> 'B -> 'B)
//     (values: 'B)
//     (field: Field<'A, 'Attributes>)
//     : Field<'B, 'Attributes>

/// <summary>
/// Build a form that depends on its own <c>'Values</c>
///
/// This is useful when a field need to checks it's value against another field value.
///
/// The classic example for using <c>meta</c> is when dealing with a repeat password field.
/// </summary>
/// <param name="fn">Function to apply to transform the form values</param>
/// <returns>A new form resulting of the application of <c>fn</c> when filling it</returns>
/// <example>
/// The classic example for using <c>Base.meta</c> is when dealing with a repeat password field.
/// <code lang="fsharp">
/// Form.meta
///     (fun values ->
///         Form.passwordField
///             {
///                 Parser =
///                     fun value ->
///                         if value = values.Password then
///                             Ok ()
///
///                         else
///                             Error "The passwords do not match"
///                 Value = fun values -> values.RepeatPassword
///                 Update =
///                     fun newValue values_ ->
///                         { values_ with RepeatPassword = newValue }
///                 Error =
///                     fun _ -> None
///                 Attributes =
///                     {
///                         Label = "Repeat password"
///                         Placeholder = "Your password again..."
///                     }
///             }
///     )
/// </code>
/// </example>
let meta
    (fn: 'Values -> Form<'Values, 'Output, 'Attributes>)
    : Form<'Values, 'Output, 'Attributes>
    =

    Base.meta fn

[<NoComparison; NoEquality>]
type MapValuesConfig<'A, 'B> =
    {
        Value: 'A -> 'B
        Update: 'B -> 'A -> 'A
    }

// /// <summary>
// /// Transform the values of a form.
// ///
// /// This function is useful when you want to re-use existing form or nest them.
// /// </summary>
// /// <param name="config">A record used to configure the mapping behaviour.
// /// <para>
// /// See <see cref="T:Fable.Form.Studio.Form.MapValuesConfig`2"/> for more informations
// /// </para>
// /// </param>
// /// <param name="form">The form to which we want to pass the result of the transformation</param>
// /// <returns>
// /// A new form resulting of <c>fn >> fill form</c>
// /// </returns>
// let mapValues
//     ({
//          Value = value
//          Update = update
//      }: MapValuesConfig<'A, 'B>)
//     (form: Form<'B, 'Output, 'Attributes>)
//     : Form<'A, 'Output, 'Attributes>
//     =

//     Base.meta (fun values ->
//         form |> Base.mapValues value |> Base.mapField (mapFieldValues update values)
//     )

module View =

    open Elmish
    open Feliz

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
            Attributes: CheckboxField.Attributes
        }

    [<NoComparison; NoEquality>]
    type RadioFieldConfig<'Msg> =
        {
            Dispatch: Dispatch<'Msg>
            OnChange: string -> 'Msg
            OnBlur: 'Msg option
            Disabled: bool
            Value: string
            Error: Error.Error option
            ShowError: bool
            Attributes: RadioField.Attributes
        }

    [<NoComparison; NoEquality>]
    type CustomConfig<'Msg, 'Attributes> =
        {
            CheckboxField: CheckboxFieldConfig<'Msg> -> ReactElement
            RadioField: RadioFieldConfig<'Msg> -> ReactElement
        // Group: ReactElement list -> ReactElement
        // Section: string -> ReactElement list -> ReactElement
        }

    let ignoreChildError
        (parentError: Error.Error option)
        (field: FilledField<'Values, 'Attributes>)
        : FilledField<'Values, 'Attributes>
        =

        match parentError with
        | Some _ -> field

        | None ->
            { field with
                Error = None
            }

    // renderField

    let rec renderField
        (customConfig: CustomConfig<'Msg, 'Attributes>)
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (field: FilledField<'Values, 'Attributes>)
        : ReactElement
        =

        let blur label =
            Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

        // let config =
        //     let fieldInfo = field.State.FieldInfo()

        //     {
        //         Dispatch = dispatch
        //         OnChange = fieldInfo.Update >> fieldConfig.OnChange
        //         OnBlur = blur fieldInfo.Attributes.Text
        //         Disabled = field.IsDisabled || fieldConfig.Disabled
        //         Value = fieldInfo.Value
        //         Error = field.Error
        //         ShowError = fieldConfig.ShowError fieldInfo.Attributes.Text
        //         Attributes = fieldInfo.Attributes
        //     }

        field.State.RenderField blur dispatch fieldConfig field

// match field.State with
// | Field.Checkbox info ->
//     let config: CheckboxFieldConfig<'Msg> =
//         {
//             Dispatch = dispatch
//             OnChange = info.Update >> fieldConfig.OnChange
//             OnBlur = blur info.Attributes.Text
//             Disabled = field.IsDisabled || fieldConfig.Disabled
//             Value = info.Value
//             Error = field.Error
//             ShowError = fieldConfig.ShowError info.Attributes.Text
//             Attributes = info.Attributes
//         }

//     customConfig.CheckboxField config

// | Field.Radio info ->
//     let config: RadioFieldConfig<'Msg> =
//         {
//             Dispatch = dispatch
//             OnChange = info.Update >> fieldConfig.OnChange
//             OnBlur = blur info.Attributes.Label
//             Disabled = field.IsDisabled || fieldConfig.Disabled
//             Value = info.Value
//             Error = field.Error
//             ShowError = fieldConfig.ShowError info.Attributes.Label
//             Attributes = info.Attributes
//         }

//     customConfig.RadioField config
