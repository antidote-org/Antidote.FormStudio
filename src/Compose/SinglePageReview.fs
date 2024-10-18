module Antidote.React.Components.FormWizard.SinglePageReview

open Feliz
open Feliz.Bulma
open Fable.Form.Antidote
open Antidote.FormStudio.Compose.Types
open Antidote.FormStudio.i18n.Util
open Antidote.FormStudio.Types

type SinglePageReviewProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        DynamicForm: DynamicForm<Form.View.Model<DynamicStepValues>>
        RenderUserField:
            bool
                -> FormCompose.ComposerFunc
                -> FormField<'UserField>
                -> Form.Form<DynamicStepValues, string, IReactProperty>
    |}

[<ReactComponent>]
let SinglePageReview (props: SinglePageReviewProps<'UserField>) =
    let flatFormSpec = props.FormSpec |> Antidote.FormStudio.Helper.flattenSpecSteps

    let flatDynamicForm =
        props.DynamicForm |> Antidote.FormStudio.Helper.flattenFormSteps

    Html.div [
        prop.children [
            match flatFormSpec.Steps |> List.tryFind (fun s -> s.StepOrder = 1) with
            | Some step ->
                Html.div [
                    Html.h1 [
                        prop.className "title is-3 is-bold has-text-centered"
                        prop.text (t props.FormSpec.Title)
                    ]
                    Html.p [
                        prop.className "subtitle is-4 has-text-centered"
                        prop.text (t step.StepLabel)
                    ]

                    Bulma.block [
                        prop.classes [
                            "has-text-centered"
                        ]
                        prop.children [
                            Bulma.label [
                                prop.classes [
                                    "has-text-centered"
                                ]
                                prop.children [
                                    Html.text (t "General Codes")
                                ]
                            ]
                            flatFormSpec.AssociatedCodes
                            |> List.map (fun code ->
                                Bulma.tag [
                                    tag.isRounded
                                    prop.style [
                                        style.cursor.pointer
                                    ]
                                    prop.key code
                                    color.hasBackgroundInfo
                                    color.hasTextWhite
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
                            flatFormSpec.Steps
                            |> List.find (fun s -> s.StepOrder = 1)
                            |> Composer.compose true props.RenderUserField
                            |> Composer.render
                                flatDynamicForm.Steps[StepOrder 1]
                                (fun _ -> ())
                                (FormActions.formAction ReadOnly true)
                        ]
                    ]
                ]
            | None -> Html.span "No step found"
        ]
    ]
