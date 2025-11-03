module Antidote.React.Components.FormWizard.Composer

// open Fable.Form.Antidote
open Antidote.FormStudio.Compose.Types
open Antidote.FormStudio.Types
open Feliz

let merge fields : Fable.Form.Simple.Form.Form<DynamicStepValues, _, IReactProperty> =
    // Produce a single merged form
    fields
    |> List.map (fun (name, form) ->
        // First, turn each individual form into one returning key-value pair
        Fable.Form.Simple.Form.succeed (fun result ->
            [
                name, result
            ]
        )
        |> Fable.Form.Simple.Form.append (form)
    )
    |> List.reduce (fun form1 form2 ->
        // Merge forms by taking two, appending them and concatenating the
        // two lists of key-value pairs they produce (to get result of the same type)
        Fable.Form.Base.succeed (fun r1 r2 -> r1 @ r2)
        |> Fable.Form.Base.append form1
        |> Fable.Form.Base.append form2
    )

let render
    (model: Fable.Form.Simple.Form.View.Model<DynamicStepValues>)
    (dispatch: Msg -> unit)
    (formAction: string)
    (fields: Fable.Form.Simple.Form.Form<DynamicStepValues, Msg, IReactProperty>)
    =

    let config =
        Fable.Form.Simple.Bulma.Form.View.asHtml
            {
                Dispatch = dispatch
                OnChange = FormChanged
                Action = formAction
                Validation = Fable.Form.Simple.Form.View.ValidateOnSubmit
            }

    let outForm = config fields model
    outForm

let compose (readOnly: bool) renderUserField (step: FormStep<'UserField>) =

    let dependencyMatch (dependsOnOpt: DependsOn option) field =
        let emptyForm = Fable.Form.Base.succeed ""

        match dependsOnOpt with
        | Some dep ->
            Fable.Form.Base.meta (fun (stepValues: DynamicStepValues) ->
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

    Fable.Form.Base.succeed (fun result -> StepCompleted result)
    |> Fable.Form.Base.append mergedFields
