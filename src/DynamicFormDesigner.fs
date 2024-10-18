module Antidote.FormStudio.DynamicFormDesigner

open Feliz
open Fable.Core.JsInterop

open Antidote.FormStudio.Types
open Antidote.FormStudio.UI.FieldPicker
open Antidote.FormStudio.UI.DynamicFormPreview
open Antidote.FormStudio.UI.FormSpecLayout

let private classes: CssModules.DynamicFormDesigner =
    import "default" "./DynamicFormDesigner.module.scss"

[<ReactComponent>]
let DynamicFormDesigner
    (formSpec: FormSpec<'UserField>)
    (setFormSpec: FormSpec<'UserField> -> unit)
    (registeredFields: IDesignerField<'UserField> list)
    renderUserField
    =
    let defaultActiveField =
        {
            FormStepNumber = 1
            FormFieldNumber = 0
            State = Idle
        }

    let selectedStepNumber, setSelectedStepNumber =
        React.useState (formSpec.Steps.[0].StepOrder)

    let activeField, setActiveField = React.useState defaultActiveField

    let isFieldDragging, setFieldDragging = React.useState false

    let setActiveFieldWrapper activeField =
        if activeField.FormFieldNumber < 1 then
            setActiveField defaultActiveField
        else
            setActiveField activeField

    let isPreviewing, setIsPreviewing = React.useState false

    let setFormSpecWrapper spec = setFormSpec spec

    let registeredFields = RegisteredFields registeredFields

    React.fragment [

        if isPreviewing then
            DynamicFormPreview
                {|
                    FormSpec = formSpec
                    SetIsPreview = setIsPreviewing
                    RenderUserField = renderUserField
                |}
        else
            Html.section [
                prop.className "section"
                prop.children [
                    Html.div [
                        prop.className "container"
                        prop.children [
                            Html.div [
                                prop.className "columns"
                                prop.children [
                                    Html.div [
                                        prop.className "colum is-4"
                                        prop.children [
                                            FieldPicker
                                                {|
                                                    FormSpec = formSpec
                                                    SelectedStepNumber = selectedStepNumber
                                                    FormSpecChanged = setFormSpecWrapper
                                                    RegisteredFields = registeredFields
                                                |}
                                        ]
                                    ]

                                    Html.div [
                                        prop.className "column is-8"
                                        prop.children [
                                            FormSpecLayout
                                                {|
                                                    RegisteredFields = registeredFields
                                                    FormSpec = formSpec
                                                    OnChange = setFormSpecWrapper
                                                    SetIsPreview = setIsPreviewing
                                                    SelectedStepNumber = selectedStepNumber
                                                    SetStepNumber = setSelectedStepNumber
                                                    ActiveField = activeField
                                                    SetActiveField = setActiveFieldWrapper
                                                    IsFieldDragging = isFieldDragging
                                                    SetFieldDragging = setFieldDragging
                                                |}
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
    ]
