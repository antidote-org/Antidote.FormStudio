module Antidote.React.FormDesigner.Designer.CategoryTags

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Antidote.Core.FormProcessor.Spec.Types

let private classes : CssModules.DynamicFormDesigner = import "default" "./DynamicFormDesigner.module.scss"


type CategoryTagsProps = {|
    FormSpec: FormSpec
    OnChange: FormSpec -> unit
|}

[<ReactComponent>]
let CategoryTags(props:CategoryTagsProps) =
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
                        let toOption (categoryTag:CategoryTag) =
                            let isChecked =
                                List.contains categoryTag props.FormSpec.CategoryTags

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
                                    props.OnChange {
                                        props.FormSpec with
                                            CategoryTags = newCategorTags
                                    }
                                )

                                prop.text categoryTag.toString
                            ]

                        Bulma.tags [
                            yield! availableCategoryTags
                            |> List.map toOption
                        ]
                    ]
                ]
            ]
        ]
    ]
