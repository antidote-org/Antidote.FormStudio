module Antidote.React.Components.FormWizard.FormCompose

open Feliz
open Feliz.Bulma
open Fable.Form
open Fable.Form.Simple
// open Fable.Form.Antidote
open Fable.Core.JsInterop
open Antidote.FormStudio.Compose.Types
open Antidote.FormStudio.Types

open FormActions
open Antidote.FormStudio.i18n.Util

// let private classes : CssModules.Compose.FormCompose = import "default" "./FormCompose.module.scss"

type ComposerFunc =
    DependsOn option
        -> Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>
        -> Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>

type FormComposeProps<'UserField> =
    {|
        // STRING CODE TO RETRIEVE THE SPEC FROM THE DATABASE
        FormSpec: FormSpec<'UserField>
        DynamicForm: DynamicForm<Fable.Form.Simple.Form.View.Model<DynamicStepValues>> option //Map<int,Form.View.Model<FormValues>>
        // EDIT OR READ-ONLY MODE
        Mode: FormComposeMode
        NavigateToStep: int -> unit
        FormChanged: DynamicForm<Fable.Form.Simple.Form.View.Model<DynamicStepValues>> -> unit
        SaveFormValuesCallback:
            DynamicForm<Fable.Form.Simple.Form.View.Model<DynamicStepValues>> -> unit
        SubmissionSuccess: bool
        RenderUserField:
            bool
                -> ComposerFunc
                -> FormField<'UserField>
                -> Fable.Form.Simple.Bulma.Form<DynamicStepValues, string>
    |}

let dynamicFormInit
    (dynamicStepValuesOpt: DynamicStepValues option)
    (formSpec: FormSpec<'UserField>)
    =
    let dynamicStepValues: DynamicStepValues =
        match dynamicStepValuesOpt with
        | Some dynamicStepValues -> dynamicStepValues
        | None -> Map.empty

    let output: DynamicForm<Fable.Form.Simple.Form.View.Model<DynamicStepValues>> =
        {
            Steps =
                [
                    1 .. formSpec.Steps.Length
                ]
                |> List.map (fun step ->
                    StepOrder step, dynamicStepValues |> Fable.Form.Simple.Form.View.idle
                )
                |> Map.ofList
            DynamicFormSpecDetails =
                {
                    FormSpecId = formSpec.Id
                    FormSpecCode = formSpec.Code
                    FormSpecTitle = formSpec.Title
                    FormSpecAbstract = formSpec.Abstract
                    FormSpecVersion = formSpec.FormSpecVersion
                    MaxScore = formSpec.Score
                    DynamicVersion = "2.0.5"
                }
        }

    output

let scrollElementByIdIntoView (elementId: string) =
    let elem = Browser.Dom.document.getElementById (elementId)

    if elem = null then
        ()
    else
        elem.scrollIntoView ()

let private wizardProgress (step: int) (totalSteps: int) =
    match totalSteps with
    | a when a = 1 -> Last
    | a when a = 2 ->
        if step = 1 then
            First
        else
            Last
    | a when a > 2 ->
        if step = 1 then
            First
        else if step = totalSteps then
            Last
        else
            Middle
    | _ -> Middle

[<ReactComponent>]
let FormCompose (props: FormComposeProps<'UserField>) =
    let initialState =
        {
            ResultViewMode = props.Mode
            FormSpec = props.FormSpec
            DynamicForm =
                match props.DynamicForm with
                | Some formValues -> formValues
                | None -> props.FormSpec |> (dynamicFormInit None)
            CurrentStep = 1
            FormSaved = false
        }

    let state, setState = React.useState initialState

    let handleFormChanged (newModel: Fable.Form.Simple.Form.View.Model<DynamicStepValues>) =
        let newKey = StepOrder state.CurrentStep

        let newForms =
            { state.DynamicForm with
                Steps = state.DynamicForm.Steps.Change(newKey, (fun _ -> newModel |> Some))
            }

        setState
            { state with
                DynamicForm = newForms
            }

        props.FormChanged newForms

    let handleFormSubmit (result: (string * string) list) =
        scrollElementByIdIntoView "form-top"

        if state.CurrentStep < state.DynamicForm.Steps.Count then
            let nextStep = state.CurrentStep + 1
            let nextStepOrder = StepOrder nextStep
            let nextFormValues = state.DynamicForm.Steps.[nextStepOrder]

            setState
                { state with
                    CurrentStep = nextStep
                    DynamicForm =
                        { state.DynamicForm with
                            Steps =
                                state.DynamicForm.Steps.Change(
                                    nextStepOrder,
                                    (fun _ -> nextFormValues |> Some)
                                )
                        }
                }

            props.NavigateToStep nextStep
        else
            setState
                { state with
                    FormSaved = true
                }

            props.SaveFormValuesCallback state.DynamicForm

    let handleNextStep () =
        let nextStep = state.CurrentStep + 1
        scrollElementByIdIntoView "form-top"
        let nextStepOrder = StepOrder nextStep
        let nextFormValues = state.DynamicForm.Steps.[nextStepOrder]

        setState
            { state with
                CurrentStep = nextStep
                DynamicForm =
                    { state.DynamicForm with
                        Steps =
                            state.DynamicForm.Steps.Change(
                                nextStepOrder,
                                (fun _ -> nextFormValues |> Some)
                            )
                    }
            }

        props.NavigateToStep nextStep

    let handlePreviousStep () =
        let prevStep = state.CurrentStep - 1
        scrollElementByIdIntoView "form-top"
        let prevStepOrder = StepOrder prevStep
        let previousFormValues = state.DynamicForm.Steps.[prevStepOrder]

        setState
            { state with
                CurrentStep = prevStep
                DynamicForm =
                    { state.DynamicForm with
                        Steps =
                            state.DynamicForm.Steps.Change(
                                prevStepOrder,
                                (fun _ -> previousFormValues |> Some)
                            )
                    }
            }

        props.NavigateToStep prevStep

    let progress =
        if state.ResultViewMode = FormComposeMode.ReadOnly then
            ReadOnly
        else
            wizardProgress state.CurrentStep state.DynamicForm.Steps.Count

    let progressValue = (state.CurrentStep * 100) / state.DynamicForm.Steps.Count

    React.fragment [
        Html.progress [
            prop.id "wizard-progress"
            prop.classes [
                "progress"
                "is-smaller"
                "is-primary"
                "wizard-progress"
            ]
            prop.style [
                style.top 0
            ]
            prop.value progressValue
            prop.max 100
        ]
        match state.FormSpec.Steps |> List.tryFind (fun s -> s.StepOrder = state.CurrentStep) with
        | Some step ->
            Html.div [
                Html.h1 [
                    prop.className "title is-3 is-bold has-text-centered"
                    prop.text (t state.FormSpec.Title)
                ]
                Html.p [
                    prop.className "subtitle is-4 has-text-centered"
                    prop.text (t step.StepLabel)
                ]
                Html.p [
                    text.hasTextCentered
                    prop.style [
                        style.whitespace.prewrap
                    ]
                    prop.text (t "General Codes")
                ]
                Bulma.block [
                    prop.classes [
                        "has-text-centered"
                    ]
                    prop.children [
                        state.FormSpec.AssociatedCodes
                        |> List.map (fun code ->
                            Bulma.tag [
                                tag.isRounded
                                prop.style [
                                    style.cursor.pointer
                                ]
                                prop.key code
                                prop.children [
                                    Html.text code
                                ]
                            ]
                        )
                        |> Html.div
                    ]
                ]

                Html.div [
                    prop.children [
                        let compoasedForm =
                            step
                            |> Composer.compose
                                (if state.ResultViewMode = FormComposeMode.ReadOnly then
                                     true
                                 else
                                     false)
                                props.RenderUserField

                        Composer.render
                            state.DynamicForm.Steps[StepOrder state.CurrentStep]
                            handleFormChanged
                            handleFormSubmit
                            (t Intl.Next.Key)
                            compoasedForm
                    ]
                ]
            ]

        | None -> Html.span "No step found"
    ]
