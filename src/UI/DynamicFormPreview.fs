module Antidote.FormStudio.UI.DynamicFormPreview

open Feliz
open Feliz.Bulma
open Antidote.React.Components.FormWizard
open Antidote.FormStudio.Compose.Types
open Antidote.React.Components.FormWizard.SinglePageReview
open Fable.Form.Antidote
open Antidote.FormStudio.Types

type DynamicFormPreviewProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        SetIsPreview: bool -> unit
        RenderUserField:
            bool
                -> FormCompose.ComposerFunc
                -> FormField<'UserField>
                -> Form.Form<DynamicStepValues, string, IReactProperty>
    |}

[<ReactComponent>]
let DynamicFormPreview (props: DynamicFormPreviewProps<'UserField>) =
    let showValuesReview, setShowValuesReview = React.useState false
    let dynamicForm, setDynamicForm = React.useState None

    Bulma.columns [

        Bulma.column [
            if showValuesReview then
                column.is6
            else
                column.is12

            prop.children [
                Bulma.button.a [
                    color.isPrimary
                    prop.style [
                        style.marginBottom 15
                        style.marginTop 15
                    ]
                    prop.text "Exit Preview Mode"
                    prop.onClick (fun _ -> props.SetIsPreview false)
                ]

                FormCompose.FormCompose(
                    {|
                        FormSpec = props.FormSpec
                        DynamicForm = None
                        Mode = FormComposeMode.Editable
                        NavigateToStep = (fun _ -> ())
                        FormChanged = (fun _ -> ())
                        SaveFormValuesCallback =
                            fun dynamicFormValues ->
                                setShowValuesReview true
                                setDynamicForm (Some dynamicFormValues)

                        SubmissionSuccess = false
                        RenderUserField = props.RenderUserField
                    |}
                )
            ]
        ]

        if showValuesReview && dynamicForm.IsSome then
            let formResultData: DynamicFormResultData =
                dynamicForm.Value |> Antidote.FormStudio.Helper.extractDataFromFableFormsModel

            let formCode =
                match dynamicForm.Value.DynamicFormSpecDetails.FormSpecCode with
                | Some formCode -> formCode
                | None -> ""

            Bulma.column [
                column.is6
                prop.children [
                    Bulma.button.a [
                        color.isPrimary
                        prop.style [
                            style.marginBottom 15
                            style.marginTop 15
                        ]
                        prop.text "Hide Form Review"
                        prop.onClick (fun _ -> setShowValuesReview false)
                    ]

                    match props.FormSpec.Score with
                    | Some _ ->
                        // let score, flags =
                        //     dynamicForm.Value
                        //     |> Antidote.FormDesigner.Helper.flattenFormSteps
                        //     |> (Antidote.Core.FormProcessor.Processors.Default.getCalculator formCode)

                        // formCode
                        // |> Antidote.React.Components.FormWizard.Processors.Components.getResultOutput
                        // |> fun resultOutputFunc -> resultOutputFunc score flags
                        // |> Antidote.React.Components.FormWizard.Processors.Components.formResultRenderer
                        Html.span "IMPLEMENT CALCULATORS"

                    | None ->
                        Html.div [
                            Html.span [
                                prop.style [
                                    style.fontSize (length.px 16)
                                    style.fontWeight 600
                                    style.color "#bfbfbf"
                                ]
                                prop.text "** THIS FORM IS NOT SCORED **"
                            ]
                        ]

                    SinglePageReview
                        {|
                            FormSpec = props.FormSpec
                            DynamicForm = dynamicForm.Value
                            RenderUserField = props.RenderUserField
                        |}
                ]
            ]
    ]
