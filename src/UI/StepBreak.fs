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
        OnChange: FormSpec<'UserField> -> unit
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
                    OnClick =
                        fun () ->
                            //add new step
                            let newStep =
                                {
                                    StepOrder = props.FormSpec.Steps |> List.length
                                    StepLabel = "New Step"
                                    Fields =
                                        [
                                            {
                                                FieldOrder = 0
                                                FieldKey = Guid.NewGuid().ToString()
                                                FieldType = null
                                                // TODO: Expose empty field type for step
                                                // FieldType.Text
                                                //     {
                                                //         Value = None
                                                //     }
                                                Label = "New Field"
                                                IsOptional = false
                                                IsDeprecated = false
                                                // Flags = []
                                                DependsOn = None
                                            }
                                        ]
                                }

                            let outFormSpec =
                                { props.FormSpec with
                                    Steps =
                                        props.FormSpec.Steps
                                        @ [
                                            newStep
                                        ]
                                }

                            props.OnChange outFormSpec
                |}
        ]
    ]
