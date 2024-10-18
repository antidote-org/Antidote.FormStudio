module Antidote.FormStudio.UI.Designer.SpecScore

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Antidote.FormStudio.Types
open Antidote.FormStudio.UI.Components.Switch
open Antidote.FormStudio.UI.Components.ButtonMinimal

let private classes: CssModules.DynamicFormDesigner =
    import "default" "./DynamicFormDesigner.module.scss"

type SpecScoreProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        OnChange: FormSpec<'UserField> -> unit
    |}

[<ReactComponent>]
let SpecScore (props: SpecScoreProps<'UserField>) =
    // let scoreRange, setScoreRange = React.useState<ScoreRange list> []
    let tagDropDownOpenId, setTagDropDownOpenId = React.useState (System.Guid.Empty)
    let enabled, setEnabled = React.useState (props.FormSpec.Score.IsSome)

    React.useEffect (
        fun _ -> setEnabled props.FormSpec.Score.IsSome
        , [|
            box props.FormSpec
        |]
    )

    React.fragment [
        Bulma.field.div [
            field.isHorizontal
            field.hasAddons
            prop.style [
                style.display.flex
                style.alignItems.center
            ]
            prop.children [
                Bulma.fieldLabel [
                    fieldLabel.isNormal
                    prop.children [
                        Bulma.label "Form Score"
                    ]
                ]
                Bulma.fieldBody [
                    Switch
                        {|
                            OnChange =
                                fun e ->
                                    if not e then
                                        props.OnChange
                                            { props.FormSpec with
                                                Score = None
                                            }

                                    setEnabled e
                            IsEnabled = enabled
                        |}
                ]
            ]
        ]

        if enabled then
            Bulma.field.div [
                field.isHorizontal
                field.hasAddons
                prop.children [
                    Bulma.fieldLabel [
                        fieldLabel.isNormal
                        prop.children [
                            Bulma.label "Max"
                        ]
                    ]
                    Bulma.fieldBody [
                        Bulma.field.div [
                            field.isNarrow
                            prop.children [
                                Bulma.input.number [
                                    input.isSmall
                                    prop.defaultValue (
                                        props.FormSpec.Score
                                        |> Option.map (fun s -> s.MaxScore)
                                        |> Option.defaultValue 0
                                    )
                                    prop.onChange (fun (e: int) ->
                                        props.OnChange(
                                            match props.FormSpec.Score with
                                            | Some score ->
                                                { props.FormSpec with
                                                    Score =
                                                        Some
                                                            { score with
                                                                MaxScore = e
                                                            }
                                                }
                                            | None ->
                                                { props.FormSpec with
                                                    Score =
                                                        Some
                                                            {
                                                                MaxScore = e
                                                                ScoreRanges = []
                                                            }
                                                }
                                        )
                                    )
                                ]
                            ]
                        ]
                        Bulma.field.div [
                            field.isHorizontal
                            field.hasAddons
                            prop.children [
                                Bulma.fieldBody [
                                    ButtonMinimal
                                        {|
                                            Text = "+ ADD RANGE"
                                            OnClick =
                                                fun _ ->
                                                    let newScoreRange =
                                                        {
                                                            Id = System.Guid.NewGuid()
                                                            Min = 0
                                                            Max = 0
                                                            Label = ""
                                                            Tag = Unspecified
                                                        }

                                                    let newFormSpec =
                                                        { props.FormSpec with
                                                            Score =
                                                                match props.FormSpec.Score with
                                                                | Some score ->
                                                                    Some
                                                                        { score with
                                                                            ScoreRanges =
                                                                                score.ScoreRanges
                                                                                @ [
                                                                                    newScoreRange
                                                                                ]
                                                                        }
                                                                | None ->
                                                                    Some
                                                                        {
                                                                            MaxScore = 0
                                                                            ScoreRanges =
                                                                                [
                                                                                    newScoreRange
                                                                                ]
                                                                        }
                                                        }

                                                    props.OnChange newFormSpec
                                        |}
                                ]
                            ]
                        ]
                    ]
                ]
            ]

            yield!
                match props.FormSpec.Score with
                | None -> []
                | Some score ->
                    score.ScoreRanges
                    |> List.map (fun r ->
                        Bulma.field.div [
                            field.isHorizontal
                            field.hasAddons
                            prop.children [
                                Bulma.fieldLabel [
                                    fieldLabel.isNormal
                                    prop.children [
                                        Bulma.label ""
                                    ]
                                ]
                                Bulma.fieldBody [
                                    Bulma.input.number [
                                        input.isSmall
                                        prop.defaultValue (r.Min)
                                        prop.onChange (fun (e: int) ->
                                            let newRange =
                                                { r with
                                                    Min = e
                                                }

                                            props.OnChange
                                                { props.FormSpec with
                                                    Score =
                                                        match props.FormSpec.Score with
                                                        | Some score ->
                                                            Some
                                                                { score with
                                                                    ScoreRanges =
                                                                        score.ScoreRanges
                                                                        |> List.map (fun r ->
                                                                            if
                                                                                r.Id = newRange.Id
                                                                            then
                                                                                newRange
                                                                            else
                                                                                r
                                                                        )
                                                                }
                                                        | None ->
                                                            Some
                                                                {
                                                                    MaxScore = 0
                                                                    ScoreRanges =
                                                                        [
                                                                            newRange
                                                                        ]
                                                                }
                                                }
                                        )
                                    ]
                                    Bulma.input.number [
                                        input.isSmall
                                        prop.defaultValue (r.Max)
                                        prop.onChange (fun (e: int) ->
                                            let newRange =
                                                { r with
                                                    Max = e
                                                }

                                            props.OnChange(
                                                { props.FormSpec with
                                                    Score =
                                                        match props.FormSpec.Score with
                                                        | Some score ->
                                                            Some
                                                                { score with
                                                                    ScoreRanges =
                                                                        score.ScoreRanges
                                                                        |> List.map (fun r ->
                                                                            if
                                                                                r.Id = newRange.Id
                                                                            then
                                                                                newRange
                                                                            else
                                                                                r
                                                                        )

                                                                }
                                                        | None ->
                                                            Some
                                                                {
                                                                    MaxScore = 0
                                                                    ScoreRanges =
                                                                        [
                                                                            newRange
                                                                        ]
                                                                }
                                                }
                                            )
                                        )
                                    ]
                                    Bulma.input.text [
                                        input.isSmall
                                        prop.placeholder "Label"
                                        prop.value r.Label
                                        prop.onChange (fun (e: string) ->
                                            let newRange =
                                                { r with
                                                    Label = e
                                                }

                                            props.OnChange
                                                { props.FormSpec with
                                                    Score =
                                                        match props.FormSpec.Score with
                                                        | Some score ->
                                                            Some
                                                                { score with
                                                                    ScoreRanges =
                                                                        score.ScoreRanges
                                                                        |> List.map (fun r ->
                                                                            if
                                                                                r.Id = newRange.Id
                                                                            then
                                                                                { r with
                                                                                    Label = e
                                                                                }
                                                                            else
                                                                                r
                                                                        )

                                                                }
                                                        | None ->
                                                            Some
                                                                {
                                                                    MaxScore = 0
                                                                    ScoreRanges =
                                                                        [
                                                                            r
                                                                        ]
                                                                }
                                                }
                                        )
                                    ]
                                    Html.div [
                                        prop.classes [
                                            "dropdown"
                                            "is-right"
                                            "is-small"
                                            (if tagDropDownOpenId = r.Id then
                                                 "is-active"
                                             else
                                                 "")
                                        ]
                                        prop.children [
                                            Html.div [
                                                prop.className "dropdown-trigger"
                                                prop.children [
                                                    Html.button [
                                                        prop.classes [
                                                            "button"
                                                            "is-small"
                                                            (Antidote.FormStudio.Helper.severityColorToClasses
                                                                r.Tag)
                                                        ]
                                                        prop.onClick (fun _ ->
                                                            setTagDropDownOpenId r.Id
                                                        )
                                                        prop.children [
                                                            // Html.span (if r.Tag = "" then "Tag" else r.Tag)
                                                            Html.span [
                                                                prop.classes [
                                                                    "icon"
                                                                    "is-small"
                                                                ]
                                                                prop.children [
                                                                    Html.i [
                                                                        prop.classes [
                                                                            "fas"
                                                                            "fa-angle-down"
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                    ]
                                                ]
                                            ]

                                            Html.div [
                                                prop.className "dropdown-menu"
                                                prop.id "dropdown-menu"
                                                prop.role "menu"
                                                prop.children [
                                                    Html.div [
                                                        prop.className "dropdown-content"
                                                        prop.children [
                                                            let tagRender
                                                                (severityColor: ScoreColor)
                                                                =
                                                                Html.a [

                                                                    prop.className (
                                                                        "dropdown-item "
                                                                        + (Antidote.FormStudio.Helper.severityColorToClasses
                                                                            severityColor)
                                                                    )
                                                                    prop.style [
                                                                        style.height 25
                                                                    ]
                                                                    // prop.text (severityColor.ToString())
                                                                    prop.onClick (fun _ ->
                                                                        let newRange =
                                                                            { r with
                                                                                Tag =
                                                                                    severityColor
                                                                            }

                                                                        props.OnChange
                                                                            { props.FormSpec with
                                                                                Score =
                                                                                    match
                                                                                        props.FormSpec.Score
                                                                                    with
                                                                                    | Some score ->
                                                                                        Some
                                                                                            { score with
                                                                                                ScoreRanges =
                                                                                                    score.ScoreRanges
                                                                                                    |> List.map (fun
                                                                                                                     r ->
                                                                                                        if
                                                                                                            r.Id = newRange.Id
                                                                                                        then
                                                                                                            newRange
                                                                                                        else
                                                                                                            r
                                                                                                    )

                                                                                            }
                                                                                    | None ->
                                                                                        Some
                                                                                            {
                                                                                                MaxScore =
                                                                                                    0
                                                                                                ScoreRanges =
                                                                                                    [
                                                                                                        newRange
                                                                                                    ]
                                                                                            }
                                                                            }

                                                                        setTagDropDownOpenId (
                                                                            System.Guid.Empty
                                                                        )
                                                                    )
                                                                ]

                                                            [
                                                                // White
                                                                // Black
                                                                // Light
                                                                // Dark
                                                                Unspecified
                                                                Primary
                                                                Link
                                                                Info
                                                                Success
                                                                Warning
                                                                Danger
                                                            ]
                                                            |> List.map tagRender
                                                            |> React.fragment
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    )

    ]
