module Antidote.FormStudio.Specification

open System
open Fable.Core

/// <summary>
/// Represent the value of a field at the view level (when the form is being filled)
///
/// This type is also used inside the condition engine
/// </summary>
[<RequireQualifiedAccess>]
type FormValues =
    | Input of string
    | Checkbox of bool

type Condition = obj

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
            Condition: Condition option
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
