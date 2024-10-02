module Antidote.React.Components.FormWizard.Composer

open Fable.Form.Antidote
open Antidote.Core.FormProcessor
open Antidote.FormStudio.Compose.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec
open Antidote.FormStudio.i18n.Util
open Feliz

let merge fields : Form.Form<DynamicStepValues, _, IReactProperty> =
    // Produce a single merged form
    fields
    |> List.map (fun (name, form) ->
            // First, turn each individual form into one returning key-value pair
            Form.succeed (fun result ->  [name, result])
            |> Form.append (form)
        )
    |> List.reduce (fun form1 form2 ->
        // Merge forms by taking two, appending them and concatenating the
        // two lists of key-value pairs they produce (to get result of the same type)
        Form.succeed (fun r1 r2 ->  r1 @ r2 )
        |> Form.append form1
        |> Form.append form2)


let render model dispatch formAction fields =
    let config =
        Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = formAction
                Validation = Form.View.ValidateOnSubmit
            }

    let outForm = config fields model
    outForm

let compose readOnly (step: FormStep) =

    let dependencyMatch (dependsOnOpt: DependsOn option) field =
        let emptyForm = Form.succeed ""
        match dependsOnOpt with
        | Some dep ->
            Form.meta( fun (stepValues:DynamicStepValues) ->
                let dependsFieldValue = stepValues |> Map.tryFind (FieldKey dep.FieldKey)
                match dependsFieldValue with
                | Some fieldDetails ->
                    printfn $"Depends on field {dep.FieldKey} with value {fieldDetails}"
                    match fieldDetails.FieldValue with
                    | Multiple v ->
                        match dep.Evaluator with
                        | Evaluator.Equals ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value = dep.FieldValue)) then field else emptyForm
                        | Evaluator.NotEquals ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value <> dep.FieldValue)) then field else emptyForm
                        | Evaluator.GreaterThan ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value > dep.FieldValue)) then field else emptyForm
                        | Evaluator.GreaterThanOrEquals ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value >= dep.FieldValue)) then field else emptyForm
                        | Evaluator.LessThan ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value < dep.FieldValue)) then field else emptyForm
                        | Evaluator.LessThanOrEquals ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value <= dep.FieldValue)) then field else emptyForm
                        | Evaluator.Exists ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value <> "")) then field else emptyForm
                        | Evaluator.IsEmpty ->
                            if dep.FieldValue = "*" || ( v |> Set.exists (fun a -> a.Value = "")) then field else emptyForm
                    | Single v ->
                        // if dep.FieldValue = "*" || v.Value = dep.FieldValue then field else emptyForm
                        // if v.Value = dep.FieldValue then field else emptyForm
                        match dep.Evaluator with
                        | Evaluator.Equals ->
                            if dep.FieldValue = "*" || v.Value = dep.FieldValue then field else emptyForm
                        | Evaluator.NotEquals ->
                            if dep.FieldValue = "*" || v.Value <> dep.FieldValue then field else emptyForm
                        | Evaluator.GreaterThan ->
                            if dep.FieldValue = "*" || v.Value > dep.FieldValue then field else emptyForm
                        | Evaluator.GreaterThanOrEquals ->
                            if dep.FieldValue = "*" || v.Value >= dep.FieldValue then field else emptyForm
                        | Evaluator.LessThan ->
                            if dep.FieldValue = "*" || v.Value < dep.FieldValue then field else emptyForm
                        | Evaluator.LessThanOrEquals ->
                            if dep.FieldValue = "*" || v.Value <= dep.FieldValue then field else emptyForm
                        | Evaluator.Exists ->
                            if dep.FieldValue = "*" || v.Value <> "" then field else emptyForm
                        | Evaluator.IsEmpty ->
                            if dep.FieldValue = "*" || v.Value = "" then field else emptyForm
                | _ -> emptyForm
            )
        | None -> field

    let optionalMatch isOptional (field:Form.Form<DynamicStepValues,string,IReactProperty>) =
        if isOptional then
            field
            |> Form.optional
            |> Form.andThen ( function
                | Some v -> Form.succeed v
                | None -> Form.succeed ""
            )
        else
            field

    let emptyField = Form.succeed ""

    let mergedFields =
        step.Fields
        |> List.map(fun specField ->

            match specField.FieldType with
            // TODO: Unify these controls after extending the
            // FormSpec to include properties for determining the appropriate 'finder'
            // | AllergyFinder info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.AllergyFinder.AllergyFinder
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            // | CPTFinder info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.CPTFinder.CPTFinder
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            // | ICD10Finder info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.ICD10Finder.ICD10Finder
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            // | DrugFinder info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.DrugFinder.DrugFinder
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            // | DrugFinderWithFrequency info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.DrugFinder.DrugFinderWithFrequency
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            | SpeechToText info ->
                Form.reactComponentField
                    {
                        Parser = Ok
                        Value = (fun values -> readValue specField values)
                        Update = (fun a ->
                            printfn $"UPDATING TEXT TO SPEECH {a}"
                            updateSingleFunc id specField a
                        )
                        Error = fun _ -> None
                        Attributes =
                            {
                                Label = (t specField.Label)
                                Render = (
                                    //ASKMAXIME!!!!!!!!!
                                    fun (p:Field.ReactComponentField.ReactComponentFieldProps) ->
                                        let pp: Field.ReactComponentField.ReactComponentFieldProps = {
                                            OnChange = ( fun s ->
                                                printfn $"CHANGED TEXT TO SPEECH {s}"
                                                p.OnChange s
                                            )
                                            Disabled = p.Disabled
                                            Value = p.Value
                                        }
                                        Antidote.React.Components.SpeechToText.SpeechToText pp
                                )
                            }
                    }
                    |> Form.disableIf readOnly
                |> optionalMatch specField.IsOptional
                |> dependencyMatch specField.DependsOn
            // | EPrescribe info ->
            //     if specField.IsDeprecated && not readOnly
            //     then emptyField
            //     else
            //         Form.reactComponentField
            //             {
            //                 Parser = Ok
            //                 Value = (fun values -> readValue specField values)
            //                 Update = updateSingleFunc id specField
            //                 Error = fun _ -> None
            //                 Attributes =
            //                     {
            //                         Label = (t specField.Label)
            //                         Render = Antidote.React.Components.PhotonWebElements.ePrescribeComponent
            //                     }
            //             }
            //             |> Form.disableIf readOnly
            //         |> optionalMatch specField.IsOptional
            //         |> dependencyMatch specField.DependsOn
            | Image info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.takePhotoOrGetFromLibraryField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn
            | StateSelectorUSA info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.stateSelectorField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Id = specField.FieldKey
                                    Label = (t specField.Label)
                                    Options = Map.empty
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Text info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.textField
                        {
                            Parser = Ok
                            Value = (fun values -> Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.readValue specField values)
                            Update =  Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = ""
                                    HtmlAttributes = [ ]
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | TextArea info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.textareaField
                        {
                            Parser = Ok
                            Value = (fun values -> Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.readValue specField values)
                            Update =  Antidote.Core.FormProcessor.Helpers.v2_0_1.Spec.updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = ""
                                    HtmlAttributes = [ ]
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Message info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.reactComponentField {
                        Parser = Ok
                        Value = fun values -> readValue specField values
                        Update = updateSingleFunc id specField
                        Error = fun _ -> None
                        Attributes = {
                            Label = (t specField.Label)
                            Render = Antidote.React.Components.FormMessage.FormMessageField info
                        }
                    }
            | Signature info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.reactComponentField
                        {
                            Parser =
                                fun value ->
                                    if System.String.IsNullOrEmpty value then
                                        Result.Error "Signature is required"
                                    else
                                        value
                                        |> Ok
                            Value = fun values -> readValue specField values
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Render = Antidote.React.Components.SignatureCanvas.SignatureField (specField.FieldKey)
                                }
                        }
                        |> Form.disableIf readOnly
            | Time info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.timeField
                        {
                            Parser = Ok
                            Value = (fun values -> readValue specField values)
                            Update =  updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = ""
                                    HtmlAttributes = [ ]
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Date info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.dateField
                        {
                            Parser = Ok
                            Value = (fun values -> readValue specField values)
                            Update =  updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = ""
                                    HtmlAttributes = [ ]
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Tel info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.telField
                        {
                            Parser =
                                (fun v ->
                                    // match Antidote.Core.V2.Types.PhoneNumber.tryParse v with
                                    // | Ok tel -> Ok (Antidote.Core.V2.Types.PhoneNumber.toString tel)
                                    // | _ -> Result.Error ("Invalid phone number")
                                    Ok v
                                )
                            Value = (fun values -> readValue specField values)
                            // Update = updateSingleFunc Antidote.Core.V2.Normalizers.String.toPhoneFormat specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = "(000)-000-0000"
                                    HtmlAttributes = [ ]
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Number info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.telField
                        {
                            Parser = Ok
                            Value = (fun values -> readValue specField values)
                            // Update = updateSingleFunc Antidote.Core.V2.Normalizers.String.toNumberFormat specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = "0"
                                    HtmlAttributes = []
                                }
                        }
                        |> Form.disableIf readOnly
                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | SingleChoice info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.flatRadioField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = None
                                    Options =
                                        info.Options
                                        |> List.map (fun o -> o.Value, (t o.Description))
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | Dropdown info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.selectField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = Some (t Intl.SelectPlaceholder.Key)
                                    Options =
                                        info.Options
                                        |> List.map (fun o -> o.Value, (t o.Description))
                                    HtmlAttributes = [  ]
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | CheckboxList info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.checkboxListField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readManyValue (FieldKey specField.FieldKey)
                            Update = (fun newValue values -> updateManyFunc id specField newValue values)
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = None
                                    Options =
                                        info.Options
                                        |> List.map (fun o -> o.Value, (t o.Description ))
                                    Layout = Field.CheckboxListField.Layout.Vertical
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn


            | MultiChoice info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.flatCheckboxField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = readManyValue (FieldKey specField.FieldKey)
                            Update = (fun newValue values -> updateManyFunc id specField newValue values)
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = (t specField.Label)
                                    Placeholder = None
                                    Options =
                                        info.Options
                                        |> List.map (fun o -> o.Value, (t o.Description ))
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn


            | Radio info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.radioField
                        {
                            Parser = (fun a -> Ok (string a))
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = specField.Label
                                    Options =
                                        info.Options
                                        |> List.map (fun o -> o.Value, (t o.Description))
                                    Layout = Field.RadioField.Layout.Vertical

                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn


            | Checkbox info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.checkboxField
                        {
                            Parser = (fun a -> Ok (string a))
                            Value = (fun value -> snd (bool.TryParse (readValue specField value)))
                            Update = (fun value values -> updateSingleFunc id specField  (string value) values )
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Text = specField.Label
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn


            | YesNo info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.twoChoiceField
                        {
                            Parser = (fun value -> Ok value)
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Id = specField.Label
                                    Label = (t specField.Label)
                                    Options1 = "yes",(t "Yes")
                                    Options2 = "no", (t "No")
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | TrueFalse info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.twoChoiceField
                        {
                            Parser = (fun value -> Ok value)
                            Value = readValue specField
                            Update = updateSingleFunc id specField
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Id = specField.Label
                                    Label = specField.Label
                                    Options1 = "true",(t "True")
                                    Options2 = "false",(t "False")
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | TagList info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.tagListField
                        {
                            Parser = (fun value -> Ok "")
                            Value = (fun a -> readManyValue (FieldKey specField.FieldKey) a) //readManyValue specField
                            Update = (fun newValue values -> updateManyFunc id specField newValue values)
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Label = specField.Label
                                    Options = info.Options |> List.map (fun o -> o.Value, (t o.Description))
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn
            | Switch info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.switchField
                        {
                            Parser = (fun value -> Ok (string value))
                            Value = (fun value -> snd (bool.TryParse (readValue specField value)))
                            Update = (fun value values -> updateSingleFunc id specField  (string value) values )
                            Error = fun _ -> None
                            Attributes =
                                {
                                    Text = specField.Label
                                    Id = specField.FieldKey
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn

            | TextAutoComplete info ->
                if specField.IsDeprecated && not readOnly
                then emptyField
                else
                    Form.textAutocompleteField
                        {
                            Parser = Ok
                            Value = (fun a -> readValue specField a)
                            Update = (fun newValue values -> updateSingleFunc id specField newValue values)
                            Error =
                                fun _ -> None
                            Attributes =
                                {
                                    Label = specField.Label
                                    Placeholder = ""
                                    Possibilities =
                                        info.Options
                                        |> List.map (fun o ->  (t o.Description ))
                                }
                        }
                        |> Form.disableIf readOnly

                    |> optionalMatch specField.IsOptional
                    |> dependencyMatch specField.DependsOn




        )
        |> List.mapi ( fun i a -> string i, a )
        |> merge


    Form.succeed (fun result -> StepCompleted result)
    |> Form.append mergedFields
