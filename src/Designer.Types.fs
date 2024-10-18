module Antidote.FormStudio.Types

open Feliz
// open Antidote.Core.FormProcessor.Spec.v2_0_1

type FieldState =
    | Idle
    | AddingDependantKeys

type ActiveField =
    {
        FormStepNumber: int
        FormFieldNumber: int
        State: FieldState
    }

[<RequireQualifiedAccess>]
type Evaluator =
    | Equals
    | NotEquals
    | GreaterThan
    | GreaterThanOrEquals
    | LessThan
    | LessThanOrEquals
    | Exists
    | IsEmpty

    member x.Key =
        match x with
        | Equals -> "Equals"
        | NotEquals -> "Not Equals"
        | GreaterThan -> "Greater Than"
        | GreaterThanOrEquals -> "Greater Than Or Equals"
        | LessThan -> "Less Than"
        | LessThanOrEquals -> "Less Than Or Equals"
        | Exists -> "Exists"
        | IsEmpty -> "Is Empty"

let tryEvaluationKeyToEvaluation str : Evaluator option =
    match str with
    | "Equals" -> Some Evaluator.Equals
    | "Not Equals" -> Some Evaluator.NotEquals
    | "Greater Than" -> Some Evaluator.GreaterThan
    | "Greater Than Or Equals" -> Some Evaluator.GreaterThanOrEquals
    | "Less Than" -> Some Evaluator.LessThan
    | "Less Than Or Equals" -> Some Evaluator.LessThanOrEquals
    | "Exists" -> Some Evaluator.Exists
    | "Is Empty" -> Some Evaluator.IsEmpty
    | _ -> None

let evaluators =
    [
        Evaluator.Equals
        Evaluator.NotEquals
        Evaluator.GreaterThan
        Evaluator.GreaterThanOrEquals
        Evaluator.LessThan
        Evaluator.LessThanOrEquals
        Evaluator.Exists
        Evaluator.IsEmpty
    ]

type DependsOn =
    {
        FieldKey: string
        FieldValue: string
        Evaluator: Evaluator
    }

type FormField<'UserField> =
    {
        FieldOrder: int
        FieldKey: string
        Label: string
        DependsOn: DependsOn option
        IsOptional: bool
        IsDeprecated: bool
        FieldType: 'UserField
    }

type FormStep<'UserField> =
    {
        StepOrder: int
        StepLabel: string
        Fields: FormField<'UserField> list
    }

type ScoreColor =
    // | White
    // | Black
    // | Light
    // | Dark
    | Unspecified
    | Primary
    | Link
    | Info
    | Success
    | Warning
    | Danger

type ScoreRange =
    {
        Id: System.Guid
        Min: int
        Max: int
        Label: string
        Tag: ScoreColor
    }

type Score =
    {
        MaxScore: int
        ScoreRanges: ScoreRange list
    }

type CategoryTag =
    | MentalHealth
    | IllicitDrugs
    | Alcohol
    | RiskScore
    | COPD
    | HRA
    | HeartDisease
    | GeneralWellness
    | KidneyDisease
    | Diabetes
    | Hospital
    | MedAdherance

    member x.toString =
        match x with
        | MentalHealth -> "Mental Health"
        | IllicitDrugs -> "Illicit Drugs"
        | Alcohol -> "Alcohol"
        | RiskScore -> "Risk Score"
        | COPD -> "COPD"
        | HRA -> "HRA"
        | HeartDisease -> "Heart Disease"
        | GeneralWellness -> "General Wellness"
        | KidneyDisease -> "Kidney Disease"
        | Diabetes -> "Diabetes"
        | Hospital -> "Hospital"
        | MedAdherance -> "Med Adherance"

    static member fromString(s: string) =
        match s with
        | "Mental Health" -> MentalHealth
        | "Illicit Drugs" -> IllicitDrugs
        | "Alcohol" -> Alcohol
        | "Risk Score" -> RiskScore
        | "COPD" -> COPD
        | "HRA" -> HRA
        | "Heart Disease" -> HeartDisease
        | "General Wellness" -> GeneralWellness
        | "Kidney Disease" -> KidneyDisease
        | "Diabetes" -> Diabetes
        | "Hospital" -> Hospital
        | "Med Adherance" -> MedAdherance
        | _ -> failwithf "Unknown category tag: %s" s

type FormSpec<'UserField> =
    {
        Id: System.Guid
        Code: string option
        Title: string
        Abstract: string
        Version: string
        FormSpecVersion: string
        Steps: FormStep<'UserField> list
        CategoryTags: CategoryTag list
        Score: Score option
        AssociatedCodes: string list
    }

type RenderPreviewProps<'UserField> =
    {
        FormSpec: FormSpec<'UserField>
        FormStep: FormStep<'UserField>
        FormField: FormField<'UserField>
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        OnChange: FormSpec<'UserField> -> unit
    }

type FieldAnswer =
    {
        FieldKey: string
        Description: string
        Value: string
    }

type FieldOption =
    {
        OptionKey: string
        Description: string
        Value: string
    }

type FieldValue =
    | Single of FieldAnswer
    | Multiple of Set<FieldAnswer>

type StepOrder = StepOrder of int
type FieldKey = FieldKey of string

type FieldDetails =
    {
        FieldOrder: int
        Key: FieldKey
        Label: string
        FieldValue: FieldValue
        Options: FieldOption list
    }

type DynamicStepValues = Map<FieldKey, FieldDetails>

type DynamicFormSpecDetails =
    {
        FormSpecId: System.Guid
        FormSpecCode: string option
        FormSpecTitle: string
        FormSpecAbstract: string
        FormSpecVersion: string
        DynamicVersion: string
        MaxScore: Score option
    }

type DynamicForm<'FableFormModel> =
    {
        DynamicFormSpecDetails: DynamicFormSpecDetails
        Steps: Map<StepOrder, 'FableFormModel>
    }

type DynamicFormResultData =
    {
        ResultFormSpecDetails: DynamicFormSpecDetails
        ResultSteps: Map<StepOrder, DynamicStepValues>
    }

type IDesignerField<'UserField> =
    abstract Icon: string
    abstract Key: string
    abstract FieldType: 'UserField //FormField<'UserField>
    abstract RenderPreview: RenderPreviewProps<'UserField> -> ReactElement

type RegisteredFields<'UserField>(fields: IDesignerField<'UserField> list) =
    let cachedMap = fields |> List.map (fun field -> field.Key, field) |> Map.ofList

    member _.AsList = fields

    member _.TryGetByKey key = cachedMap.TryFind key

    member _.GetByKey key = cachedMap[key]

    member _.AsMap = cachedMap
