namespace Fable.Form.Studio.Bulma

open Feliz
open Elmish
open Fable.Form
open Fable.Form.Studio
open Fable.Form.Studio.Bulma.Fields

[<RequireQualifiedAccess>]
module Form =

    module View =

        let asHtml (config: Form.View.ViewConfig<'Values, 'Msg>) =
            Form.View.render config Internal.View.form Internal.View.renderField

    // Specialized combinators

    let succeed (output: 'Output) : Form<'Values, 'Output, 'Attributes> = Base.succeed output

    let append
        (newForm: Form<'Values, 'A, 'Attributes>)
        (currentForm: Form<'Values, 'A -> 'B, 'Attributes>)
        : Form<'Values, 'B, 'Attributes>
        =
        Base.append newForm currentForm

    // Fields helpers

    type CheckboxField<'Values> = CheckboxField.InnerField<'Values>
    type TextField<'Values> = TextField.InnerField<'Values>

    let checkboxField
        (config: Base.FieldConfig<CheckboxField.Attributes, bool, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        CheckboxField.form (fun field -> CheckboxField.Field field) config

    let textField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextRaw, field)) config

    let passwordField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextPassword, field)) config

    let emailField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextEmail, field)) config

    let colorField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextColor, field)) config

    let dateField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextDate, field)) config

    let dateTimeLocalField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextDateTimeLocal, field)) config

    let numberField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextNumber, field)) config

    let searchField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextSearch, field)) config

    let telField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextTel, field)) config

    let timeField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextTime, field)) config

    let textareaField
        (config: Base.FieldConfig<TextField.Attributes, string, 'Values, 'Output>)
        : Form<'Values, 'Output, 'Attributes>
        =
        TextField.form (fun field -> TextField.Field(TextField.TextArea, field)) config

    let list
        (config: FormList.Config<'Values, 'ElementValues>)
        (elementForIndex: int -> Form<'ElementValues, 'Output, 'Attributes>)
        : Form<'Values, 'Output list, 'Attributes>
        =

        let fillElement
            (elementState: FormList.ElementState<'Values, 'ElementValues>)
            : Base.FilledForm<'Output, IField<'Values, 'Attributes>>
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
            (formList: FormList.FormListBag<'Values, IField<'Values, 'Attributes>>)
            : IField<'Values, 'Attributes>
            =
            FormList.FormListField formList

        FormList.form tagger config fillElement
