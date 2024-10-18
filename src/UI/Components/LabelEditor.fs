module Antidote.FormStudio.UI.Components.LabelEditor

open System
open Feliz
open Feliz.Bulma
open Antidote.FormStudio
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper

type LabelEditProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        FormField: FormField<'UserField>
        OnChange: FormSpec<'UserField> -> unit
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let LabelEditor (props: LabelEditProps<'UserField>) =
    if props.ActiveField.State = AddingDependantKeys then
        Html.p [
            prop.text props.FormField.Label
        ]
    else
        Bulma.input.text [
            if not (String.IsNullOrWhiteSpace(props.FormField.Label)) then
                prop.className GlobalCSS.classes.``input-as-label``
            else
                color.isDanger

            prop.onChange (fun (newValue: string) ->
                let newFormField =
                    { props.FormField with
                        Label = newValue
                    }

                let outFormSpec =
                    props.FormSpec |> updateFormFieldInFormSpecStep newFormField props.FormStep

                props.OnChange outFormSpec
            )
            prop.value props.FormField.Label
        ]
