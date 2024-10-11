module Antidote.React.Components.FormWizard.Editor

open Fable.Form.Antidote
open Antidote.Core.FormProcessor.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.Core.FormProcessor.Helper
open Antidote.Core.FormProcessor

open Feliz
open Feliz.Bulma
open Elmish
open Feliz.UseElmish
// open Antidote.Core.V2.Types
open Antidote.FormStudio.i18n.Util
open System
// open Antidote.Core.V2.Domain.Form
// open Antidote.Core.V2.Types
open Browser
open Fable.Form.Antidote
open type Feliz.Toastify.Exports
open Antidote.React.Components.FormWizard
open Antidote.Core.V2.Utils.JS
open Feliz.ReactRouterDom
open Fable.Core.JsInterop
open Antidote.Core.V2.Communications
open Antidote.Core.V2.Domain
open Fable.Form.Antidote.Form.View
// open Antidote.Core.V2.Utils
open System.Text
open Thoth.Json
open SinglePageReview

let private classes: CssModules.Compose.Editor =
    import "default" "./Editor.module.scss"

type EditorFormValues =
    {
        Code: string
        Title: string
        Abstract: string
        Status: string
        SpecVersion: string
        SpecString: string
    }

type ViewMode =
    | FillForm
    | Editor

type State =
    {
        Form: EditorFormValues
        ViewMode: ViewMode
        Review: bool
        SelectedFormSpecOpt: FormSpec option
        AvailableFormSpecs: FormSpec list
    }

type Msg =
    | UpdateCode of string
    | UpdateTitle of string
    | UpdateAbstract of string
    | UpdateStatus of string
    | UpdateSpec of string
    | UpdateValues of string

    | Review

    // Endpoints
    | SaveFormSpec
    | SaveFormSpecResult of Antidote.Core.V2.Domain.Form.Response.SaveFormSpec
    | ReadAllFormSpecs
    | ReadAllFormSpecsResult of Antidote.Core.V2.Domain.Form.Response.ReadAllFormSpecs
    | ErroredRequest of exn

    | SelectFormSpec of FormSpec
    | Submit of DynamicForm<Form.View.Model<DynamicStepValues>>
    | UnauthGotFormSpec of Unauthenticated.Response.UnauthenticatedReadFormSpec
    | GotFormSpec of Form.Response.ReadFormSpec

// | SaveFormValues of Antidote.Core.V2.Domain.Form.Response.SaveFormValues
// | ErrorSavingFormValues of exn

let init (formDetailsOpt: string option * string option) =
    let readFormSpecCommand =
        match formDetailsOpt with
        | Some formId, Some formVersion ->
            let unauthFormSpecRequest =
                {
                    DynamicFormSpecId = Guid.Parse(formId)
                    DynamicFormSpecVersion = Some formVersion
                }
                : Unauthenticated.Request.UnauthenticatedReadFormSpec

            Cmd.OfAsync.either
                Antidote.Client.API.EndPoints.unauthenticated.UnauthenticatedReadFormSpec
                unauthFormSpecRequest
                UnauthGotFormSpec
                ErroredRequest
        | _ -> Cmd.ofMsg ReadAllFormSpecs

    let editorFormValues: EditorFormValues =
        {
            Code = ""
            Title = ""
            Abstract = ""
            SpecVersion = "2.0.0"
            Status = ""
            SpecString = ""
        }

    {
        Form = editorFormValues
        ViewMode = Editor
        AvailableFormSpecs = []
        SelectedFormSpecOpt = None
        Review = false
    },
    readFormSpecCommand

let update (msg: Msg) (state: State) =
    match msg with
    | UpdateValues values ->
        let newForm = { state.Form with SpecString = values }
        { state with Form = newForm; Review = false }, Cmd.none

    | Submit dynamicForm ->
        debuglog ($"Editor: Update: Submit: Ready to serialize result data!")

        let serializedDynamicForm = dynamicForm |> Thoth.Json.Encode.Auto.toString

        let serializedDynamicFormResultData =
            dynamicForm |> extractDataFromFableFormsModel |> Thoth.Json.Encode.Auto.toString

        debuglog ($"Editor: Update: Submit: DynamicForm: {serializedDynamicForm}")

        debuglog (
            $"Editor: Update: Submit: DynamicFormResultData: {serializedDynamicFormResultData}"
        )

        state, Cmd.none

    // let formValuesSerialized = Thoth.Json.Encode.Auto.toString( 0, dynamicForm )

    // let patientId : Antidote.Core.V2.Types.AccountId.T = (Antidote.Core.V2.Types.AccountId.create (System.Guid("232b825d-6395-4dd5-8cf2-40a8ce57e30f")) )
    // let clinicianId: Antidote.Core.V2.Types.AccountId.T = (Antidote.Core.V2.Types.AccountId.create (System.Guid("232b825d-6395-4dd5-8cf2-40a8ce57e30f")) )
    // let submitCmd =
    //     Cmd.OfAsync.either
    //         (Antidote.Client.User.UserSession.Instance.RemotingRequest Antidote.Client.API.EndPoints.form (fun services -> services.SaveFormValues))
    //         {
    //             AppointmentId = "" // referenceId
    //             PatientAccountId =  patientId
    //             ClinicianAccountId = clinicianId
    //             DynamicFormSpecId = Guid.NewGuid()
    //             FormValuesJson = formValuesSerialized
    //             WarningFlag = false // severity.WarningFlag
    //             ResultSeverity = "" // severity.SeverityLabel
    //         }
    //         SaveFormValues
    //         ErrorSavingFormValues
    // state, submitCmd
    | UpdateCode code ->
        let newForm = { state.Form with Code = code }
        { state with Form = newForm; Review = false }, Cmd.none
    | UpdateTitle title ->
        let newForm = { state.Form with Title = title }
        { state with Form = newForm; Review = false }, Cmd.none
    | UpdateAbstract abst ->
        let newForm = { state.Form with Abstract = abst }
        { state with Form = newForm; Review = false }, Cmd.none
    | UpdateStatus status ->
        let newForm = { state.Form with Status = status }
        { state with Form = newForm; Review = false }, Cmd.none
    | UpdateSpec specJson ->
        let formSpecOpt =
            specJson
            |> Migrator.FormSpec.decodeFormSpec Migrator.FormSpec.FormSpecInput.Unknown
            |> Migrator.FormSpec.migrateTo Migrator.FormSpec.FormSpecOutput.Latest
            |> fun x ->
                match x with
                | Migrator.FormSpec.FormSpecVersion.V2_0_1_FormSpec formSpec -> Some formSpec
                | _ -> None

        debuglog $"Editor: Update: UpdateSpec: FormSpec:{formSpecOpt}"
        let newForm = { state.Form with SpecString = specJson }

        { state with
            Form = newForm
            Review = false
            SelectedFormSpecOpt = formSpecOpt
        },
        Cmd.none

    | Review -> { state with Review = true }, Cmd.none

    // Need to add enabled to the form spec DTO
    | SelectFormSpec formSpec ->
        let newForm =
            {
                Code =
                    match formSpec.Code with
                    | Some formCode -> formCode
                    | None -> ""
                Title = formSpec.Title
                Abstract = formSpec.Abstract
                SpecVersion = formSpec.Version
                Status = "true" //formSpec.Enabled.ToString()
                SpecString = "" //formSpec.FormSpecJson
            }

        { state with
            Form = newForm
            SelectedFormSpecOpt = Some formSpec
            Review = false
        },
        Cmd.none

    | SaveFormSpec ->
        let request: Antidote.Core.V2.Domain.Form.Request.SaveFormSpec =
            {
                Id = Guid.NewGuid()
                Enabled = true // NEED TO HAVE A WAY TO SET THIS
                Code = Some state.Form.Code
                Title = state.Form.Title
                Abstract = state.Form.Abstract
                DynamicFormSpecJson = state.Form.SpecString
                DynamicFormSpecVersion = state.Form.SpecVersion
                SaveDate = DateTimeOffset.Now
                IsPrivate = true
                Status = Antidote.Core.V2.Types.SpecStatus.Draft
            }

        state,
        Cmd.OfAsync.either
            (Antidote.Client.User.UserSession.Instance.RemotingRequest
                Antidote.Client.API.EndPoints.form
                (fun services -> services.SaveFormSpec))
            request
            SaveFormSpecResult
            ErroredRequest
    | SaveFormSpecResult response ->
        match response with
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.NoMatchingForm ->
            toast (Html.div "There was an issue saving the form spec.") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.FormSpecTitleAlreadyTaken ->
            toast (Html.div "Conflicting form spec title found.") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.FormSpecIdAlreadyTaken ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: SaveFormSpecResult: SAVED FORM"
            toast (Html.div "Conflicting form spec ID found.") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.Archived ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: SaveFormSpecResult: SAVED FORM"
            toast (Html.div "Form has been archived!") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.Published id ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: SaveFormSpecResult: SAVED FORM"
            toast (Html.div "Published form!") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.Saved id ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: SaveFormSpecResult: SAVED FORM"
            toast (Html.div "Saved form!") |> ignore
            state, Cmd.ofMsg ReadAllFormSpecs
        | Antidote.Core.V2.Domain.Form.Response.SaveFormSpec.InvalidRequest errs ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: SaveFormSpecResult: UNABLE TO SAVE FORM"
            state, Cmd.none
    | GotFormSpec _ ->
        // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: GotFormSpec: NOT IMPLEMENTED"
        state, Cmd.none
    | UnauthGotFormSpec(response: Unauthenticated.Response.UnauthenticatedReadFormSpec) ->
        match response with
        | Unauthenticated.Response.UnauthenticatedReadFormSpec.Read formSpecResult ->
            let formSpec =
                formSpecResult.FormSpecJson
                |> Migrator.FormSpec.decodeFormSpec Migrator.FormSpec.FormSpecInput.Unknown
                |> Migrator.FormSpec.migrateTo Migrator.FormSpec.FormSpecOutput.Latest
                |> fun x ->
                    match x with
                    | Migrator.FormSpec.FormSpecVersion.V2_0_1_FormSpec formSpec -> formSpec
                    | _ -> failwith $"This build is incompatible with the form spec: {x}"

            let newForm =
                {
                    Code =
                        match formSpec.Code with
                        | Some code -> code
                        | None -> ""
                    Title = formSpec.Title
                    Abstract = formSpec.Abstract
                    Status = ""
                    SpecVersion = "1.0.0"
                    SpecString = formSpecResult.FormSpecJson
                }

            { state with
                Form = newForm
                Review = true
                ViewMode = FillForm
            },
            Cmd.none

        | _ ->
            // debuglog "Editor: Update: UnauthGotFormSpec: NOT FOUND"
            state, Cmd.none
    | ReadAllFormSpecs ->
        state,
        Cmd.OfAsync.either
            (Antidote.Client.User.UserSession.Instance.RemotingRequest
                Antidote.Client.API.EndPoints.form
                (fun services -> services.ReadAllFormSpecs))
            { FormSpecCategory = Antidote.Core.V2.Types.FormSpecCategory.Published }
            ReadAllFormSpecsResult
            ErroredRequest
    | ReadAllFormSpecsResult response ->
        match response with
        | Antidote.Core.V2.Domain.Form.Response.ReadAllFormSpecs.ReadFormSpecs formSpecDtos ->
            let formSpecs =
                formSpecDtos
                |> List.map (fun dto -> dto.FormSpecJson)
                |> List.map (
                    Migrator.FormSpec.decodeFormSpec Migrator.FormSpec.FormSpecInput.Unknown
                )
                |> List.map (Migrator.FormSpec.migrateTo Migrator.FormSpec.FormSpecOutput.Latest)
                |> List.map (fun x ->
                    match x with
                    | Migrator.FormSpec.FormSpecVersion.V2_0_1_FormSpec formSpec -> formSpec
                    | _ -> failwith $"This build is incompatible with the form spec: {x}"
                )

            { state with AvailableFormSpecs = formSpecs }, Cmd.none
        | Antidote.Core.V2.Domain.Form.Response.ReadAllFormSpecs.NoFormSpecsFound ->
            // Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: ReadAllFormSpecsResult: NO FORM SPECS FOUND"
            state, Cmd.none
    | ErroredRequest ex ->
        Antidote.Core.V2.Utils.JS.debuglog "Editor: Update: ErroredRequest: API EXCEPTION"
        state, Cmd.none

[<ReactComponent>]
let SpecSelector (state, dispatch, setDynamicForm) =
    Html.div
        [
            prop.style [ style.margin 20 ]
            prop.children
                [
                    Bulma.field.div
                        [
                            Bulma.label "Use Existing Form as Template"
                            Bulma.control.div
                                [
                                    prop.children
                                        [
                                            Bulma.select
                                                [
                                                    prop.defaultValue (
                                                        match state.SelectedFormSpecOpt with
                                                        | Some formSpec -> formSpec.Title
                                                        | None -> ""
                                                    )
                                                    prop.onChange (fun title ->
                                                        setDynamicForm (None)

                                                        let selectedFormSpec =
                                                            state.AvailableFormSpecs
                                                            |> List.find (fun f -> f.Title = title)

                                                        SelectFormSpec selectedFormSpec |> dispatch
                                                    )
                                                    prop.children
                                                        [
                                                            Html.option "Select One"
                                                            state.AvailableFormSpecs
                                                            |> List.sortBy (fun x -> x.Title)
                                                            |> List.map (fun formSpec ->
                                                                Html.option
                                                                    [
                                                                        prop.value formSpec.Title
                                                                        prop.children
                                                                            [
                                                                                Html.text
                                                                                    formSpec.Title
                                                                            ]
                                                                    ]
                                                            )
                                                            |> React.fragment
                                                        ]
                                                ]
                                        ]
                                ]
                        ]
                ]
        ]

[<ReactComponent>]
let SpecValuesEditor
    (
        state,
        dispatch,
        setDynamicForm,
        (dynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>> option)
    )
    =
    Html.form
        [
            prop.style [ style.margin 20 ]
            prop.children
                [
                    Bulma.field.div
                        [
                            Bulma.label "Code"
                            Bulma.control.div
                                [
                                    Bulma.input.text
                                        [
                                            prop.onChange (UpdateCode >> dispatch)
                                            prop.placeholder "form name"
                                            prop.value state.Form.Code
                                        ]
                                ]
                        ]
                    Bulma.field.div
                        [
                            Bulma.label "Title"
                            Bulma.control.div
                                [
                                    Bulma.input.text
                                        [
                                            prop.onChange (UpdateTitle >> dispatch)
                                            prop.placeholder "form title"
                                            prop.value state.Form.Title
                                        ]
                                ]
                        ]
                    Bulma.field.div
                        [
                            Bulma.label "Abstract"
                            Bulma.control.div
                                [
                                    Bulma.input.text
                                        [
                                            prop.onChange (UpdateAbstract >> dispatch)
                                            prop.placeholder "short description of the form"
                                        ]
                                ]
                        ]
                    Bulma.field.div
                        [
                            Bulma.label "Status"
                            Bulma.control.div
                                [
                                    Bulma.control.hasIconsLeft
                                    // prop.onSelect (UpdateStatus >> dispatch)
                                    prop.children
                                        [
                                            Bulma.select
                                                [ Html.option "Enabled"; Html.option "Disabled" ]
                                            Bulma.icon
                                                [
                                                    Bulma.icon.isSmall
                                                    Bulma.icon.isLeft
                                                    prop.children
                                                        [ Html.i [ prop.className "fas fa-check" ] ]
                                                ]
                                        ]
                                ]
                        ]

                    Bulma.field.div
                        [
                            Bulma.label "Spec"
                            Bulma.control.div
                                [
                                    Bulma.textarea
                                        [
                                            prop.onChange (UpdateSpec >> dispatch)
                                            prop.placeholder "Enter the FormSpec in JSON format"
                                            prop.value (
                                                // ""
                                                // let specRes =
                                                //     match Decode.Auto.fromString<FormSpec>(state.Form.Spec) with // state.Form.Spec
                                                //     | Ok spec ->
                                                //         UpdateSpec spec |> dispatch
                                                //         ""
                                                //     | Error a -> state.Form.Spec
                                                // match state.SelectedFormSpecOpt with
                                                // | Some formSpec ->
                                                //     let formSpecJson = Encode.Auto.toString<FormSpec>(0, formSpec)
                                                //     $"{formSpecJson}"
                                                // | _ ->
                                                //     state.Form.Spec
                                                state.Form.SpecString
                                            )
                                        ]
                                ]
                        ]
                    Bulma.field.div
                        [
                            Bulma.label "Values"
                            Bulma.control.div
                                [
                                    Bulma.textarea
                                        [
                                            prop.onChange (fun formValuesJson ->
                                                let dynamicFormRes =
                                                    deserializeForm
                                                        state.SelectedFormSpecOpt
                                                        formValuesJson

                                                match dynamicFormRes with
                                                | Ok dynamicForm ->
                                                    setDynamicForm (Some dynamicForm)

                                                | Result.Error err ->
                                                    toast (
                                                        Html.div
                                                            "Error decoding form values from string!"
                                                    )
                                                    |> ignore

                                            )
                                            prop.placeholder
                                                "Enter is the dynamicForm in JSON format"
                                            prop.value (
                                                match dynamicForm with
                                                | Some c -> serializeForm c
                                                | None -> ""
                                            )

                                        ]
                                ]
                        ]

                    Bulma.field.div
                        [
                            Bulma.label "Spec in FormSpec Base64"
                            Html.span
                                [
                                    prop.className "control"
                                    prop.style
                                        [
                                            style.display.block
                                            style.overflowWrap.anywhere
                                            style.fontSize 6
                                        ]
                                    prop.text (
                                        match state.SelectedFormSpecOpt with
                                        | Some formSpec ->
                                            let formSpecJson =
                                                Encode.Auto.toString<FormSpec> (0, formSpec)
                                                |> Base64.encode

                                            $"{formSpecJson}"
                                        | _ -> ""
                                    )
                                ]
                        ]

                    // Bulma.field.div [
                    //     Bulma.label "Spec in Base64 encoded format"
                    //     Html.span [
                    //         prop.className "control"
                    //         prop.style [
                    //             style.display.block
                    //             style.overflowWrap.anywhere
                    //             style.fontSize 6
                    //         ]
                    //         prop.text (state.Form.Spec |> Base64.encode)
                    //     ]
                    // ]
                    Bulma.field.div
                        [
                            Bulma.field.isGrouped
                            Bulma.field.isGroupedCentered
                            prop.children
                                [
                                    Bulma.control.div
                                        [
                                            Bulma.button.a
                                                [
                                                    Bulma.color.isLink
                                                    prop.onClick (fun _ -> Review |> dispatch)
                                                    prop.text "Review"
                                                ]
                                        ]
                                    Bulma.control.div
                                        [
                                            Bulma.button.a
                                                [ Bulma.color.isLink; prop.text "Submit" ]
                                        ]
                                    Bulma.control.div
                                        [
                                            Bulma.button.a
                                                [
                                                    Bulma.color.isLink
                                                    prop.text "Save"
                                                    prop.onClick (fun _ -> SaveFormSpec |> dispatch)
                                                ]
                                        ]
                                ]
                        ]
                ]
        ]

[<ReactComponent>]
let DynamicFormRenderer (formSpec, dispatch, formComposeMode, setDynamicForm, dynamicFormOpt) =
    Bulma.column
        [
            FormCompose.FormCompose(
                {|
                    FormSpec = formSpec
                    DynamicForm = dynamicFormOpt
                    Mode = formComposeMode
                    NavigateToStep = (fun _ -> ())
                    FormChanged = fun dynamicForm -> setDynamicForm (Some dynamicForm)
                    SaveFormValuesCallback = fun dynamicForm -> Submit dynamicForm |> dispatch
                    SubmissionSuccess = false
                |}
            )
        ]

// type FormReviewProps = {|
//     State: State
//     Dispatch: Msg -> unit
//     FormSpec: FormSpec
//     setDynamicForm: Map<int, Form.View.Model<DynamicStepValues>> option -> unit
//     // dynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>> option
//     dynamicForm: Map<int, Form.View.Model<DynamicStepValues>> option
//     // DynamicForm: DynamicForm<Form.View.Model<dynamicForm>>
// |}

// [<ReactComponent>]
// let FormReview (dispatch, (formSpec:FormSpec), setDynamicForm, (flatDynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>) ) =
//     Bulma.column [
//         prop.children [
//             Html.div [
//                 prop.classes [
//                     classes.pageContainer
//                 ]
//                 prop.children[
//                     Bulma.text.div [
//                         size.isSize3
//                         text.hasTextCentered
//                         prop.text (t formSpec.Title )
//                     ]
//                     Bulma.text.div [
//                         text.hasTextCentered
//                         prop.style [
//                             style.whitespace.prewrap
//                         ]
//                         prop.text (t formSpec.Steps[0].StepLabel)
//                     ]
//                     Html.div [
//                         prop.className classes.formContainer
//                         prop.children [
//                             formSpec.Steps
//                             |> List.find (fun s -> s.StepOrder = 1)
//                             |> Composer.compose true
//                             |> Composer.render flatDynamicForm.Steps[StepOrder 1] (fun _ -> ()) (FormActions.formAction ReadOnly true)
//                         ]
//                     ]
//                 ]
//             ]
//         ]
//     ]

[<ReactComponent>]
let Editor () =
    let location = useLocation ()

    let dynamicFormOpt, setDynamicForm =
        React.useState<DynamicForm<Form.View.Model<DynamicStepValues>> option> (None)

    let paramMap = Antidote.Core.V2.Utils.QueryParser.parseQueryString location.search

    let formDetails =
        let formIdOpt = paramMap |> Map.tryFind "formId"
        let formVersionOpt = paramMap |> Map.tryFind "formVersion"
        formIdOpt, formVersionOpt

    let state, dispatch = React.useElmish (init (formDetails), update, [||])

    Html.div
        [
            match state.ViewMode with
            | Editor ->
                SpecSelector(state, dispatch, setDynamicForm)
                SpecValuesEditor(state, dispatch, setDynamicForm, dynamicFormOpt)
            | FillForm -> Html.none

            match state.Review with
            | false -> Html.none
            | true ->
                match state.SelectedFormSpecOpt with
                | None -> Html.div []
                | Some formSpec ->
                    Bulma.columns
                        [
                            //This one gets a DynamicForm *option*, because the composer doesn't need an initial state for the internal values.
                            Bulma.column
                                [
                                    DynamicFormRenderer(
                                        formSpec,
                                        dispatch,
                                        FormComposeMode.Editable,
                                        setDynamicForm,
                                        dynamicFormOpt
                                    )
                                ]
                            Bulma.column
                                [
                                    match state.ViewMode with
                                    | Editor ->
                                        //This one gets a FLAT DynamicForm
                                        // Flat because it reviews *all fields in one page* and *not an option* because we can't review a form without values.

                                        ///////TODO: This is what it *should be*!!! Figure out why this doesn't refresh the fable forms!!!!!
                                        //////       As it is, FormReview it's just a fn copy of the FormCompose component 🤬
                                        // DynamicFormRenderer ( flatSpec, dispatch, FormComposeMode.ReadOnly, setDynamicForm, flatDynamicForm )
                                        match dynamicFormOpt with
                                        | Some dynamicForm ->
                                            SinglePageReview
                                                {| FormSpec = formSpec; DynamicForm = dynamicForm |}
                                        | None -> Html.none

                                    | FillForm -> Html.none
                                ]
                        ]
        ]
