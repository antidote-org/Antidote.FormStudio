module Antidote.React.FormDesigner.Designer.CategoryTags

open Feliz
open Feliz.Bulma
open Antidote.FormStudio.Types

type CategoryTagsProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        OnChange: FormSpec<'UserField> -> unit
    |}

let private availableCategoryTags =
    [
        MentalHealth
        IllicitDrugs
        Alcohol
        RiskScore
        COPD
        HRA
        HeartDisease
        GeneralWellness
        KidneyDisease
        Diabetes
        Hospital
        MedAdherance
    ]
    |> List.sort

[<ReactComponent>]
let CategoryTags (props: CategoryTagsProps<'UserField>) =
    Bulma.field.div [
        field.isHorizontal
        prop.children [
            Bulma.fieldLabel [
                fieldLabel.isNormal
                prop.children [
                    Bulma.label "Tags"
                ]
            ]
            Bulma.fieldBody [
                Bulma.field.div [
                    prop.children [
                        let toOption (categoryTag: CategoryTag) =
                            let isChecked = List.contains categoryTag props.FormSpec.CategoryTags

                            Bulma.tag [
                                tag.isRounded
                                prop.style [
                                    style.cursor.pointer
                                ]
                                prop.key categoryTag.toString

                                if isChecked then
                                    color.isInfo

                                prop.onClick (fun _ ->

                                    // Compute the new state
                                    let newCategorTags =
                                        if isChecked then
                                            props.FormSpec.CategoryTags
                                            |> List.filter (fun x -> x <> categoryTag)
                                        else
                                            categoryTag :: props.FormSpec.CategoryTags

                                    // Save the new state
                                    props.OnChange
                                        { props.FormSpec with
                                            CategoryTags = newCategorTags
                                        }
                                )

                                prop.text categoryTag.toString
                            ]

                        Bulma.tags [
                            yield! availableCategoryTags |> List.map toOption
                        ]
                    ]
                ]
            ]
        ]
    ]
