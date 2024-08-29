module Fable.Form.Studio.Field.DesignerField

open Fable.Form

type Attributes<'Attributes> =
    {
        /// <summary>
        /// Label to display
        /// </summary>
        Label: string
        /// <summary>
        /// Placeholder to display when the field is empty
        /// </summary>
        Placeholder: string
        /// <summary>
        /// A list of HTML attributes to add to the generated field
        /// </summary>
        HtmlAttributes: 'Attributes list
    }

type Value<'InnerValue> =
    {
        ShowModal : bool
        Value : 'InnerValue
    }

type DesignerField<'Values, 'Attributes, 'InnerValue> = Field.Field<Attributes<'Attributes>, Value<'InnerValue>, 'Values>

let form<'Values, 'Attributes, 'InnerValue, 'Field, 'Output>
    : ((DesignerField<'Values, 'Attributes, 'InnerValue> -> 'Field)
          -> Base.FieldConfig<Attributes<'Attributes>, Value<'InnerValue>, 'Values, 'Output>
          -> Base.Form<'Values, 'Output, 'Field>) =
    Base.field (fun _ -> false)
