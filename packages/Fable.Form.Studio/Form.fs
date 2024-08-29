module Fable.Form.Studio.Form

open Feliz
open Fable.Form
open Fable.Form.Studio
open Fable.Form.Studio.View
open Fable.Form.Studio.Field

type RadioField<'Values> = RadioField.RadioField<'Values>
type CheckboxField<'Values> = CheckboxField.CheckboxField<'Values>

open Elmish

[<NoComparison; NoEquality>]
type CommonFieldConfig<'Msg, 'Input, 'Attributes> =
    {
        Dispatch: Dispatch<'Msg>
        OnChange: 'Input -> 'Msg
        OnBlur: 'Msg option
        Disabled: bool
        Value: bool
        Error: Error.Error option
        ShowError: bool
        Attributes: 'Attributes
    }

// type IExtensibleField<'Values, 'Attributes> =
//     abstract RenderField:
//         (string -> option<'Msg>) ->
//         Form.View.FieldConfig<'Values, 'Msg> ->
//         Dispatch<'Msg> ->
//         FilledField<'Values, 'Attributes> ->
//         ReactElement

//     abstract MapFieldValues : unit -> unit

// /// <summary>
// /// DUs used to represents the different of Field supported by Fable.Form.Studio
// /// </summary>
// and Field<'Values, 'Attributes> =
//     | List of Field<'Values, 'Attributes> list
//     | Group of Field<'Values, 'Attributes> list
//     | Setion of title : string * Field<'Values, 'Attributes> list
//     | ExtensibleField of IExtensibleField<'Values, 'Attributes>
    // | TextInput of TextInputField<'Values>

    // abstract ToForm<'Field, 'Input, 'Output> :
    //     'Field ->
    //         (('Field -> Field<'Values, 'Attributes>)
    //             -> Base.FieldConfig<'Attributes, 'Input, 'Values, 'Output>
    //             -> Base.Form<'Values, 'Output, 'Field>)
// abstract FieldInfo<'Input> : unit -> Field.Field<'Attributes, 'Input, 'Values>

// | Checkbox of CheckboxField<'Values>
// | Radio of RadioField<'Values>
// | Section of title: string * FilledField<'Values, 'Attributes> list

/// <summary>
/// DUs used to represents the different of Field supported by Fable.Form.Studio
/// </summary>
and Field<'Values, 'Attributes> =
    abstract RenderField:
        (string -> option<'Msg>) ->
        Form.View.FieldConfig<'Values, 'Msg> ->
        Dispatch<'Msg> ->
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

type CheckboxField<'Values, 'Field, 'Output, 'Value, 'Attributes> (field: CheckboxField.CheckboxField<'Values>)
    =
    interface Field<'Values, 'Attributes> with
        member this.RenderField onBlur fieldConfig dispatch filledField  =
            let config: CheckboxFieldConfig<'Msg> =
                {
                    Dispatch = dispatch
                    OnChange = field.Update >> fieldConfig.OnChange
                    OnBlur = onBlur field.Attributes.Text
                    Disabled = filledField.IsDisabled || fieldConfig.Disabled
                    Value = field.Value
                    Error = filledField.Error
                    ShowError = fieldConfig.ShowError field.Attributes.Text
                    Attributes = field.Attributes
                }

            Html.div "This is a checkdwdwdwdwdddwdwbox"

        // member this.ToForm _ =
        //     CheckboxField.form
        //     failwith "Not implemented"

    // static member Create(config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>) : Form<'Values, 'Output, 'Attributes> =
    //     CheckboxField.form (fun field -> CheckboxField field) config

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

        field.State.RenderField blur fieldConfig dispatch field

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
