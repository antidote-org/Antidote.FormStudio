module Fable.Form.Studio.Bulma

open Fable.Form.Studio.Field

[<RequireQualifiedAccess>]
module Form =

    module View =

        open Feliz
        open Feliz.Bulma
        open Fable.Form
        open Fable.Form.Studio
        open Fable.Form.Studio.Form.View
        open Fable.Form.Studio.View.Form.View

        let fieldLabel (label: string) = Bulma.label [ prop.text label ]

        let errorMessage (message: string) =
            Bulma.help [
                color.isDanger
                prop.text message
            ]

        let errorMessageAsHtml (showError: bool) (error: Error.Error option) =
            match error with
            | Some(Error.External externalError) -> errorMessage externalError

            | _ ->
                if showError then
                    error
                    |> Option.map errorToString
                    |> Option.map errorMessage
                    |> Option.defaultValue (Bulma.help [])

                else
                    Bulma.help []

        let wrapInFieldContainer (children: ReactElement list) =
            Bulma.field.div [ prop.children children ]

        let withLabelAndError
            (label: string)
            (showError: bool)
            (error: Error.Error option)
            (fieldAsHtml: ReactElement)
            : ReactElement
            =
            [
                fieldLabel label
                Bulma.control.div [ fieldAsHtml ]
                errorMessageAsHtml showError error
            ]
            |> wrapInFieldContainer

        let checkboxField
            ({
                 Dispatch = dispatch
                 OnChange = onChange
                 OnBlur = onBlur
                 Disabled = disabled
                 Value = value
                 Attributes = attributes
             }: CheckboxFieldConfig<'Msg>)
            =

            Bulma.control.div [
                Bulma.input.labels.checkbox [
                    prop.children [
                        Bulma.input.checkbox [
                            prop.onChange (onChange >> dispatch)
                            match onBlur with
                            | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                            | None -> ()
                            prop.disabled disabled
                            prop.isChecked value
                        ]

                        Html.text attributes.Text
                    ]
                ]
            ]
            |> (fun x -> [ x ])
            |> wrapInFieldContainer

        let radioField
            ({
                 Dispatch = dispatch
                 OnChange = onChange
                 OnBlur = onBlur
                 Disabled = disabled
                 Value = value
                 Error = error
                 ShowError = showError
                 Attributes = attributes
             }: RadioFieldConfig<'Msg>)
            =

            let radio (key: string, label: string) =
                Bulma.input.labels.radio [
                    Bulma.input.radio [
                        prop.name attributes.Label
                        prop.isChecked (key = value: bool)
                        prop.disabled disabled
                        prop.onChange (fun (_: bool) -> onChange key |> dispatch)
                        match onBlur with
                        | Some onBlur -> prop.onBlur (fun _ -> dispatch onBlur)

                        | None -> ()
                    ]

                    Html.text label
                ]

            Bulma.control.div [ attributes.Options |> List.map radio |> prop.children ]
            |> withLabelAndError attributes.Label showError error

        let form
            ({
                 Dispatch = dispatch
                 OnSubmit = onSubmit
                 State = state
                 Action = action
                 Fields = fields
             }: FormConfig<'Msg>)
            =

            let innerForm =
                Html.form [
                    prop.onSubmit (fun ev ->
                        ev.stopPropagation ()
                        ev.preventDefault ()

                        onSubmit |> Option.map dispatch |> Option.defaultWith ignore
                    )

                    prop.children [
                        yield! fields

                        match state with
                        | Error error -> errorMessage error

                        | Success success ->
                            Bulma.field.div [
                                Bulma.control.div [
                                    text.hasTextCentered
                                    color.hasTextSuccess
                                    text.hasTextWeightBold

                                    prop.text success
                                ]
                            ]

                        | Loading
                        | Idle -> Html.none

                        match action with
                        | Action.SubmitOnly submitLabel ->
                            Bulma.field.div [
                                field.isGrouped
                                field.isGroupedRight

                                prop.children [
                                    Bulma.control.div [
                                        Bulma.button.button [
                                            prop.type'.submit
                                            color.isPrimary
                                            prop.text submitLabel
                                            // If the form is loading animate the submit button with the loading animation
                                            if state = Loading then
                                                button.isLoading
                                        ]
                                    ]

                                ]
                            ]

                        | Action.Custom func -> func state dispatch
                    ]
                ]

            Html.div [
                prop.className "form-designer"
                prop.children [
                    innerForm

                    Html.div [
                        prop.id "field-properties-portal"
                    ]
                ]
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg, IReactProperty> =
            {
                CheckboxField = checkboxField
                RadioField = radioField
            }

        let asHtml (config: ViewConfig<'Values, 'Msg>) =
            custom config form (renderField htmlViewConfig)
