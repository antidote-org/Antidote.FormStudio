module Antidote.React.Components.FormWizard.FormCompose

open Feliz
open Feliz.UseElmish
open Feliz.Bulma
open Elmish
open Fable.Form.Antidote
open Fable.Core.JsInterop
open Antidote.FormStudio.Compose.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Values.v2_0_1

open FormActions
open Antidote.FormStudio.i18n.Util

// let private classes : CssModules.Compose.FormCompose = import "default" "./FormCompose.module.scss"

type ComposerFunc =
    DependsOn option
        -> Form.Form<DynamicStepValues, string, IReactProperty>
        -> Form.Form<DynamicStepValues, string, IReactProperty>

type FormComposeProps =
    {|
        // STRING CODE TO RETRIEVE THE SPEC FROM THE DATABASE
        FormSpec: FormSpec
        DynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>> option //Map<int,Form.View.Model<FormValues>>
        // EDIT OR READ-ONLY MODE
        Mode: FormComposeMode
        NavigateToStep: int -> unit
        FormChanged: DynamicForm<Form.View.Model<DynamicStepValues>> -> unit
        SaveFormValuesCallback: DynamicForm<Form.View.Model<DynamicStepValues>> -> unit
        SubmissionSuccess: bool
        RenderUserField:
            bool
                -> ComposerFunc
                -> FormField
                -> Form.Form<DynamicStepValues, string, IReactProperty>
    |}

let dynamicFormInit (dynamicStepValuesOpt: DynamicStepValues option) (formSpec: FormSpec) =
    let dynamicStepValues: DynamicStepValues =
        match dynamicStepValuesOpt with
        | Some dynamicStepValues -> dynamicStepValues
        | None -> Map.empty

    let output: DynamicForm<Form.View.Model<DynamicStepValues>> =
        {
            Steps =
                [
                    1 .. formSpec.Steps.Length
                ]
                |> List.map (fun step -> StepOrder step, dynamicStepValues |> Form.View.idle)
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

let private init (props: FormComposeProps) =
    {
        ResultViewMode = props.Mode
        FormSpec = props.FormSpec
        DynamicForm =
            match props.DynamicForm with
            | Some formValues -> formValues
            | None -> props.FormSpec |> (dynamicFormInit None)

        CurrentStep = 1
        FormSaved = false
    },
    Cmd.none

let scrollElementByIdIntoView (elementId: string) =
    let elem = Browser.Dom.document.getElementById (elementId)

    if elem = null then
        ()
    else
        elem.scrollIntoView ()

let private update (props: FormComposeProps) (msg: Msg) (model: FormComposeState) =
    match msg with

    | NavigateToStep newStep ->
        { model with
            CurrentStep = newStep
        },
        Cmd.ofEffect (fun i -> props.NavigateToStep newStep)

    | NextStep ->
        let nextStep = model.CurrentStep + 1
        scrollElementByIdIntoView "form-top"
        let nextStepOrder = StepOrder nextStep
        let nextFormValues = model.DynamicForm.Steps.[nextStepOrder]

        { model with
            CurrentStep = nextStep
            DynamicForm =
                { model.DynamicForm with
                    Steps =
                        model.DynamicForm.Steps.Change(
                            nextStepOrder,
                            (fun _ -> nextFormValues |> Some)
                        )

                }
        },
        Cmd.ofMsg (NavigateToStep nextStep)

    | PreviousStep ->
        let prevStep = model.CurrentStep - 1
        scrollElementByIdIntoView "form-top"
        let prevStepOrder = StepOrder prevStep
        let previousFormValues = model.DynamicForm.Steps.[prevStepOrder]

        { model with
            CurrentStep = prevStep
            DynamicForm =
                { model.DynamicForm with
                    Steps =
                        model.DynamicForm.Steps.Change(
                            prevStepOrder,
                            (fun _ -> previousFormValues |> Some)
                        )
                }
        },
        Cmd.ofMsg (NavigateToStep prevStep)

    | StepCompleted result ->
        scrollElementByIdIntoView "form-top"

        if (model.CurrentStep < model.DynamicForm.Steps.Count) then
            model, Cmd.ofMsg (NavigateToStep(model.CurrentStep + 1))
        else
            model, Cmd.ofMsg Submit

    | FormChanged newModel ->
        let newKey = StepOrder model.CurrentStep

        let newForms =
            { model.DynamicForm with
                Steps = model.DynamicForm.Steps.Change(newKey, (fun _ -> newModel |> Some))
            }

        { model with
            DynamicForm = newForms
        },
        Cmd.ofEffect (fun v -> props.FormChanged newForms)

    | Submit ->
        let newModel =
            { model with
                FormSaved = true
            }

        newModel, Cmd.ofEffect (fun v -> props.SaveFormValuesCallback model.DynamicForm)

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
let FormCompose (props: FormComposeProps) =
    let state, dispatch = React.useElmish (init (props), update props, [||])

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
                            // Html.div [
                            //     prop.children [
                            Bulma.tag [
                                tag.isRounded
                                prop.style [
                                    style.cursor.pointer
                                ]
                                prop.key code
                                // color.hasBackgroundInfo
                                // color.hasTextWhite
                                prop.children [
                                    Html.text code
                                ]
                            ]
                        )
                        |> Html.div

                    ]
                ]

                Html.div [
                    // prop.className classes.formContainer
                    prop.children [
                        step
                        |> Composer.compose
                            (if state.ResultViewMode = FormComposeMode.ReadOnly then
                                 true
                             else
                                 false)
                            props.RenderUserField
                        |> Composer.render
                            state.DynamicForm.Steps[StepOrder state.CurrentStep]
                            dispatch
                            (formAction progress (state.FormSaved && props.SubmissionSuccess))
                    ]
                ]
            ]

        | None -> Html.span "No step found"
    ]
