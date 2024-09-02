namespace Antidote.Form.Designer.Bulma.Internal

open Feliz
open Feliz.Bulma
open Elmish
open Fable.Form
open Antidote.Form.Designer
open Antidote.Form.Designer.Form.View
open Antidote.Form.Designer.Bulma

module View =

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

    let rec renderField
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (field: FilledField<'Values, 'Attributes>)
        : ReactElement
        =

        let blur label =
            Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

        field.State.RenderField blur dispatch fieldConfig field
