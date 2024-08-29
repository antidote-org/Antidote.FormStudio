module Antidote.FormStudio.Specification

open System
open Fable.Core
open Feliz

/// <summary>
/// Represent the value of a field at the view level (when the form is being filled)
///
/// This type is also used inside the condition engine
/// </summary>
[<RequireQualifiedAccess>]
type FormValues =
    | Input of string
    | Checkbox of bool

type Operation = | Equal

type Condition =
    {
        RefGuid: Guid
        Value: FormValues
        Operation: Operation
    }

module Fields =

    [<Mangle>]
    type IField =
        abstract IsRequired: bool
        abstract Condition: Condition option

    type Input =
        {
            Guid: Guid
            Label: string
            IsRequired: bool
            Condition: Condition option // TODO: Should be a list
        }

        interface IField with
            member this.IsRequired = this.IsRequired
            member this.Condition = this.Condition

    type Checkbox =
        {
            Guid: Guid
            Label: string
            IsRequired: bool
            Condition: Condition option
        }

        interface IField with
            member this.IsRequired = this.IsRequired
            member this.Condition = this.Condition

[<RequireQualifiedAccess>]
type Fields =
    | Input of Fields.Input
    | Checkbox of Fields.Checkbox

type Form =
    {
        // Id : Guid
        Title: string
        Fields: Fields list
    }

// type DesignerField =
//     {
//         Guid: Guid
//         RenderPropertyEditor: unit -> ReactElement
//         RenderDesignerPreview: unit -> ReactElement
//         Icon: string
//         Label: string
//     }

// let inputTextDesignField: DesignerField =
//     {
//         Guid = Guid.NewGuid()
//         RenderPropertyEditor = fun () -> Html.div [ prop.text "Properties editor for Input Text" ]
//         RenderDesignerPreview = fun () -> Html.div [ Html.text "Input Text preview" ]
//         Icon = "fas fa-font"
//         Label = "Input Text"
//     }

// let checkboxDesignField: DesignerField =
//     {
//         Guid = Guid.NewGuid()
//         RenderPropertyEditor = fun () -> Html.div [ prop.text "Properties editor for Checkbox" ]
//         RenderDesignerPreview = fun () -> Html.div [ Html.text "Checkbox preview" ]
//         Icon = "fas fa-check-square"
//         Label = "Checkbox"
//     }

module FilledFields =

    type Input =
        {
            Guid: Guid
            Value: string
        }

    type Checkbox =
        {
            Guid: Guid
            Value: bool
        }

[<RequireQualifiedAccess>]
type FilledFields =
    | Input of FilledFields.Input
    | Checkbox of FilledFields.Checkbox
    | NotFilled of Guid
