module Antidote.FormStudio.UI.DynamicFormSpecDetails

open System
open Feliz
open Feliz.Bulma
open Antidote.FormStudio.Types
open Antidote.FormStudio.UI.Components.BulmaHorizontalField

type DynamicFormSpecDetailsProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        OnChange: FormSpec<'UserField> -> unit
        ActiveField: ActiveField
        SetIsPreview: (bool -> unit) option
    |}

[<ReactComponent>]
let DynamicFormSpecDetails (props: DynamicFormSpecDetailsProps<'UserField>) =
    let formSpecJson, setFormSpecJson = React.useState ""

    let formSpecBase64, setFormSpecBase64 = React.useState ""

    React.fragment [
        Html.hr []

        // Preview button toolbar
        match props.SetIsPreview with
        | Some setIsPreview ->
            Bulma.field.div [
                field.isGrouped
                field.isGroupedRight
                prop.style [
                    style.marginBottom 20
                ]
                prop.children [
                    Bulma.control.div [
                        Bulma.button.button [
                            color.isPrimary
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-eye"
                                    prop.style [
                                        style.marginRight 5
                                    ]
                                ]
                                Html.text "Preview Form"
                            ]
                            prop.onClick (fun _ -> setIsPreview true)
                        ]
                    ]
                ]
            ]
        | None -> Html.none

        Bulma.columns [
            Bulma.column [

                BulmaHorizontalField
                    {|
                        Label = Some "Title"
                        Placeholder = "Name as it will appear in the assessments list"
                        Value = props.FormSpec.Title
                        Layout = BulmaFieldLayout.Horizontal
                        OnChange =
                            (fun e ->
                                props.OnChange(
                                    { props.FormSpec with
                                        Id = Guid.NewGuid()
                                        Title = e
                                    }
                                )
                            )
                        LeftIcon = Some("fas fa-heading")
                        RightIcon = None
                        ActiveField = props.ActiveField
                    |}
                BulmaHorizontalField
                    {|
                        Label = Some "Abstract"
                        Placeholder = "Short description of the form, and its purpose"
                        Value = props.FormSpec.Abstract
                        Layout = BulmaFieldLayout.Horizontal
                        OnChange =
                            (fun e ->
                                props.OnChange(
                                    { props.FormSpec with
                                        Abstract = e
                                    }
                                )
                            )
                        LeftIcon = Some("fas fa-paragraph")
                        RightIcon = None
                        ActiveField = props.ActiveField
                    |}

                Antidote.React.FormDesigner.Designer.CategoryTags.CategoryTags
                    {|
                        FormSpec = props.FormSpec
                        OnChange = props.OnChange
                    |}

                Antidote.React.FormDesigner.Designer.AssociatedCodes.AssociatedCodes
                    {|
                        FormSpec = props.FormSpec
                        OnChange = props.OnChange
                    |}

            // SpecScore
            //     {|
            //         FormSpec = props.FormSpec
            //         OnChange = props.OnChange
            //     |}
            ]
        ]
    ]
