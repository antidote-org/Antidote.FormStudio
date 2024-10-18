module Antidote.FormStudio.UI.StepBreak

open Feliz
open Fable.Core.JsInterop
open Antidote.FormStudio
open Antidote.FormStudio.Types
open System
open Antidote.FormStudio.UI.Components.ButtonMinimal

let private classes: CssModules.UI.StepBreak =
    import "default" "./StepBreak.module.scss"

type StepBreakProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        AddStep: unit -> unit
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let StepBreak (props: StepBreakProps<'UserField>) =
    Html.div [
        prop.className [
            classes.``step-break``
            if props.ActiveField.State = AddingDependantKeys then
                GlobalCSS.classes.disabled
            else
                ""
        ]
        prop.children [
            ButtonMinimal
                {|
                    Text = "+ ADD STEP"
                    OnClick = fun () -> props.AddStep()
                |}
        ]
    ]
