module Antidote.React.Components.FormWizard.DynamicFormBuilder

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


emitJsStatement () "import React from \"react\""

let private classes : CssModules.FormWizard.Compose.DynamicFormBuilder = import "default" "./DynamicFormBuilder.module.scss"

[<RequireQualifiedAccess>]
type BuilderField =
    | Step
    | Message
    | Signature
    | SingleChoice
    | MultiChoice
    | Text
    | Tel
    | Date
    | Time
    | Number
    | YesNo
    | TrueFalse
        member x.Key =
            match x with
            | Step -> "Step"
            | Message -> "Message"
            | Signature -> "Signature"
            | SingleChoice -> "Single Choice"
            | MultiChoice -> "MultiChoice"
            | Text -> "Text"
            | Tel -> "Tel"
            | Date -> "Date"
            | Time -> "Time"
            | Number -> "Number"
            | YesNo -> "Yes / No"
            | TrueFalse -> "True / False"

        member x.Icon =
            match x with
            | Step -> "fas fa-step-forward"
            | Message -> "fas fa-info"
            | Signature -> "fas fa-signature"
            | SingleChoice -> "fas fa-dot-circle"
            | MultiChoice -> "fas fa-check-square"
            | Text -> "fas fa-font"
            | Tel -> "fas fa-phone"
            | Date -> "fas fa-calendar"
            | Time -> "fas fa-clock"
            | Number -> "fas fa-hashtag"
            | YesNo -> "fas fa-dot-circle"
            | TrueFalse -> "fas fa-dot-circle"

let availableComponents =
    [
        BuilderField.Step
        BuilderField.Message
        BuilderField.Signature
        BuilderField.SingleChoice
        BuilderField.MultiChoice
        BuilderField.Text
        BuilderField.Tel
        BuilderField.Date
        BuilderField.Time
        BuilderField.Number
        BuilderField.YesNo
        BuilderField.TrueFalse
    ]

let defaultFormSpec: FormSpec = {
    Id = Guid.NewGuid()
    Code = ""
    Title = ""
    Abstract = ""
    Version = "1.0.0"
    FormSpecVersion = ""
    Steps = [
        {
            StepOrder = 1
            StepLabel = "Step 1"
            Fields = [{
                FieldOrder = 1
                FieldKey = "Field1"
                Label = "Field 1"
                DependsOn = None
                IsOptional = false
                IsDeprecated = false
                FieldType = FieldType.Text { Value = None }
            }]
        }
    ]
    CategoryTags = []
}


type StepBreakProps = {
    FormSpec: FormSpec
    OnChange: FormSpec -> unit
}


[<ReactComponent>]
let StepBreak(props: StepBreakProps) =
    Html.div [
        prop.className classes.stepBreak
        prop.children [
            Html.button [
                prop.className classes.buttonMinimal
                prop.text "+ ADD STEP"
                prop.onClick (fun _ ->
                    //add new step
                    let newStep = {
                        StepOrder = props.FormSpec.Steps |> List.length
                        StepLabel = "New Step"
                        // Icon = "fas fa-font"
                        Fields = [
                            {
                                FieldOrder = 0
                                FieldKey = "new-field"
                                FieldType = FieldType.Text { Value = None}
                                Label = "New Field"
                                IsOptional = false
                                IsDeprecated = false
                                DependsOn = None
                            }
                        ]
                    }
                    let newFormSpec = {
                        props.FormSpec with
                            Steps = props.FormSpec.Steps @ [ newStep ]
                    }
                    props.OnChange newFormSpec
                )
            ]
        ]
    ]


type FormSpecLayoutProps = {
    FormSpec: FormSpec
    OnChange: FormSpec -> unit
}

[<RequireQualifiedAccess>]
type BulmaFleldLayout =
    | Horizontal
    | Vertical

type BulmaHorizontalFieldProps = {
    Label: string option
    Placeholder: string
    Value: string
    Layout: BulmaFleldLayout
    OnChange: string -> unit
}

[<ReactComponent>]
let BulmaHorizontalField(props:BulmaHorizontalFieldProps) =
    Bulma.field.div [
            if props.Layout = BulmaFleldLayout.Horizontal then
                field.isHorizontal
            prop.children [
                match props.Label with
                | Some label ->
                    Bulma.fieldLabel [
                        fieldLabel.isNormal
                        prop.children [
                            Bulma.label label
                        ]
                    ]
                | None -> ()

                Bulma.fieldBody [
                    Bulma.field.div [
                        prop.children [
                            Bulma.control.p [
                                control.isExpanded
                                control.hasIconsLeft
                                control.hasIconsRight
                                prop.children [
                                    Bulma.input.email [
                                        // prop.classes [ "input"; "is-success" ]
                                        color.isSuccess
                                        prop.placeholder props.Placeholder
                                        prop.value props.Value
                                        prop.onChange props.OnChange
                                    ]
                                    Bulma.icon [
                                        icon.isSmall
                                        icon.isLeft
                                        prop.children [
                                            Html.i [
                                                prop.classes [ "fas"; "fa-envelope" ]
                                            ]
                                        ]
                                    ]
                                    Bulma.icon [
                                        icon.isSmall
                                        icon.isRight
                                        prop.children [
                                            Html.i [
                                                prop.classes [ "fas"; "fa-check" ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

[<ReactComponent>]
let FormSpecLayout(props: FormSpecLayoutProps) =
    Bulma.column [
        BulmaHorizontalField {
            Label = Some "Form Code"
            Placeholder = "Internal Code reference the form in the API"
            Value = props.FormSpec.Code
            Layout = BulmaFleldLayout.Horizontal
            OnChange = (fun e ->
                props.OnChange ({
                    props.FormSpec with
                        Code = e
                })
            )
        }

        BulmaHorizontalField {
            Label = Some "Form Title"
            Placeholder = "Name as it will appear in the assessments list"
            Value = props.FormSpec.Title
            Layout = BulmaFleldLayout.Horizontal
            OnChange = (fun e ->
                props.OnChange ({
                    props.FormSpec with
                        Title = e
                })
            )
        }
        BulmaHorizontalField {
            Label = Some "Form Abstract"
            Placeholder = "Short description of the form, and its purpose"
            Value = props.FormSpec.Abstract
            Layout = BulmaFleldLayout.Horizontal
            OnChange = (fun e ->
                props.OnChange ({
                    props.FormSpec with
                        Abstract = e
                })
            )
        }
        BulmaHorizontalField {
            Label = Some "Form Version"
            Placeholder = "Version of the form"
            Value = props.FormSpec.Version
            Layout = BulmaFleldLayout.Horizontal
            OnChange = (fun e ->
                props.OnChange ({
                    props.FormSpec with
                        Version = e
                })
            )
        }

        props.FormSpec.Steps
        |> List.map (fun step ->
            Bulma.card [
                prop.children [
                    Bulma.cardHeader [
                        prop.children [
                            Bulma.cardHeaderTitle.div [
                                prop.children [
                                    BulmaHorizontalField {
                                        Label = None
                                        Placeholder = "Step Label"
                                        Value = step.StepLabel
                                        Layout = BulmaFleldLayout.Horizontal
                                        OnChange = (fun e ->
                                            props.OnChange ({
                                                props.FormSpec with
                                                    Steps = props.FormSpec.Steps
                                                    |> List.map (fun s ->
                                                        if s.StepOrder = step.StepOrder then
                                                            { s with StepLabel = e }
                                                        else
                                                            s
                                                    )
                                            })
                                        )
                                    }
                                    // Html.span [
                                    //     prop.classes [ "icon" ]
                                    //     prop.children [
                                    //         Html.i [
                                    //             // prop.classes [ step.Icon ]
                                    //         ]
                                    //     ]
                                    // ]
                                    // Html.span [
                                    //     prop.children [
                                    //         Bulma.label step.StepLabel
                                    //         Bulma.input.text [
                                    //             prop.value step.StepLabel
                                    //         ]
                                    //     ]
                                    // ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.children [
                            step.Fields
                            |> List.map (fun formField ->
                                match formField.FieldType with
                                | FieldType.Text _ ->
                                    Bulma.field.div [
                                        field.isHorizontal
                                        prop.children [

                                            Bulma.fieldLabel [
                                                fieldLabel.isNormal
                                                prop.children [
                                                    Bulma.label "Field Label"
                                                ]
                                            ]
                                            Bulma.fieldBody [
                                                Bulma.field.div [
                                                    prop.children [
                                                        Bulma.control.p [
                                                            control.isExpanded
                                                            control.hasIconsLeft
                                                            control.hasIconsRight
                                                            prop.children [
                                                                Bulma.input.email [
                                                                    // prop.classes [ "input"; "is-success" ]
                                                                    color.isSuccess
                                                                    prop.placeholder "Field Label"
                                                                    prop.value formField.Label
                                                                    prop.onChange (fun (e:string) ->
                                                                        let newFormField = {
                                                                            formField with
                                                                                Label = e
                                                                        }
                                                                        let newFormSpec = {
                                                                            props.FormSpec with
                                                                                Steps = props.FormSpec.Steps
                                                                                |> List.map (fun s ->
                                                                                    if s.StepOrder = step.StepOrder then
                                                                                        { s with
                                                                                            Fields = s.Fields
                                                                                            |> List.map (fun f ->
                                                                                                if f.FieldOrder = formField.FieldOrder then
                                                                                                    newFormField
                                                                                                else
                                                                                                    f
                                                                                            )
                                                                                        }
                                                                                    else
                                                                                        s
                                                                                )
                                                                        }

                                                                        props.OnChange newFormSpec
                                                                    )
                                                                ]
                                                                Bulma.icon [
                                                                    icon.isSmall
                                                                    icon.isLeft
                                                                    prop.children [
                                                                        Html.i [
                                                                            prop.classes [ "fas"; "fa-font" ]
                                                                        ]
                                                                    ]
                                                                ]
                                                                Bulma.icon [
                                                                    icon.isSmall
                                                                    icon.isRight
                                                                    prop.children [
                                                                        Html.i [
                                                                            prop.classes [ "fas"; "fa-check" ]
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                    ]
                                                ]
                                                Bulma.field.div [
                                                    Bulma.label [
                                                        Bulma.input.checkbox [ prop.value "remember" ]
                                                        Bulma.text.span "Optional"
                                                    ]
                                                ]
                                                Bulma.field.div [
                                                    Bulma.label [
                                                        Bulma.input.checkbox [ prop.value "remember" ]
                                                        Bulma.text.span "Deprecated"
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                            )
                            |> React.fragment
                        ]
                    ]
                    StepBreak {
                        FormSpec = props.FormSpec
                        OnChange = (fun formSpec ->
                            // add new step
                            let newStep = {
                                StepOrder = props.FormSpec.Steps |> List.length
                                StepLabel = "New Step"
                                // Icon = "fas fa-font"
                                Fields = [
                                    {
                                        FieldOrder = 0
                                        FieldKey = "new-field"
                                        FieldType = FieldType.Text { Value = None}
                                        Label = "New Field"
                                        IsOptional = false
                                        IsDeprecated = false
                                        DependsOn = None
                                    }
                                ]
                            }
                            let newFormSpec = {
                                props.FormSpec with
                                    Steps = props.FormSpec.Steps @ [ newStep ]
                            }
                            props.OnChange newFormSpec
                        )
                    }
                ]
            ]
        ) |> React.fragment
        // Html.div [
        //     prop.className classes.stepBreak
        //     prop.children [
        //         Html.button [
        //             prop.className classes.buttonMinimal
        //             prop.text "+ ADD STEP"
        //             prop.onClick (fun _ ->
        //                 //add new step
        //                 let newStep = {
        //                     StepOrder = props.FormSpec.Steps |> List.length
        //                     StepLabel = "New Step"
        //                     // Icon = "fas fa-font"
        //                     Fields = [
        //                         {
        //                             FieldOrder = 0
        //                             FieldKey = "new-field"
        //                             FieldType = FieldType.Text { Value = None}
        //                             Label = "New Field"
        //                             IsOptional = false
        //                             IsDeprecated = false
        //                             DependsOn = None
        //                         }
        //                     ]
        //                 }
        //                 let newFormSpec = {
        //                     props.FormSpec with
        //                         Steps = props.FormSpec.Steps @ [ newStep ]
        //                 }
        //                 props.OnChange newFormSpec
        //             )
        //         ]
        //     ]
        // ]

        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.className "field-label"
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.classes [ "field"; "is-expanded" ]
        //                     prop.children [
        //                         Html.div [
        //                             prop.classes [ "field"; "has-addons" ]
        //                             prop.children [
        //                                 Html.p [
        //                                     prop.className "control"
        //                                     prop.children [
        //                                         Html.a [
        //                                             prop.classes [ "button"; "is-static" ]
        //                                             prop.text "+44"
        //                                         ]
        //                                     ]
        //                                 ]
        //                                 Html.p [
        //                                     prop.classes [ "control"; "is-expanded" ]
        //                                     prop.children [
        //                                         Html.input [
        //                                             prop.className "input"
        //                                             prop.type' "tel"
        //                                             prop.placeholder "Your phone number"
        //                                         ]
        //                                     ]
        //                                 ]
        //                             ]
        //                         ]
        //                         Html.p [
        //                             prop.className "help"
        //                             prop.text "Do not enter the first zero"
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.classes [ "field-label"; "is-normal" ]
        //             prop.children [
        //                 Html.label [
        //                     prop.className "label"
        //                     prop.text "Department"
        //                 ]
        //             ]
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.classes [ "field"; "is-narrow" ]
        //                     prop.children [
        //                         Html.div [
        //                             prop.className "control"
        //                             prop.children [
        //                                 Html.div [
        //                                     prop.classes [ "select"; "is-fullwidth" ]
        //                                     prop.children [
        //                                         Html.select [
        //                                             Html.option "Business development"
        //                                             Html.option "Marketing"
        //                                             Html.option "Sales"
        //                                         ]
        //                                     ]
        //                                 ]
        //                             ]
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.className "field-label"
        //             prop.children [
        //                 Html.label [
        //                     prop.className "label"
        //                     prop.text "Already a member?"
        //                 ]
        //             ]
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.classes [ "field"; "is-narrow" ]
        //                     prop.children [
        //                         Html.div [
        //                             prop.className "control"
        //                             prop.children [
        //                                 Html.label [
        //                                     prop.className "radio"
        //                                     prop.children [
        //                                         Html.input [
        //                                             prop.type' "radio"
        //                                             prop.name "member"
        //                                         ]
        //                                         Html.text " Yes"
        //                                     ]
        //                                 ]
        //                                 Html.label [
        //                                     prop.className "radio"
        //                                     prop.children [
        //                                         Html.input [
        //                                             prop.type' "radio"
        //                                             prop.name "member"
        //                                         ]
        //                                         Html.text " No"
        //                                     ]
        //                                 ]
        //                             ]
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.classes [ "field-label"; "is-normal" ]
        //             prop.children [
        //                 Html.label [
        //                     prop.className "label"
        //                     prop.text "Subject"
        //                 ]
        //             ]
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.className "field"
        //                     prop.children [
        //                         Html.div [
        //                             prop.className "control"
        //                             prop.children [
        //                                 Html.input [
        //                                     prop.classes [ "input"; "is-danger" ]
        //                                     prop.type' "text"
        //                                     prop.placeholder "e.g. Partnership opportunity"
        //                                 ]
        //                             ]
        //                         ]
        //                         Html.p [
        //                             prop.classes [ "help"; "is-danger" ]
        //                             prop.text " This field is required"
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.classes [ "field-label"; "is-normal" ]
        //             prop.children [
        //                 Html.label [
        //                     prop.className "label"
        //                     prop.text "Question"
        //                 ]
        //             ]
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.className "field"
        //                     prop.children [
        //                         Html.div [
        //                             prop.className "control"
        //                             prop.children [
        //                                 Html.textarea [
        //                                     prop.className "textarea"
        //                                     prop.placeholder "Explain how we can help you"
        //                                 ]
        //                             ]
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
        // Html.div [
        //     prop.classes [ "field"; "is-horizontal" ]
        //     prop.children [
        //         Html.div [
        //             prop.className "field-label"
        //         ]
        //         Html.div [
        //             prop.className "field-body"
        //             prop.children [
        //                 Html.div [
        //                     prop.className "field"
        //                     prop.children [
        //                         Html.div [
        //                             prop.className "control"
        //                             prop.children [
        //                                 Html.button [
        //                                     prop.classes [ "button"; "is-primary" ]
        //                                     prop.text "Send message"
        //                                 ]
        //                             ]
        //                         ]
        //                     ]
        //                 ]
        //             ]
        //         ]
        //     ]
        // ]
    ]

[<ReactComponent>]
let DynamicFormBuilder() =
    let location = useLocation()

    let formSpec, setFormSpec = React.useState defaultFormSpec
    // let dynamicForm, setDynamicForm = React.useState defaultDynamicForm

    React.fragment [
        Bulma.container [
            Bulma.columns [
                Bulma.column [
                    column.is3
                    prop.children [
                        Html.nav [
                            prop.className "panel"
                            prop.children [
                                Html.p [
                                    prop.className "panel-heading"
                                    prop.text "Components "
                                ]
                                Html.div [
                                    prop.className "panel-block"
                                    prop.children [
                                        Html.p [
                                            prop.classes [ "control"; "has-icons-left" ]
                                            prop.children [
                                                Html.input [
                                                    prop.className "input"
                                                    prop.type' "text"
                                                    prop.placeholder "Search"
                                                ]
                                                Html.span [
                                                    prop.classes [ "icon"; "is-left" ]
                                                    prop.children [
                                                        Html.i [
                                                            prop.classes [ "fas"; "fa-search" ]
                                                            // prop.ariaHidden "true"
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                                // Html.p [
                                //     prop.className "panel-tabs"
                                //     prop.children [
                                //         Html.a [
                                //             prop.className "is-active"
                                //             prop.text "All"
                                //         ]
                                //         Html.a [ prop.text "Field Type" ]
                                //     ]
                                // ]

                                availableComponents
                                |> List.map (fun field ->
                                    Html.a [
                                        prop.draggable true
                                        prop.onDragStart (fun e ->
                                            let _ = e.dataTransfer.setData("text/plain", field.Key)
                                            ()
                                        )
                                        prop.classes [ "panel-block"; "is-active" ]
                                        prop.children [
                                            Html.span [
                                                prop.className "panel-icon"
                                                prop.children [
                                                    Html.i [
                                                        prop.className field.Icon
                                                    ]
                                                ]
                                            ]
                                            Html.text field.Key
                                        ]
                                    ]
                                ) |> React.fragment

                                // Html.label [
                                //     prop.className "panel-block"
                                //     prop.children [
                                //         Html.input [
                                //             prop.type' "checkbox"
                                //         ]
                                //         Html.text " remember me"
                                //     ]
                                // ]
                                // Html.div [
                                //     prop.className "panel-block"
                                //     prop.children [
                                //         Html.button [
                                //             prop.classes [ "button"; "is-link"; "is-outlined"; "is-fullwidth" ]
                                //             prop.text "Reset all filters"
                                //         ]
                                //     ]
                                // ]
                            ]
                        ]
                    ]
                ]
                FormSpecLayout {
                    FormSpec = formSpec
                    OnChange = setFormSpec
                }
                // Bulma.column [
                //     Html.div [
                //         prop.style [
                //             // style.backgroundColor "lightgray"
                //             // style.height 100
                //         ]
                //         prop.onDragOver (fun (e: Types.DragEvent) ->
                //             e.preventDefault();
                //             ()
                //         )
                //         prop.onDrop (fun (e: Types.DragEvent) ->
                //             e.preventDefault();
                //             let data = e.dataTransfer.getData("text/plain")
                //             debuglog data
                //             // let builderField = availableComponents |> List.find (fun field -> field.Key = data)
                //             // match builderField with
                //             // | BuilderField.Step ->
                //             //     let newStep = {
                //             //         Id = Guid.NewGuid()
                //             //         Title = "New Step"
                //             //         Abstract = ""
                //             //         Fields = []
                //             //     }
                //             //     let newFormSpec = {
                //             //         formSpec with
                //             //         Steps = formSpec.Steps @ [ newStep ]
                //             //     }
                //             //     setFormSpec newFormSpec
                //             // | _ -> ()
                //             ()
                //         )
                //         prop.children [
                //             Bulma.field.div [
                //                 Bulma.label [
                //                     prop.className "label"
                //                     prop.text "Title"
                //                 ]
                //                 Bulma.control.div [
                //                     Bulma.input.text [
                //                         prop.placeholder "Name of the form as it will appear in the assessments list"
                //                         prop.value formSpec.Title
                //                         prop.onChange (fun e ->
                //                             setFormSpec ({
                //                                 formSpec with
                //                                     Title = e
                //                             })
                //                         )
                //                     ]
                //                 ]
                //             ]
                //             Bulma.field.div [
                //                 Bulma.label [
                //                     prop.className "label"
                //                     prop.text "Code"
                //                 ]
                //                 Bulma.control.div [
                //                     Bulma.input.text [
                //                         prop.placeholder "This is the code that will be used to reference the form in the API"
                //                         prop.value formSpec.Code
                //                         prop.onChange (fun e ->
                //                             setFormSpec ({
                //                                 formSpec with
                //                                     Code = e
                //                             })
                //                         )
                //                     ]
                //                 ]
                //             ]
                //         ]
                //     ]
                // ]
            ]
            FormCompose.FormCompose({|
                FormSpec = formSpec
                DynamicForm = None
                Mode = FormComposeMode.Editable
                NavigateToStep = (fun _ -> ())
                FormChanged = (fun _ -> ())
                SaveFormValuesCallback = (fun _ -> ())
            |})
        ]

    ]
