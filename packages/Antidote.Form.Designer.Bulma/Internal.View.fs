namespace Antidote.Form.Designer.Bulma.Internal

open System
open Feliz
open Feliz.Bulma
open Feliz.Listeners
open Elmish
open Fable.Form
open Antidote.Form.Designer
open Antidote.Form.Designer.Form.View
open Antidote.Form.Designer.Bulma
open Fable.Core.JsInterop
open Browser
open Browser.Types

module View =

    type private PreviewContainerEvent =
        {
            ActiveGuid: Guid
        }

    [<Literal>]
    let PREVIEW_CONTAINER_CHANGED = "preview-container-changed"

    let fieldLabel (label: string) = Bulma.label [ prop.text label ]

    let errorMessage (message: string) =
        Bulma.help [ color.isDanger; prop.text message ]

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

    [<ReactComponent>]
    let ActivePreviewContainerListener (setActiveId: Guid -> unit) =
        React.useWindowListener.on (
            PREVIEW_CONTAINER_CHANGED,
            fun ev ->
                let ev = ev :> CustomEvent<PreviewContainerEvent>

                printfn "Event: %A" ev

                match ev.detail with
                | Some detail -> setActiveId detail.ActiveGuid
                | None -> ()
        )

        Html.none

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
            Html.form
                [
                    prop.onSubmit (fun ev ->
                        ev.stopPropagation ()
                        ev.preventDefault ()

                        onSubmit |> Option.map dispatch |> Option.defaultWith ignore
                    )

                    prop.children
                        [
                            yield! fields

                            match state with
                            | Error error -> errorMessage error

                            | Success success ->
                                Bulma.field.div
                                    [
                                        Bulma.control.div
                                            [
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
                                Bulma.field.div
                                    [
                                        field.isGrouped
                                        field.isGroupedRight

                                        prop.children
                                            [
                                                Bulma.control.div
                                                    [
                                                        Bulma.button.button
                                                            [
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

        Html.div
            [
                prop.className "form-designer"
                prop.children
                    [
                        innerForm

                        Html.div [ prop.id "field-properties-portal" ]
                    ]
            ]

    let rec renderField
        (activeId: Guid)
        (dispatch: Dispatch<'Msg>)
        (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
        (field: FilledField<'Values, 'Attributes>)
        : ReactElement
        =

        let blur label =
            Option.map (fun onBlurEvent -> onBlurEvent label) fieldConfig.OnBlur

        let portalDest =
            let dest = document.getElementById ("field-properties-portal")

            if isNull dest then
                None
            else
                Some dest

        React.fragment
            [
                field.State.RenderPreview activeId blur dispatch fieldConfig field

                if field.State.Id = activeId && portalDest.IsSome then
                    ReactDOM.createPortal (
                        field.State.RenderPropertiesEditor blur dispatch fieldConfig field,
                        portalDest.Value
                    )
            ]

    [<ReactComponent>]
    let PreviewContainer
        (activeFieldId: Guid)
        (id: Guid)
        (updateFocus: bool -> unit)
        (content: ReactElement)
        =
        React.useWindowListener.on (
            PREVIEW_CONTAINER_CHANGED,
            fun ev ->
                let ev = ev :> CustomEvent<PreviewContainerEvent>

                printfn "Event: %A" ev

        // match ev.detail with
        // | Some detail ->
        //     printfn "PreviewContainer: %A" detail.ActiveGuid
        //     printfn "PreviewContainerMyId: %A" id
        //     if detail.ActiveGuid <> id then
        //         printfn "remove focus"
        //         updateFocus false
        // | None -> ()
        // updateFocus true
        )

        // printfn "PreviewContainer: %A" isActive

        Html.div
            [
                prop.className
                    [
                        "preview-container"
                        if activeFieldId = id then
                            "is-active"
                    ]

                prop.onClick (fun _ ->
                    // updateFocus true
                    // Notify all other preview containers to close
                    let detail =
                        jsOptions<CustomEventInit<_>> (fun o ->
                            o.detail <-
                                {
                                    ActiveGuid = id
                                }
                                |> Some
                        )

                    window.dispatchEvent (CustomEvent.Create(PREVIEW_CONTAINER_CHANGED, detail))
                    |> ignore
                )

                prop.children [ content ]
            ]
