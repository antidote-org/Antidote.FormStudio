namespace Antidote.FormStudio.Compose

open Feliz
open System.Collections.Generic
open System
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.Core.FormProcessor
open Fable.Form.Antidote

module Types =

    type Severity = {
        WarningFlag: bool
        SeverityLabel: string
        SeverityColor: ScoreColor
    }

    type StepProgress =
        | ReadOnly
        | First
        | Middle
        | Last

    [<RequireQualifiedAccess>]
    type FormComposeMode =
        | Editable
        | ReadOnly

    type FormComposeState = {
        ResultViewMode: FormComposeMode
        FormSpec: FormSpec
        DynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>
        CurrentStep: int
        FormSaved: bool

    }

    type FormProcessor = {
        Result: int * bool -> ReactElement
        Calculator: FormComposeState -> int * bool
    }

    type Msg =
        | FormChanged of Fable.Form.Antidote.Form.View.Model<DynamicStepValues>
        | StepCompleted of (string * string ) list
        | Submit // of Form.View.Model<FormValues>
        | NextStep
        | PreviousStep
        | NavigateToStep of int
        // | GetFormSpec of string
        // | GotFormSpec of Form.Response.ReadFormSpec
        // | SavedForm of Form.Response.SaveFormValues
        // | ReadValue of Form.Response.ReadFormValues
        // | ErroredRequest of exn

    module Name =
        type T = private Name of string
        let create (value : string) = Name value
        let tryParse (value : string) = Ok (create value)
        let toString (Name value) = value

    module Subject =
        type T = private Subject of string
        let create (value : string) = Subject value
        let tryParse (value : string) = Ok (create value)
        let toString (Subject value) = value

    module Level =
        type T = private Level of string
        let create (value: string) = Level value
        let tryParse (value: string) = Ok (create value)
        let toString (Level value) = value


    type AssessmentTask = {
        Title: string
        Description: string
    }

    [<RequireQualifiedAccess>]
    type RenderingMode =
        | Warning of string
        | Normal of string
        | LargeWithSubtitle of string * string
