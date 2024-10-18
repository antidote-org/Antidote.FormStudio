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
    |}

[<ReactComponent>]
let DynamicFormSpecDetails (props: DynamicFormSpecDetailsProps<'UserField>) =
    let formSpecJson, setFormSpecJson = React.useState ""

    let formSpecBase64, setFormSpecBase64 = React.useState ""

    React.fragment [
        Html.hr []
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
