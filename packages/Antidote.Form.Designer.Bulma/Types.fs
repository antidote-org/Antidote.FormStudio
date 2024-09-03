namespace Antidote.Form.Designer.Bulma

open Feliz
open Elmish
open Fable.Form
open Antidote.Form.Designer

type OnBlur<'Msg> = string -> 'Msg option

/// <summary>
/// DUs used to represents the different of Field supported by Fable.Form.Studio
/// </summary>
type IField<'Values, 'Attributes> =

    abstract RenderPreview:
        System.Guid ->
        OnBlur<'Msg> ->
        Dispatch<'Msg> ->
        Form.View.FieldConfig<'Values, 'Msg> ->
        FilledField<'Values, 'Attributes> ->
            ReactElement

    abstract RenderPropertiesEditor:
        OnBlur<'Msg> ->
        Dispatch<'Msg> ->
        Form.View.FieldConfig<'Values, 'Msg> ->
        FilledField<'Values, 'Attributes> ->
            ReactElement

    abstract Id: System.Guid

and FilledField<'Values, 'Attributes> = Base.FilledField<IField<'Values, 'Attributes>>

/// <summary>
/// Represents a form using Fable.Form.Studio representation
/// </summary>
type Form<'Values, 'Output, 'Attributes> = Base.Form<'Values, 'Output, IField<'Values, 'Attributes>>
