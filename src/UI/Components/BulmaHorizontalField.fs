module Antidote.FormStudio.UI.Components.BulmaHorizontalField

open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Antidote.FormStudio

open Antidote.FormStudio.Types

[<RequireQualifiedAccess>]
type BulmaFieldLayout =
    | Horizontal
    | Vertical

let private classes: CssModules.DynamicFormDesigner =
    import "default" "./../../DynamicFormDesigner.module.scss"

let private renderIcon (position: IReactProperty) (iconClass: string) =
    Bulma.icon [
        icon.isSmall
        position
        prop.children [
            Html.i [
                prop.classes [
                    iconClass
                ]
            ]
        ]
    ]

type private BulmaHorizontalFieldProps =
    {|
        Label: string option
        Placeholder: string
        Value: string
        Layout: BulmaFieldLayout
        OnChange: string -> unit
        LeftIcon: string option
        RightIcon: string option
        ActiveField: ActiveField
    |}

[<ReactComponent>]
let BulmaHorizontalField (props: BulmaHorizontalFieldProps) =
    Bulma.field.div [
        prop.style [
            style.display.flex
            style.alignItems.center
        ]

        if props.Layout = BulmaFieldLayout.Horizontal then
            field.isHorizontal

        prop.className [
            if props.ActiveField.State = AddingDependantKeys then
                GlobalCSS.classes.disabled
        ]
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
                            if props.LeftIcon |> Option.isSome then
                                control.hasIconsLeft
                            if props.RightIcon |> Option.isSome then
                                control.hasIconsRight
                            prop.children [
                                Bulma.input.text [
                                    input.isSmall
                                    prop.placeholder props.Placeholder
                                    prop.value props.Value
                                    prop.onChange props.OnChange
                                ]

                                props.LeftIcon
                                |> Option.map (renderIcon icon.isLeft)
                                |> Option.defaultValue Html.none

                                props.RightIcon
                                |> Option.map (renderIcon icon.isRight)
                                |> Option.defaultValue Html.none
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
