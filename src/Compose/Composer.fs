module Antidote.React.Components.FormWizard.Composer

open Feliz
open Fable.Form.Simple
open Fable.Form.Simple.Bulma
open Antidote.FormStudio.Compose.Types
open Antidote.FormStudio.Types

let merge fields : Fable.Form.Simple.Bulma.Form<DynamicStepValues, (string * string) list> =
    // Produce a single merged form
    fields
    |> List.map (fun (name, form) ->
        // First, turn each individual form into one returning key-value pair
        Fable.Form.Simple.Bulma.Form.succeed (fun result ->
            [
                name, result
            ]
        )
        |> Fable.Form.Simple.Bulma.Form.append (form)
    )
    |> List.reduce (fun form1 form2 ->
        // Merge forms by taking two, appending them and concatenating the
        // two lists of key-value pairs they produce (to get result of the same type)
        Fable.Form.Simple.Bulma.Form.succeed (fun r1 r2 -> r1 @ r2)
        |> Fable.Form.Simple.Bulma.Form.append form1
        |> Fable.Form.Simple.Bulma.Form.append form2
    )

let render
    (model: Fable.Form.Simple.Form.View.Model<DynamicStepValues>)
    (onChange: Fable.Form.Simple.Form.View.Model<DynamicStepValues> -> unit)
    (onSubmit: (string * string) list -> unit)
    (formAction: string)
    (fields: Fable.Form.Simple.Bulma.Form<DynamicStepValues, (string * string) list>)
    =

    // Handle form state changes - extract result when form succeeds
    let handleChange (newModel: Fable.Form.Simple.Form.View.Model<DynamicStepValues>) =
        onChange newModel
        // When form succeeds, extract the result from the form values
        match newModel.State with
        | Fable.Form.Simple.Form.View.Success _ ->
            // Extract result from the form's values - convert DynamicStepValues to (string * string) list
            let result =
                newModel.Values
                |> Map.toList
                |> List.map (fun (FieldKey key, details) ->
                    match details.FieldValue with
                    | Single v -> (key, v.Value)
                    | Multiple vs ->
                        // For multiple values, join them or take first
                        match vs |> Set.toList with
                        | [] -> (key, "")
                        | v :: _ -> (key, v.Value)
                )

            onSubmit result
        | _ -> ()

    Fable.Form.Simple.Bulma.Form.View.asHtml
        {
            OnChange = handleChange
            OnSubmit = onSubmit
            Action = Fable.Form.Simple.Form.View.Action.SubmitOnly formAction
            Validation = Fable.Form.Simple.Form.View.ValidateOnSubmit
        }
        fields
        model

let compose (readOnly: bool) renderUserField (step: FormStep<'UserField>) =

    let dependencyMatch (dependsOnOpt: DependsOn option) field =
        let emptyForm = Fable.Form.Simple.Bulma.Form.succeed ""

        match dependsOnOpt with
        | Some dep ->
            Fable.Form.Simple.Bulma.Form.meta (fun (stepValues: DynamicStepValues) ->
                let dependsFieldValue = stepValues |> Map.tryFind (FieldKey dep.FieldKey)

                match dependsFieldValue with
                | Some fieldDetails ->
                    printfn $"Depends on field {dep.FieldKey} with value {fieldDetails}"

                    match fieldDetails.FieldValue with
                    | Multiple v ->
                        match dep.Evaluator with
                        | Evaluator.Equals ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value = dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.NotEquals ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value <> dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.GreaterThan ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value > dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.GreaterThanOrEquals ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value >= dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.LessThan ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value < dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.LessThanOrEquals ->
                            if
                                dep.FieldValue = "*"
                                || (v |> Set.exists (fun a -> a.Value <= dep.FieldValue))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.Exists ->
                            if
                                dep.FieldValue = "*" || (v |> Set.exists (fun a -> a.Value <> ""))
                            then
                                field
                            else
                                emptyForm
                        | Evaluator.IsEmpty ->
                            if
                                dep.FieldValue = "*" || (v |> Set.exists (fun a -> a.Value = ""))
                            then
                                field
                            else
                                emptyForm
                    | Single v ->
                        // if dep.FieldValue = "*" || v.Value = dep.FieldValue then field else emptyForm
                        // if v.Value = dep.FieldValue then field else emptyForm
                        match dep.Evaluator with
                        | Evaluator.Equals ->
                            if dep.FieldValue = "*" || v.Value = dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.NotEquals ->
                            if dep.FieldValue = "*" || v.Value <> dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.GreaterThan ->
                            if dep.FieldValue = "*" || v.Value > dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.GreaterThanOrEquals ->
                            if dep.FieldValue = "*" || v.Value >= dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.LessThan ->
                            if dep.FieldValue = "*" || v.Value < dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.LessThanOrEquals ->
                            if dep.FieldValue = "*" || v.Value <= dep.FieldValue then
                                field
                            else
                                emptyForm
                        | Evaluator.Exists ->
                            if dep.FieldValue = "*" || v.Value <> "" then
                                field
                            else
                                emptyForm
                        | Evaluator.IsEmpty ->
                            if dep.FieldValue = "*" || v.Value = "" then
                                field
                            else
                                emptyForm
                | _ -> emptyForm
            )
        | None -> field

    let mergedFields =
        step.Fields
        |> List.map (fun specField -> renderUserField readOnly dependencyMatch specField)
        |> List.mapi (fun i a -> string i, a)
        |> merge

    Fable.Form.Simple.Bulma.Form.succeed (fun result -> result)
    |> Fable.Form.Simple.Bulma.Form.append mergedFields
