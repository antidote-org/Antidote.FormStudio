module Antidote.React.Components.FormWizard.DynamicFormAsComponent

open Feliz
open Feliz.Bulma
open System
open Fable.Core.JsInterop

open Antidote.Core.FormProcessor.Spec.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Values.v2_0_1
open Antidote.FormStudio.Compose.Types
open Fable.Form.Antidote

open FsToolkit.ErrorHandling

let private classes: CssModules.Compose.DynamicFormAsComponent =
    import "default" "./DynamicFormAsComponent.module.scss"

type DynamicFormAsComponentProps<'T> =
    {|
        FormSpec: FormSpec
        MappingAdapter: DynamicFormResultData -> Validation<'T, string>
        // MappingAdapter: DynamicFormResultData -> Result<'T, string>
        CompletionCallback: Result<'T, string list> -> unit
        EditCallback: unit -> unit
        RenderUserField:
            bool
                -> FormCompose.ComposerFunc
                -> FormField
                -> Form.Form<DynamicStepValues, string, IReactProperty>
    |}

[<ReactComponent>]
let DynamicFormAsComponent (props: DynamicFormAsComponentProps<'T>) =
    let dynamicForm, setDynamicForm = React.useState None
    let savedFields, setSavedFields = React.useState []
    let mode, setMode = React.useState FormComposeMode.Editable

    Bulma.box [
        // DigitalOceanCnd()
        match mode with
        | FormComposeMode.Editable ->
            Bulma.column [
                FormCompose.FormCompose(
                    {|
                        FormSpec = props.FormSpec
                        DynamicForm = dynamicForm
                        Mode = mode
                        NavigateToStep = (fun _ -> ())
                        FormChanged = fun _ -> ()
                        SaveFormValuesCallback =
                            fun dynamicForm ->
                                let serializedDynamicFormResultData =
                                    dynamicForm
                                    |> Antidote.FormDesigner.Helper.extractDataFromFableFormsModel
                                    |> Antidote.FormSpec.ResultExtract.dynamicFormResultDataToList

                                setSavedFields serializedDynamicFormResultData
                                setDynamicForm (Some dynamicForm)
                                setMode FormComposeMode.ReadOnly

                                let data =
                                    dynamicForm
                                    |> Antidote.FormDesigner.Helper.extractDataFromFableFormsModel
                                    |> props.MappingAdapter

                                props.CompletionCallback data
                        SubmissionSuccess = false
                        RenderUserField = props.RenderUserField
                    |}
                )
            ]
        | FormComposeMode.ReadOnly ->
            React.fragment [
                Bulma.label [
                    size.isSize1
                    prop.children [
                        Html.text props.FormSpec.Title
                    ]
                ]
                Html.table [
                    prop.classes [
                        "table"
                        "is-bordered"
                        "is-striped"
                        "is-narrow"
                        "is-hoverable"
                        "is-fullwidth"
                    ]
                    prop.children [
                        Html.thead [
                            Html.tr [
                                Html.th "Question"
                                Html.th "Answer(s)"
                            ]
                        ]
                        Html.tbody [
                            savedFields
                            |> List.map (fun field ->
                                Html.tr [
                                    Html.td field.Label
                                    Html.td [
                                        // prop.key (FieldKey field.Key)
                                        prop.children [
                                            Html.ul [
                                                match field.FieldValue with
                                                | Single answer ->
                                                    if answer.Value.StartsWith("data:image") then
                                                        Html.img [
                                                            prop.src answer.Value
                                                        ]

                                                    else
                                                        answer.Description |> Html.text

                                                | Multiple answers ->
                                                    answers
                                                    |> Seq.map (fun a -> Html.li a.Description)
                                                    |> React.fragment
                                            ]
                                        ]
                                    ]
                                ]
                            )
                            |> React.fragment
                        ]
                    ]
                ]
                Html.div [
                    prop.className classes.controls
                    prop.children [
                        Bulma.button.a [
                            button.isSmall
                            prop.onClick (fun _ ->
                                props.EditCallback()
                                setMode FormComposeMode.Editable
                            )
                            prop.children [
                                Html.text "Edit"
                            ]
                        ]
                    ]
                ]
            ]
    ]
