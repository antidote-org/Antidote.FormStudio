module Antidote.FormDesigner.Types

open Feliz
open Feliz.Bulma
open Antidote.Core.FormProcessor.Spec.v2_0_1

type FieldState =
    | Idle
    | AddingDependantKeys

type ActiveField =
    {
        FormStepNumber: int
        FormFieldNumber: int
        State: FieldState
    }

type RenderPreviewProps =
    {
        FormSpec: FormSpec
        FormStep: FormStep
        FormField: FormField
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        OnChange: FormSpec -> unit
    }

// type FormStep<'FieldType> =
//     {
//         StepOrder: int
//         StepLabel: string
//         Fields: 'FieldType list // This is user domain
//     }

// type FormSpec<'FieldType> =
//     {
//         Title: string
//         Steps: FormStep<'FieldType> list
//     }

type IDesignerField =
    abstract Icon: string
    abstract Key: string
    abstract FieldType: FieldType
    abstract RenderPreview: RenderPreviewProps -> ReactElement
// abstract RenderEditor : obj
// abstract ToSpecs : FieldType

type RegisteredFields(fields: IDesignerField list) =
    let cachedMap = fields |> List.map (fun field -> field.Key, field) |> Map.ofList

    member _.AsList = fields

    member _.TryGetByKey key = cachedMap.TryFind key

    member _.GetByKey key = cachedMap[key]

    member _.AsMap = cachedMap

// Output of the designer
type FormSpecs =
    {
        Fields: FieldType list
    }

// Transformation FormSpecs to JSON
// Save it to the database

[<RequireQualifiedAccess>]
type DesignerFieldType =
    | Image
    | Message
    | Signature
    | SingleChoice
    | MultiChoice
    | Text
    | TextArea
    | Tel
    | Date
    | Time
    | Number
    | StateSelectorUSA
    | YesNo
    | TrueFalse
    | Radio
    | Checkbox
    | CheckboxList
    | Dropdown
    | TagList
    | Switch
    | TextAutoComplete
    | EPrescribe
    | SpeechToText
    | DrugFinder
    | DrugFinderWithFrequency
    // TODO: Unify these controls after extending the
    // FormSpec to include properties for determining the appropriate 'finder'
    | AllergyFinder
    | CPTFinder
    | ICD10Finder

    member x.Key =
        match x with
        | DrugFinder -> "Drug Finder"
        | DrugFinderWithFrequency -> "Drug Finder With Frequency"
        | SpeechToText -> "Speech To Text"
        | Image -> "Image"
        | Text -> "Text"
        | TextArea -> "Text Area"
        | Tel -> "Tel"
        | Date -> "Date"
        | Time -> "Time"
        | Number -> "Number"
        | SingleChoice -> "Single Choice"
        | MultiChoice -> "Multi Choice"
        | StateSelectorUSA -> "USA State Selector"
        | YesNo -> "Yes / No"
        | TrueFalse -> "True / False"
        | Message -> "Message"
        | Signature -> "Signature"
        | Radio -> "Radio"
        | Checkbox -> "Checkbox"
        | CheckboxList -> "Checkbox List"
        | Dropdown -> "Dropdown"
        | TagList -> "Tag List"
        | Switch -> "Switch"
        | TextAutoComplete -> "Text Auto Complete"
        | EPrescribe -> "ePrescribe"
        // TODO: Unify these controls after extending the
        // FormSpec to include properties for determining the appropriate 'finder'
        | AllergyFinder -> "Allergy Finder"
        | CPTFinder -> "CPT Finder"
        | ICD10Finder -> "ICD10 Finder"

    member x.Icon =
        match x with
        | DrugFinder -> "fas fa-prescription-bottle-alt"
        | DrugFinderWithFrequency -> "fas fa-file-prescription"
        | SpeechToText -> "fas fa-microphone"
        | Image -> "fas fa-image"
        | Message -> "fas fa-info"
        | Signature -> "fas fa-signature"
        | SingleChoice -> "fas fa-dot-circle"
        | MultiChoice -> "fas fa-check-square"
        | Text -> "fas fa-font"
        | TextArea -> "fas fa-font"
        | Tel -> "fas fa-phone"
        | Date -> "fas fa-calendar"
        | Time -> "fas fa-clock"
        | Number -> "fas fa-hashtag"
        | StateSelectorUSA -> "fas fa-map"
        | YesNo -> "fas fa-dot-circle"
        | TrueFalse -> "fas fa-dot-circle"
        | Radio -> "fas fa-dot-circle"
        | Checkbox -> "fas fa-check-square"
        | CheckboxList -> "fas fa-check-square"
        | Dropdown -> "fas fa-caret-down"
        | TagList -> "fas fa-tags"
        | Switch -> "fas fa-toggle-on"
        | TextAutoComplete -> "fas fa-font"
        | EPrescribe -> "fas fa-prescription"
        // TODO: Unify these controls after extending the
        // FormSpec to include properties for determining the appropriate 'finder'
        | AllergyFinder -> "fas fa-file-medical"
        | CPTFinder -> "fas fa-clipboard-list"
        | ICD10Finder -> "fas fa-list-alt"

let availableComponents =
    [
        DesignerFieldType.Image
        DesignerFieldType.Text
        DesignerFieldType.TextArea
        DesignerFieldType.Tel
        DesignerFieldType.Date
        DesignerFieldType.Time
        DesignerFieldType.Number
        DesignerFieldType.SingleChoice
        DesignerFieldType.MultiChoice
        DesignerFieldType.StateSelectorUSA
        DesignerFieldType.YesNo
        DesignerFieldType.TrueFalse
        DesignerFieldType.Signature
        DesignerFieldType.Radio
        DesignerFieldType.Checkbox
        DesignerFieldType.CheckboxList
        DesignerFieldType.Dropdown
        DesignerFieldType.TagList
        DesignerFieldType.Switch
        DesignerFieldType.TextAutoComplete
        DesignerFieldType.EPrescribe
        DesignerFieldType.SpeechToText
        // DesignerFieldType.DrugFinder
        DesignerFieldType.DrugFinderWithFrequency
        // TODO: Unify these controls after extending the
        // FormSpec to include properties for determining the appropriate 'finder'
        DesignerFieldType.AllergyFinder
        DesignerFieldType.CPTFinder
        DesignerFieldType.ICD10Finder
    // DesignerFieldType.Message
    ]

type TextFieldComponentProps =
    {
        FormSpec: FormSpec
        FormStep: FormStep
        FormField: FormField
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        OnChange: FormSpec -> unit
    }
