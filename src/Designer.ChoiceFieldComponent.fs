module Antidote.FormDesigner.ChoiceFieldComponent

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop

open Antidote.Core.FormProcessor
open Antidote.Core.FormProcessor.Spec.v2_0_1

open Antidote.FormDesigner.Types

let private classes: CssModules.DynamicFormDesigner =
    import "default" "./DynamicFormDesigner.module.scss"

type TextFieldComponentProps =
    {|
        FormSpec: FormSpec
        FormStep: FormStep
        FormField: FormField
        ActiveField: ActiveField
        SetActiveField: ActiveField -> unit
        OnChange: FormSpec -> unit
    |}

[<ReactComponent>]
let ChoiceFieldComponent (props: TextFieldComponentProps) =
    let iconString =
        match props.FormField.FieldType with
        | TagList _ -> "fas fa-tags"
        | MultiChoice f -> "fas fa-check"
        | CheckboxList f -> "fas fa-check"
        | Radio _
        | SingleChoice _ -> "fas fa-dot-circle"
        | Dropdown _ -> "fas fa-caret-down"
        | TextAutoComplete _ -> "fas fa-font"
        | _ -> failwith "Not a choice field"

    let optionDescription =
        match props.FormField.FieldType with
        | TagList _ -> "Tag"
        | MultiChoice _ -> "Choice"
        | CheckboxList _ -> "Check"
        | Radio _
        | SingleChoice _ -> "Option"
        | Dropdown _ -> "Selection"
        | TextAutoComplete _ -> "Choice"
        | _ -> failwith "Not a choice field"

    let renderOption (index: int) (option: Spec.v2_0_1.FieldOption) =
        React.fragment
            [
                //COMBINED
                Html.div
                    [
                        prop.classes [ "field"; "is-horizontal" ]
                        prop.children
                            [
                                Html.div
                                    [
                                        prop.className "field-body"
                                        prop.children
                                            [
                                                Html.div
                                                    [
                                                        prop.classes [ "field"; "is-expanded" ]
                                                        prop.children
                                                            [
                                                                Html.div
                                                                    [
                                                                        prop.classes
                                                                            [
                                                                                "field"
                                                                                "has-addons"
                                                                            ]
                                                                        prop.children
                                                                            [
                                                                                Html.p
                                                                                    [
                                                                                        prop.className
                                                                                            "control"
                                                                                        prop.children
                                                                                            [
                                                                                                Html.a
                                                                                                    [
                                                                                                        prop.classes
                                                                                                            [
                                                                                                                "button"
                                                                                                                "is-static"
                                                                                                            ]
                                                                                                        prop.children
                                                                                                            [
                                                                                                                Html.i
                                                                                                                    [
                                                                                                                        prop.className
                                                                                                                            iconString
                                                                                                                    ]
                                                                                                            ]
                                                                                                    ]
                                                                                            ]
                                                                                    ]
                                                                                Html.div
                                                                                    [
                                                                                        prop.classes
                                                                                            [
                                                                                                "control"
                                                                                                "is-expanded"
                                                                                            ]
                                                                                        prop.children
                                                                                            [
                                                                                                Html.input
                                                                                                    [
                                                                                                        prop.className
                                                                                                            "input"
                                                                                                        prop.placeholder
                                                                                                            "Option"
                                                                                                        prop.value
                                                                                                            option.Description
                                                                                                        prop.onChange (fun
                                                                                                                           (e:
                                                                                                                               string) ->
                                                                                                            let updatedOption =
                                                                                                                { option with
                                                                                                                    Description =
                                                                                                                        e
                                                                                                                }

                                                                                                            let newFormField =
                                                                                                                Helper.updateOptionInField
                                                                                                                    props.FormField
                                                                                                                    updatedOption

                                                                                                            let newFormSpec =
                                                                                                                Helper.updateFormFieldInFormSpecStep
                                                                                                                    newFormField
                                                                                                                    props.FormStep
                                                                                                                    props.FormSpec

                                                                                                            props.OnChange
                                                                                                                newFormSpec
                                                                                                        )
                                                                                                    ]
                                                                                            ]
                                                                                    ]
                                                                                Html.div
                                                                                    [
                                                                                        prop.classes
                                                                                            [
                                                                                                "control"
                                                                                                "is-expanded"
                                                                                            ]
                                                                                        prop.children
                                                                                            [
                                                                                                Html.input
                                                                                                    [
                                                                                                        prop.className
                                                                                                            "input"
                                                                                                        prop.placeholder
                                                                                                            "Value"
                                                                                                        prop.value
                                                                                                            option.Value
                                                                                                        prop.onChange (fun
                                                                                                                           (e:
                                                                                                                               string) ->
                                                                                                            let updatedOption =
                                                                                                                { option with
                                                                                                                    Value =
                                                                                                                        e
                                                                                                                }

                                                                                                            let newFormField =
                                                                                                                Helper.updateOptionInField
                                                                                                                    props.FormField
                                                                                                                    updatedOption

                                                                                                            let newFormSpec =
                                                                                                                Helper.updateFormFieldInFormSpecStep
                                                                                                                    newFormField
                                                                                                                    props.FormStep
                                                                                                                    props.FormSpec

                                                                                                            props.OnChange
                                                                                                                newFormSpec
                                                                                                        )
                                                                                                    ]
                                                                                            ]
                                                                                    ]

                                                                                Html.div
                                                                                    [
                                                                                        prop.className
                                                                                            "control"
                                                                                        prop.children
                                                                                            [
                                                                                                Bulma.button.a
                                                                                                    [
                                                                                                        //color.isDanger
                                                                                                        prop.onClick (fun
                                                                                                                          _ ->
                                                                                                            let newFormField =
                                                                                                                Helper.deleteOptionInFormField
                                                                                                                    props.FormField
                                                                                                                    option

                                                                                                            let newFormSpec =
                                                                                                                Helper.updateFormFieldInFormSpecStep
                                                                                                                    newFormField
                                                                                                                    props.FormStep
                                                                                                                    props.FormSpec

                                                                                                            props.OnChange
                                                                                                                newFormSpec
                                                                                                        )
                                                                                                        prop.children
                                                                                                            [
                                                                                                                // Html.i [
                                                                                                                //     prop.children [
                                                                                                                //         Icon [
                                                                                                                //             icon.icon mdi.closeCircle
                                                                                                                //             icon.width 15
                                                                                                                //             icon.height 15
                                                                                                                //             icon.color "red"
                                                                                                                //         ]
                                                                                                                //     ]
                                                                                                                // ]
                                                                                                                Html.text
                                                                                                                    "Delete"
                                                                                                            ]
                                                                                                    ]
                                                                                            ]
                                                                                    ]
                                                                            ]
                                                                    ]
                                                            // Html.p [
                                                            //     prop.className "help"
                                                            //     prop.text "Do not enter the first zero"
                                                            // ]
                                                            ]
                                                    ]
                                            ]
                                    ]
                            ]
                    ]
            ]

    React.fragment
        [
            props.FormField
            |> Helper.getFieldOptions
            |> List.mapi (fun i o -> renderOption i o)
            |> React.fragment

            Html.button
                [
                    prop.className classes.buttonMinimal
                    prop.text "+ ADD OPTION"
                    prop.onClick (fun _ ->
                        let optionNumber = Helper.getFieldOptionsCount props.FormField

                        let newOption =
                            Helper.createOption
                                (optionDescription + " " + optionNumber)
                                (optionNumber)

                        let newFormField =
                            Helper.createFormFieldWithOptions props.FormField [ newOption ]

                        let newFormSpec =
                            Helper.updateFormFieldInFormSpecStep
                                newFormField
                                props.FormStep
                                props.FormSpec

                        props.OnChange newFormSpec
                    )
                ]

            Html.div
                [
                    prop.className "file is-centered is-small"
                    prop.children
                        [
                            Html.label
                                [
                                    prop.className "file-label"
                                    prop.children
                                        [
                                            Html.input
                                                [
                                                    prop.className "file-input"
                                                    prop.type' "file"
                                                    prop.onInput (fun filesArray ->
                                                        let filesArray: Browser.Types.FileList =
                                                            unbox filesArray?target?files

                                                        let fileReader =
                                                            Browser.Dom.FileReader.Create()

                                                        fileReader.onload <-
                                                            (fun (e: Browser.Types.Event) ->
                                                                let parserResult = "" //papa?parse$(fileReader.result, parserOptions)

                                                                let options =
                                                                    unbox<string array array>
                                                                        parserResult?data
                                                                    |> Array.skip 1
                                                                    |> Array.map (fun cols ->
                                                                        Helper.createOption
                                                                            cols[0]
                                                                            cols[1]
                                                                    )
                                                                    |> Array.sort
                                                                    |> Array.toList

                                                                let newFormField =
                                                                    Helper.createFormFieldWithOptions
                                                                        props.FormField
                                                                        options

                                                                let newFormSpec =
                                                                    Helper.updateFormFieldInFormSpecStep
                                                                        newFormField
                                                                        props.FormStep
                                                                        props.FormSpec

                                                                props.OnChange newFormSpec
                                                            )

                                                        fileReader.readAsText (filesArray.item 0)
                                                    )
                                                ]
                                            Html.span
                                                [
                                                    prop.className "file-cta"
                                                    prop.children
                                                        [
                                                            Html.span
                                                                [
                                                                    prop.className "file-icon"
                                                                    prop.children
                                                                        [
                                                                            Html.i
                                                                                [
                                                                                    prop.className
                                                                                        "fas fa-file-csv"
                                                                                ]
                                                                        ]
                                                                ]
                                                            Html.span
                                                                [
                                                                    prop.className "file-label"
                                                                    prop.text "+ LOAD FROM FILE"
                                                                ]
                                                        ]
                                                ]
                                        ]
                                ]
                        ]
                ]
        ]

// Fable.Form.Designer.Fields
// List of fields made to go inside of property editors
