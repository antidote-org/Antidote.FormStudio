namespace Antidote.Form.Designer.Bulma

open Feliz
open Elmish
open Fable.Form
open Antidote.Form.Designer

type OnBlur<'Msg> = string -> 'Msg option

/// <summary>
/// Wrapper around System.Guid to represent the active field id
///
/// This is to avoid confusion between Guid of the current field and the active field
/// </summary>
type ActiveFieldId =
    | ActiveFieldId of System.Guid

    member this.IsEqualTo(other: System.Guid) =
        match this with
        | ActiveFieldId id -> id = other

/// <summary>
/// DUs used to represents the different of Field supported by Fable.Form.Studio
/// </summary>
type IField<'Values, 'Attributes> =

    abstract RenderPreview:
        ActiveFieldId ->
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
