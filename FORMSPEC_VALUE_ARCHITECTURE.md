# FormSpec and Form Values Architecture Documentation

## Overview

This proof-of-concept demonstrates a **generic, type-safe architecture** for handling dynamic form specifications and their associated values using Fable.Form. The entire system is built around **generic relationships** between `FormSpec<'UserField>` and Fable.Form values, allowing complete type safety while maintaining flexibility.

## Table of Contents

1. [Core Architecture](#core-architecture)
2. [Type System](#type-system)
3. [Value Flow](#value-flow)
4. [Form Composition](#form-composition)
5. [Value Management](#value-management)
6. [Key Patterns](#key-patterns)
7. [Implementation Examples](#implementation-examples)

---

## Core Architecture

### The Generic Relationship

The foundation of this architecture is the **generic relationship** between:

```
FormSpec<'UserField> → DynamicForm<Form.View.Model<DynamicStepValues>>
```

Where:
- **`FormSpec<'UserField>`** - The **schema/definition** of the form (structure, fields, validation rules)
- **`DynamicForm<'FableFormModel>`** - The **runtime state** containing actual form values
- **`'UserField`** - A **generic type parameter** that represents domain-specific field types

### Key Principle

> **The FormSpec is generic over field types, but the value storage is unified through `DynamicStepValues`**

This means:
- Different applications can define their own `'UserField` types (e.g., `FieldType.Text`, `FieldType.Checkbox`, `FieldType.Date`)
- All form values are stored in the same structure: `DynamicStepValues`
- The system bridges between generic field types and unified value storage

---

## Type System

### 1. FormSpec Types (Schema Definition)

#### `FormSpec<'UserField>`
```fsharp
type FormSpec<'UserField> =
    {
        Id: System.Guid
        Code: string option
        Title: string
        Abstract: string
        Version: string
        FormSpecVersion: string
        Steps: FormStep<'UserField> list  // ← Generic over UserField
        CategoryTags: CategoryTag list
        Score: Score option
        AssociatedCodes: string list
    }
```

**Purpose**: Defines the **structure** and **metadata** of a form. The `'UserField` type parameter allows different applications to define their own field types.

#### `FormStep<'UserField>`
```fsharp
type FormStep<'UserField> =
    {
        StepOrder: int
        StepLabel: string
        Fields: FormField<'UserField> list  // ← Generic over UserField
    }
```

**Purpose**: Represents a single step/page in a multi-step form.

#### `FormField<'UserField>`
```fsharp
type FormField<'UserField> =
    {
        FieldOrder: int
        FieldKey: string              // Unique identifier for the field
        Label: string
        DependsOn: DependsOn option   // Conditional field visibility
        IsOptional: bool
        IsDeprecated: bool
        FieldType: 'UserField         // ← Generic field type
        DesignerFieldKey: string
    }
```

**Purpose**: Defines a single field in the form. The `FieldType: 'UserField` is where domain-specific types are stored.

### 2. Value Types (Runtime State)

#### `DynamicStepValues` (The Core Value Storage)
```fsharp
type FieldKey = FieldKey of string

type FieldValue =
    | Single of FieldAnswer
    | Multiple of Set<FieldAnswer>

type FieldDetails =
    {
        FieldOrder: int
        Key: FieldKey
        Label: string
        FieldValue: FieldValue      // ← Actual value storage
        Options: FieldOption list
    }

type DynamicStepValues = Map<FieldKey, FieldDetails>
```

**Purpose**: **Unified value storage** that works for ALL field types. This is the bridge between generic `FormSpec<'UserField>` and concrete values.

**Key Insight**: 
- `FormSpec<'UserField>` uses generic types (`FieldType.Text`, `FieldType.Checkbox`, etc.)
- `DynamicStepValues` stores values as `FieldValue` (Single/Multiple of `FieldAnswer`)
- The conversion happens in `RenderUserField` functions

#### `DynamicForm<'FableFormModel>`
```fsharp
type DynamicForm<'FableFormModel> =
    {
        DynamicFormSpecDetails: DynamicFormSpecDetails
        Steps: Map<StepOrder, 'FableFormModel>  // ← Generic over FableFormModel
    }
```

**Purpose**: Contains all form state. In practice, `'FableFormModel` is `Form.View.Model<DynamicStepValues>`.

#### `Form.View.Model<DynamicStepValues>`
```fsharp
// From Fable.Form.Antidote
type Model<'Values> =
    {
        State: Form.View.State
        Values: 'Values              // ← DynamicStepValues
        Errors: Form.View.Error list
    }
```

**Purpose**: Fable.Form's model type that wraps `DynamicStepValues` with validation state.

### 3. The Bridge Functions

#### `RenderUserField` Function Signature
```fsharp
type ComposerFunc =
    DependsOn option
        -> Form.Form<DynamicStepValues, string, IReactProperty>
        -> Form.Form<DynamicStepValues, string, IReactProperty>

type RenderUserField =
    bool                                    // readOnly
    -> ComposerFunc                         // dependencyMatch
    -> FormField<'UserField>                // specField
    -> Form.Form<DynamicStepValues, string, IReactProperty>
```

**Purpose**: **Converts** a `FormField<'UserField>` (from FormSpec) into a `Form.Form<DynamicStepValues, ...>` (Fable.Form field).

---

## Value Flow

### Complete Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. FormSpec<'UserField>                                         │
│    - Defines structure with generic field types                 │
│    - Example: FieldType.Text { Value = None }                  │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 2. FormCompose.init()                                          │
│    - Creates DynamicForm<Form.View.Model<DynamicStepValues>>  │
│    - Initializes empty Map<StepOrder, Form.View.Model<...>>    │
│    - Each step starts with Form.View.idle Map.empty            │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 3. Composer.compose()                                          │
│    - Takes FormStep<'UserField>                                │
│    - Calls RenderUserField for each field                      │
│    - Merges all fields into single Form.Form<DynamicStepValues> │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 4. RenderUserField (Bridge Function)                          │
│    - Converts FormField<'UserField> → Form.Form<DynamicStepValues> │
│    - Uses Value function to READ from DynamicStepValues        │
│    - Uses Update function to WRITE to DynamicStepValues        │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 5. User Interaction                                            │
│    - User types in field                                       │
│    - Fable.Form calls Update function                          │
│    - Update modifies DynamicStepValues Map                     │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 6. FormChanged Message                                         │
│    - New Form.View.Model<DynamicStepValues> created           │
│    - Updates DynamicForm.Steps map                            │
│    - Calls FormChanged callback                               │
└─────────────────┬───────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────────┐
│ 7. Value Extraction                                            │
│    - extractDataFromFableFormsModel()                          │
│    - Converts DynamicForm → DynamicFormResultData             │
│    - Returns Map<StepOrder, DynamicStepValues>                │
└─────────────────────────────────────────────────────────────────┘
```

---

## Form Composition

### Step-by-Step Composition Process

#### 1. Initialization (`dynamicFormInit`)

```fsharp
let dynamicFormInit
    (dynamicStepValuesOpt: DynamicStepValues option)
    (formSpec: FormSpec<'UserField>)
    =
    let dynamicStepValues: DynamicStepValues =
        match dynamicStepValuesOpt with
        | Some dynamicStepValues -> dynamicStepValues
        | None -> Map.empty  // ← Starts empty

    let output: DynamicForm<Form.View.Model<DynamicStepValues>> =
        {
            Steps =
                [1 .. formSpec.Steps.Length]
                |> List.map (fun step -> 
                    StepOrder step, 
                    dynamicStepValues |> Form.View.idle  // ← Idle state with empty map
                )
                |> Map.ofList
            DynamicFormSpecDetails = { ... }
        }
    output
```

**Key Points**:
- Creates one entry in `Steps` map per step in FormSpec
- Each step starts with `Form.View.idle Map.empty`
- Can optionally initialize with existing values

#### 2. Field Composition (`Composer.compose`)

```fsharp
let compose (readOnly: bool) renderUserField (step: FormStep<'UserField>) =
    let mergedFields =
        step.Fields
        |> List.map (fun specField -> 
            renderUserField readOnly dependencyMatch specField
        )
        |> List.mapi (fun i a -> string i, a)
        |> merge  // ← Merges all fields into single form

    Form.succeed (fun result -> StepCompleted result) 
    |> Form.append mergedFields
```

**Key Points**:
- Iterates over each `FormField<'UserField>` in the step
- Calls `renderUserField` to convert each field to `Form.Form<DynamicStepValues, ...>`
- Uses `merge` function to combine all fields into one form
- Returns form that produces `(string * string) list` on completion

#### 3. Field Merging (`merge`)

```fsharp
let merge fields : Form.Form<DynamicStepValues, _, IReactProperty> =
    fields
    |> List.map (fun (name, form) ->
        // Wrap each form to return key-value pair
        Form.succeed (fun result -> [name, result])
        |> Form.append (form)
    )
    |> List.reduce (fun form1 form2 ->
        // Merge by concatenating key-value lists
        Form.succeed (fun r1 r2 -> r1 @ r2) 
        |> Form.append form1 
        |> Form.append form2
    )
```

**Key Points**:
- Each field form is wrapped to return `(string * string) list`
- Forms are merged by concatenating their result lists
- Final form returns all field values as a list

---

## Value Management

### Reading Values (`Value` Function)

The `Value` function is used by Fable.Form to **read** the current value from `DynamicStepValues`:

```fsharp
// Example from demo/Main.fs
Form.textField
    {
        Parser = Ok
        Value = fun values -> Helpers.readValue specField values  // ← READ
        Update = Helpers.updateSingleFunc id specField            // ← WRITE
        Error = fun _ -> None
        Attributes = { ... }
    }
```

**Implementation**:
```fsharp
let readValue (field: FormField<FieldType>) (values: DynamicStepValues) : string =
    match values.Keys |> Seq.tryFind (fun k -> k = (FieldKey field.FieldKey)) with
    | None -> ""  // ← No value yet
    | Some key ->
        let fv = values.Item key
        match fv.FieldValue with
        | Single v -> v.Value      // ← Extract value
        | _ -> ""
```

**Flow**:
1. Fable.Form calls `Value` with current `DynamicStepValues` map
2. Function looks up field by `FieldKey field.FieldKey`
3. Extracts value from `FieldValue.Single` or `FieldValue.Multiple`
4. Returns string value (or empty if not found)

### Writing Values (`Update` Function)

The `Update` function is used by Fable.Form to **write** new values to `DynamicStepValues`:

```fsharp
// Example from demo/Main.fs
Update = Helpers.updateSingleFunc id specField
```

**Implementation**:
```fsharp
let updateSingleFunc
    formatter
    (specField: FormField<FieldType>)
    (newValue: string)
    (values: DynamicStepValues)
    : DynamicStepValues
    =
    let newFieldDetails: FieldDetails =
        {
            FieldOrder = specField.FieldOrder
            Key = FieldKey specField.FieldKey
            FieldValue = Single {
                FieldKey = specField.FieldKey
                Value = formatter newValue
                Description = newValue
            }
            Label = specField.Label
            Options = []
        }

    match values.Keys |> Seq.tryFind (fun k -> k = newFieldDetails.Key) with
    | None -> values.Add(newFieldDetails.Key, newFieldDetails)      // ← Add new
    | Some key -> values.Remove key |> (fun f -> values.Add(key, newFieldDetails))  // ← Update existing
```

**Flow**:
1. User types in field
2. Fable.Form calls `Update` with new value and current `DynamicStepValues`
3. Function creates/updates `FieldDetails` in the map
4. Returns new `DynamicStepValues` map
5. Fable.Form creates new `Form.View.Model` with updated values
6. `FormChanged` message is dispatched

### Form State Updates (`FormChanged` Message)

```fsharp
| FormChanged newModel ->
    let newKey = StepOrder model.CurrentStep
    
    let newForms =
        { model.DynamicForm with
            Steps = model.DynamicForm.Steps.Change(newKey, (fun _ -> newModel |> Some))
        }
    
    { model with DynamicForm = newForms },
    Cmd.ofEffect (fun v -> props.FormChanged newForms)
```

**Key Points**:
- Updates the `Steps` map for current step
- Preserves all other steps unchanged
- Calls `FormChanged` callback to notify parent component

### Value Extraction (`extractDataFromFableFormsModel`)

```fsharp
let extractDataFromFableFormsModel
    (dynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>)
    : DynamicFormResultData
    =
    {
        ResultFormSpecDetails = dynamicForm.DynamicFormSpecDetails
        ResultSteps =
            dynamicForm.Steps
            |> Map.map (fun key dynamicStepValues -> 
                dynamicStepValues.Values  // ← Extract DynamicStepValues from Form.View.Model
            )
    }
```

**Purpose**: Converts runtime form state back to pure data structure for saving/processing.

---

## Key Patterns

### Pattern 1: Generic FormSpec with Unified Values

```fsharp
// Generic schema
type FormSpec<'UserField> = { ... }

// Unified value storage
type DynamicStepValues = Map<FieldKey, FieldDetails>

// Bridge function
type RenderUserField = 
    FormField<'UserField> 
    -> Form.Form<DynamicStepValues, ...>
```

**Benefit**: Type safety for schema definition, flexibility for value storage.

### Pattern 2: Field Key Mapping

```fsharp
// FormSpec uses string FieldKey
type FormField<'UserField> = {
    FieldKey: string  // ← String identifier
    ...
}

// Values use wrapped FieldKey
type FieldKey = FieldKey of string  // ← Wrapped for type safety
type DynamicStepValues = Map<FieldKey, FieldDetails>
```

**Conversion**: `FieldKey field.FieldKey` wraps string → `FieldKey` type.

### Pattern 3: Conditional Fields (`dependencyMatch`)

```fsharp
let dependencyMatch (dependsOnOpt: DependsOn option) field =
    match dependsOnOpt with
    | Some dep ->
        Form.meta (fun (stepValues: DynamicStepValues) ->
            let dependsFieldValue = stepValues |> Map.tryFind (FieldKey dep.FieldKey)
            match dependsFieldValue with
            | Some fieldDetails ->
                // Evaluate condition
                if conditionMet then field else emptyForm
            | _ -> emptyForm
        )
    | None -> field
```

**Key Points**:
- Uses `Form.meta` to access form values
- Checks dependent field value from `DynamicStepValues`
- Returns field or empty form based on condition

### Pattern 4: Step-by-Step State Management

```fsharp
type DynamicForm<'FableFormModel> = {
    Steps: Map<StepOrder, 'FableFormModel>  // ← One model per step
}
```

**Benefits**:
- Each step maintains its own form state
- Navigation preserves all step values
- Can validate steps independently

---

## Implementation Examples

### Example 1: Text Field Implementation

```fsharp
// In demo/Main.fs
match specField.FieldType with
| FieldType.Text info ->
    Form.textField
        {
            Parser = Ok
            Value = fun values -> Helpers.readValue specField values
            Update = Helpers.updateSingleFunc id specField
            Error = fun _ -> None
            Attributes = {
                Label = specField.Label
                Placeholder = ""
                HtmlAttributes = []
            }
        }
    |> Form.disableIf readOnly
    |> optionalMatch specField.IsOptional
    |> dependencyMatch specField.DependsOn
```

**Flow**:
1. Pattern match on `FieldType.Text`
2. Create `Form.textField` with Value/Update functions
3. Apply optional/readonly/dependency transformations
4. Returns `Form.Form<DynamicStepValues, string, IReactProperty>`

### Example 2: Checkbox Field Implementation

```fsharp
| FieldType.Checkbox info ->
    Form.checkboxField
        {
            Parser = string >> Ok
            Value = fun value -> 
                snd (bool.TryParse(Helpers.readValue specField value))
            Update = fun value values ->
                Helpers.updateSingleFunc id specField (string value) values
            Error = fun _ -> None
            Attributes = {
                Text = specField.Label
            }
        }
```

**Key Differences**:
- `Value` converts string → bool
- `Update` converts bool → string
- Uses same `updateSingleFunc` helper (stores as string)

### Example 3: Complete Form Usage

```fsharp
[<ReactComponent>]
let FormCompose (props: FormComposeProps<'UserField>) =
    let state, dispatch = React.useElmish (init props, update props, [||])
    
    // Get current step
    let currentStep = state.FormSpec.Steps 
        |> List.find (fun s -> s.StepOrder = state.CurrentStep)
    
    // Compose form
    let composedForm = currentStep
        |> Composer.compose 
            (state.ResultViewMode = FormComposeMode.ReadOnly)
            props.RenderUserField
    
    // Render form
    Composer.render
        state.DynamicForm.Steps[StepOrder state.CurrentStep]  // ← Current step's values
        dispatch
        (formAction progress (state.FormSaved && props.SubmissionSuccess))
        composedForm
```

### Example 4: Saving Form Values

```fsharp
| Submit ->
    let newModel = { model with FormSaved = true }
    
    newModel, 
    Cmd.ofEffect (fun v -> 
        // Extract pure data
        let resultData = extractDataFromFableFormsModel model.DynamicForm
        
        // Call save callback
        props.SaveFormValuesCallback model.DynamicForm
    )
```

---

## Summary: The Complete Picture

### Architecture Summary

1. **Schema Layer** (`FormSpec<'UserField>`)
   - Generic over domain-specific field types
   - Defines structure, validation, dependencies
   - Immutable, pure data structure

2. **Value Layer** (`DynamicStepValues`)
   - Unified storage for all field types
   - Map-based: `Map<FieldKey, FieldDetails>`
   - Supports Single and Multiple values

3. **Form Layer** (`Form.Form<DynamicStepValues, ...>`)
   - Fable.Form integration
   - Handles validation, state management
   - Bridges schema ↔ values

4. **State Layer** (`DynamicForm<Form.View.Model<DynamicStepValues>>`)
   - Runtime form state
   - Per-step form models
   - Managed by Elmish

### Key Takeaways

✅ **Generic FormSpec**: Type-safe schema definition with `'UserField` parameter

✅ **Unified Values**: All values stored in `DynamicStepValues` regardless of field type

✅ **Bridge Functions**: `RenderUserField` converts schema → form → values

✅ **Value Functions**: `Value` reads, `Update` writes to `DynamicStepValues`

✅ **Step Isolation**: Each step maintains its own form state

✅ **Type Safety**: Generic types ensure compile-time safety

✅ **Flexibility**: Can support any field type through `'UserField` parameter

---

## Next Steps for Main Feature Branch

Based on this POC, consider:

1. **Preserve Generic Pattern**: Keep `FormSpec<'UserField>` generic
2. **Maintain Value Structure**: Keep `DynamicStepValues` as unified storage
3. **Standardize Bridge Functions**: Create consistent `RenderUserField` implementations
4. **Extract Helpers**: Create reusable `readValue`/`updateValue` functions
5. **Type Safety**: Ensure `FieldKey` mapping is consistent
6. **State Management**: Consider Elmish patterns for complex forms

This architecture provides a **natural, generic, type-safe** way to handle form values while maintaining flexibility for different field types and use cases.

