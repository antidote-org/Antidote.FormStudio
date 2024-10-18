module Antidote.FormStudio.Helper

open System
open Antidote.FormStudio.Types
open Fable.Form.Antidote

let defaultFormSpec<'UserField> : FormSpec<'UserField> =
    {
        Id = Guid.NewGuid()
        Code = None
        Title = ""
        Abstract = ""
        Version = "1.0.0"
        FormSpecVersion = "2.0.1"
        Steps =
            [
                {
                    StepOrder = 1
                    StepLabel = "First Step"
                    Fields = []
                }
            ]
        CategoryTags = []
        Score = None
        AssociatedCodes = []
    }

let getDesignerFieldType fieldName (registeredFields: RegisteredFields<'UserField>) =
    registeredFields.GetByKey fieldName

let severityColorToClasses severityColor =
    match severityColor with
    // | White -> "has-text-black", "has-background-white"
    // | Black -> "has-text-white", "has-background-black"
    // | Light -> "has-text-black", "has-background-light"
    // | Dark -> "has-text-white", "has-background-dark"
    // | Unspecified -> "has-text-white has-background-success-dark"
    // | Primary -> "has-text-white has-background-primary"
    // | Link -> "has-text-white has-background-link"
    // | Info -> "has-text-white has-background-info"
    // | Success -> "has-text-white has-background-success"
    // | Warning -> "has-text-grey-dark has-background-warning"
    // | Danger -> "has-text-white has-background-danger"
    | Unspecified -> ""
    | Primary -> "has-text-white has-background-primary"
    | Link -> "has-text-white has-background-link"
    | Info -> "has-text-white has-background-info"
    | Success -> "has-text-white has-background-success"
    | Warning -> "has-text-grey-dark has-background-warning"
    | Danger -> "has-text-white has-background-danger"

let defaultField
    (designerFieldType: IDesignerField<'UserField>)
    (fieldOrder: int)
    : FormField<'UserField>
    =
    {
        FieldType = designerFieldType.FieldType
        FieldKey = Guid.NewGuid().ToString()
        FieldOrder = fieldOrder
        Label = "Untitled " + designerFieldType.Key
        IsOptional = false
        IsDeprecated = false
        DependsOn = None
        DesignerFieldKey = designerFieldType.Key
    }

let updateFormFieldInFormSpecStep
    (inField: FormField<'UserField>)
    (formStep: FormStep<'UserField>)
    (formSpec: FormSpec<'UserField>)
    : FormSpec<'UserField>
    =
    let outStep =
        { formStep with
            Fields =
                formStep.Fields
                |> List.map (fun f ->
                    if f.FieldKey = inField.FieldKey then
                        inField
                    else
                        f
                )
        }

    let outFormSpec =
        { formSpec with
            Steps =
                formSpec.Steps
                |> List.map (fun s ->
                    if s.StepOrder = formStep.StepOrder then
                        outStep
                    else
                        s
                )
        }

    outFormSpec

let addFormFieldToStep
    (formStep: FormStep<'UserField>)
    (formSpec: FormSpec<'UserField>)
    (formField: FormField<'UserField>)
    : FormSpec<'UserField>
    =

    let outStep =
        { formStep with
            Fields =
                formStep.Fields
                @ [
                    formField
                ]
        }

    let newFormSpec =
        { formSpec with
            Steps =
                formSpec.Steps
                |> List.map (fun s ->
                    if s.StepOrder = formStep.StepOrder then
                        outStep
                    else
                        s
                )
        }

    newFormSpec

let insertFormFieldToStepAt (formStep: FormStep<'UserField>) newPositionFieldOrder fieldKey =
    formStep.Fields
    |> List.insertAt
        (newPositionFieldOrder - 1)
        (formStep.Fields |> List.find (fun f -> f.FieldKey = fieldKey))
    |> List.mapi (fun index f ->
        { f with
            FieldOrder = index + 1
        }
    )

let tryFindFormStepByStepNumber
    (stepNumber: int)
    (formSpec: FormSpec<'UserField>)
    : FormStep<'UserField> option
    =
    formSpec.Steps |> List.tryFind (fun step -> step.StepOrder = stepNumber)

let moveFormFieldUpInFormSpec
    (formStepOrder)
    (formField: FormField<'UserField>)
    (formSpec: FormSpec<'UserField>)
    =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun s ->
                if s.StepOrder = formStepOrder then
                    { s with
                        Fields =
                            s.Fields
                            |> List.map (fun f ->
                                if f.FieldOrder = formField.FieldOrder then
                                    { f with
                                        FieldOrder = formField.FieldOrder - 1
                                    }
                                else if f.FieldOrder = formField.FieldOrder - 1 then
                                    { f with
                                        FieldOrder = formField.FieldOrder
                                    }
                                else
                                    f
                            )
                            |> List.sortBy (fun f -> f.FieldOrder)
                            |> List.mapi (fun index f ->
                                { f with
                                    FieldOrder = index + 1
                                }
                            )
                    }
                else
                    s
            )
    }

let moveFormFieldDownInFormSpec
    (formStepOrder)
    (formField: FormField<'UserField>)
    (formSpec: FormSpec<'UserField>)
    =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun s ->
                if s.StepOrder = formStepOrder then
                    { s with
                        Fields =
                            s.Fields
                            |> List.map (fun f ->
                                if f.FieldOrder = formField.FieldOrder then
                                    { f with
                                        FieldOrder = formField.FieldOrder + 1
                                    }
                                else if f.FieldOrder = formField.FieldOrder + 1 then
                                    { f with
                                        FieldOrder = formField.FieldOrder
                                    }
                                else
                                    f
                            )
                            |> List.sortBy (fun f -> f.FieldOrder)
                            |> List.mapi (fun index f ->
                                { f with
                                    FieldOrder = index + 1
                                }
                            )
                    }
                else
                    s
            )
    }

let moveFieldByKeyToPositionInFormStepSpec
    fieldKey
    newPositionFieldOrder
    (formStepOrder)
    (formSpec: FormSpec<'UserField>)
    =
    let formStep = tryFindFormStepByStepNumber formStepOrder formSpec

    match formStep with
    | None -> formSpec
    | Some formStep ->
        { formSpec with
            Steps =
                formSpec.Steps
                |> List.map (fun s ->
                    if s.StepOrder = formStep.StepOrder then
                        { s with
                            Fields =
                                s.Fields
                                |> List.insertAt
                                    (newPositionFieldOrder - 1)
                                    (s.Fields |> List.find (fun f -> f.FieldKey = fieldKey))
                                |> List.distinct
                                |> List.mapi (fun index f ->
                                    { f with
                                        FieldOrder = index + 1
                                    }
                                )

                        }
                    else
                        s
                )
        }

let removeFormFieldFromFormSpec
    (formStepOrder: int)
    (formField: FormField<'UserField>)
    (formSpec: FormSpec<'UserField>)
    =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun s ->
                if s.StepOrder = formStepOrder then
                    { s with
                        Fields =
                            s.Fields
                            |> List.filter (fun f -> f.FieldOrder <> formField.FieldOrder)
                            |> List.mapi (fun i f ->
                                { f with
                                    FieldOrder = i + 1
                                }
                            )
                    }
                else
                    s
            )
    }

let moveFormStepUpInFormSpec (formStepOrder: int) (formSpec: FormSpec<'UserField>) =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun innerStep ->
                if innerStep.StepOrder = (formStepOrder - 1) then
                    { innerStep with
                        StepOrder = (formStepOrder + 1)
                    }

                else if innerStep.StepOrder = formStepOrder then
                    { innerStep with
                        StepOrder = formStepOrder - 1
                    }

                else
                    innerStep
            )
            |> List.sortBy (fun s -> s.StepOrder)
            |> List.mapi (fun i s ->
                { s with
                    StepOrder = i + 1
                }
            )
    }

let moveFormStepDownInFormSpec (formStepOrder: int) (formSpec: FormSpec<'UserField>) =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun innerStep ->
                if innerStep.StepOrder = (formStepOrder + 1) then
                    { innerStep with
                        StepOrder = (formStepOrder - 1)
                    }

                else if innerStep.StepOrder = formStepOrder then
                    { innerStep with
                        StepOrder = formStepOrder + 1
                    }

                else
                    innerStep
            )
            |> List.sortBy (fun s -> s.StepOrder)
            |> List.mapi (fun i s ->
                { s with
                    StepOrder = i + 1
                }
            )
    }

let removeFormStepFromFormSpec (formStepOrder: int) (formSpec: FormSpec<'UserField>) =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.filter (fun s -> s.StepOrder <> formStepOrder)
            |> List.mapi (fun i s ->
                { s with
                    StepOrder = i + 1
                }
            )
    }

let tryGetStepByNumber (stepNumber: int) (formSpec: FormSpec<'UserField>) =
    formSpec.Steps |> List.tryFind (fun s -> s.StepOrder = stepNumber)

let updateFormStepInFormSpec (formStep: FormStep<'UserField>) (formSpec: FormSpec<'UserField>) =
    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun s ->
                if s.StepOrder = formStep.StepOrder then
                    formStep
                else
                    s
            )
    }

let tryFindFieldInSpec
    (fieldNumber: int)
    (stepNumber: int)
    (formSpec: FormSpec<'UserField>)
    : FormField<'UserField> option
    =
    let stepOpt =
        formSpec.Steps |> List.tryFind (fun step -> step.StepOrder = stepNumber)

    match stepOpt with
    | None -> None
    | Some step ->
        let fieldOpt =
            step.Fields |> List.tryFind (fun field -> field.FieldOrder = fieldNumber)

        match fieldOpt with
        | None -> None
        | Some field -> Some field

let getDefaultFormFieldByDesignerFieldType fieldOrder (fieldDesigner: IDesignerField<'UserField>) =
    defaultField fieldDesigner fieldOrder

let insertDesignerFieldTypeToStepAt
    (source: string)
    (stepNumber: int)
    (at: int)
    (formSpec: FormSpec<'UserField>)
    (registeredFields: RegisteredFields<'UserField>)
    : FormSpec<'UserField>
    =
    let step = formSpec.Steps |> List.find (fun step -> step.StepOrder = stepNumber)

    let ff =
        registeredFields.GetByKey source |> getDefaultFormFieldByDesignerFieldType at

    let newStep =
        { step with
            Fields = step.Fields |> List.insertAt (at - 1) ff
        }

    { formSpec with
        Steps =
            formSpec.Steps
            |> List.map (fun s ->
                if s.StepOrder = stepNumber then
                    newStep
                else
                    s
            )
    }

let addDesignerFieldTypeToStep
    (source: string)
    (stepNumber: int)
    (formSpec: FormSpec<'UserField>)
    (registeredFields: RegisteredFields<'UserField>)
    : FormSpec<'UserField>
    =

    let formStep = formSpec.Steps |> List.find (fun step -> step.StepOrder = stepNumber)

    registeredFields.GetByKey source
    |> getDefaultFormFieldByDesignerFieldType (formStep.Fields.Length + 1)
    |> addFormFieldToStep formStep formSpec

[<RequireQualifiedAccess>]
type DragSource =
    | Designer_FormFieldType_Key of string
    | Designer_FormField_FieldKey of string

    member x.ToPlainText =
        match x with
        | Designer_FormFieldType_Key data -> $"DESIGNER_FORMFIELDTYPE_KEY:{data}"
        | Designer_FormField_FieldKey data -> $"DESIGNER_FORMFIELD_FIELDKEY:{data}"

let tryGetDragSourceFromData (data: string) =
    let dataParts = data.Split(':')

    if dataParts.Length <> 2 then
        failwith $"The drag/drop data *must be* two parts"
    else
        match dataParts[0] with
        | "DESIGNER_FORMFIELDTYPE_KEY" -> DragSource.Designer_FormFieldType_Key
        | "DESIGNER_FORMFIELD_FIELDKEY" -> DragSource.Designer_FormField_FieldKey
        | _ -> failwith "Unsuported drag/drop source"
        |> fun x -> Some(x dataParts[1])

let flattenSpecSteps formSpec : FormSpec<'UserField> =
    let rec flatten (steps: FormStep<'UserField> list) (acc: FormField<'UserField> list) =
        match steps with
        | [] -> acc
        | step :: rest ->
            let reorderedFields =
                step.Fields
                |> List.map (fun field ->
                    { field with
                        FieldOrder = step.StepOrder * 1000 + field.FieldOrder
                    }
                )

            flatten rest (acc @ reorderedFields)

    { formSpec with
        Steps =
            [
                {
                    StepOrder = 1
                    StepLabel = "Complete Form"
                    Fields = flatten formSpec.Steps []
                }
            ]
    }
    : FormSpec<'UserField>

let flattenFormSteps
    (dynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>)
    : DynamicForm<Form.View.Model<DynamicStepValues>>
    =
    let flatForm =
        dynamicForm.Steps
        |> Map.fold (fun acc k v -> (v.Values |> Map.toList) @ acc) []
        |> Map.ofList
        |> Form.View.idle
        |> fun x -> Map[StepOrder 1, x]

    { dynamicForm with
        Steps = flatForm
    }

let extractDataFromFableFormsModel
    (dynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>)
    : DynamicFormResultData
    =
    {
        ResultFormSpecDetails = dynamicForm.DynamicFormSpecDetails
        ResultSteps =
            dynamicForm.Steps
            |> Map.map (fun key dynamicStepValues -> dynamicStepValues.Values)
    }
