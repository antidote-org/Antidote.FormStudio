module Antidote.React.Components.FormWizard.FormActions

open Feliz
open Feliz.Bulma
// open Fable.Form.Antidote
open Antidote.FormStudio.Compose.Types
open Antidote.FormStudio.i18n.Util

let previousAction
    (previousLabel: string)
    (formState: Fable.Form.Simple.Form.View.State)
    (onPrevious: unit -> unit)
    =
    Bulma.field.div [
        field.isGrouped
        field.isGroupedRight
        prop.classes [
            "form-actions"
        ]

        prop.children [
            Bulma.control.div [
                prop.style [
                    // style.position.sticky
                    style.custom ("minWidth", "100%")
                // style.custom ("boxShadow", "0px 0px 15px 15px #0000001f")
                // style.custom ("backgroundColor", "#0000001f")
                ]
                prop.children [
                    Bulma.button.a [
                        button.isOutlined
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]
                        prop.onClick (fun _ -> onPrevious ())
                        prop.text previousLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            prop.disabled true
                    ]

                ]
            ]
        ]
    ]

let previousAndNextAction
    (previousLabel: string)
    (nextLabel: string)
    (formState: Fable.Form.Simple.Form.View.State)
    (onPrevious: unit -> unit)
    (onNext: unit -> unit)
    =

    Bulma.field.div [
        field.isGrouped
        //THIS INLINE STYLES MUST MOVE TO A SCSS MODULE
        prop.style [
            // style.position.sticky
            style.bottom 15
            style.height 40
            style.custom ("minWidth", "100%")
            style.display.flex
            style.justifyContent.center
        // style.custom ("boxShadow", "0px 0px 15px 15px #0000001f")
        // style.custom ("backgroundColor", "#0000001f")

        ]
        prop.children [
            Bulma.control.div [
                prop.style [
                    style.custom ("minWidth", "50%")
                ]
                prop.children [
                    Bulma.button.a [
                        button.isOutlined
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]

                        prop.onClick (fun _ -> onPrevious ())
                        prop.text previousLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            prop.disabled true
                    ]
                ]
            ]

            // Default submit button
            Bulma.control.div [
                prop.style [
                    style.custom ("minWidth", "50%")
                ]
                prop.children [
                    Bulma.button.button [
                        color.isPrimary
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]
                        prop.onClick (fun _ -> onNext ())
                        prop.text nextLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            button.isLoading
                    ]
                ]
            ]
        ]
    ]

let previousAndSubmitAction
    (previousLabel: string)
    (submitLabel: string)
    (formState: Fable.Form.Simple.Form.View.State)
    (onPrevious: unit -> unit)
    =
    Bulma.field.div [
        field.isGrouped
        //THIS INLINE STYLES MUST MOVE TO A SCSS MODULE
        prop.style [
            // style.position.sticky
            style.bottom 15
            style.height 60
            style.custom ("minWidth", "100%")
            style.display.flex
            style.justifyContent.center
        // style.custom ("boxShadow", "0px 0px 15px 15px #0000001f")
        // style.custom ("backgroundColor", "#0000001f")
        ]
        prop.children [
            Bulma.control.div [
                prop.style [
                    style.custom ("minWidth", "50%")
                ]
                prop.children [
                    Bulma.button.a [
                        button.isOutlined
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]

                        prop.onClick (fun _ -> onPrevious ())
                        prop.text previousLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            prop.disabled true
                    ]
                ]
            ]

            // Default submit button
            Bulma.control.div [
                prop.style [
                    style.custom ("minWidth", "50%")
                ]
                prop.children [
                    Bulma.button.button [
                        color.isSuccess
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]
                        // prop.onClick (fun _ ->
                        //     dispatch Submit
                        // )
                        prop.text submitLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            button.isLoading
                    ]
                ]
            ]
        ]
    ]

let simplyNextAction (nextLabel: string) (formState: Fable.Form.Simple.Form.View.State) =

    Bulma.field.div [
        field.isGrouped
        field.isGroupedRight
        // prop.classes ["form-actions"]
        //THIS INLINE STYLES MUST MOVE TO A SCSS MODULE
        prop.style [
            // style.position.sticky
            style.bottom 15
            style.height 40
            style.custom ("minWidth", "100%")
            style.display.flex
            style.justifyContent.center
        // style.custom ("boxShadow", "0px 0px 15px 15px #0000001f")
        // style.custom ("backgroundColor", "#0000001f")

        ]
        prop.children [
            // Default submit button
            Bulma.control.div [
                prop.style [
                    style.custom ("minWidth", "100%")
                ]
                prop.children [
                    Bulma.button.button [
                        // prop.type'.submit
                        color.isPrimary
                        prop.style [
                            style.custom ("minWidth", "100%")
                        ]
                        prop.text nextLabel
                        // If the form is loading animate the button with the loading animation
                        if formState = Fable.Form.Simple.Form.View.Loading then
                            button.isLoading
                    ]
                ]
            ]
        ]
    ]

// let simplySubmit
//     (submit:string)
//     (formState : Form.View.State)
//     (dispatch : Dispatch<Msg>)
//      =

//         Bulma.field.div [
//             field.isGrouped
//             field.isGroupedRight
//             // prop.classes ["form-actions"]
//             //THIS INLINE STYLES MUST MOVE TO A SCSS MODULE
//             prop.style [
//                 style.position.sticky
//                 style.bottom 15
//                 style.height 60
//                 style.custom ("minWidth","100%")
//             ]
//             prop.children [
//                 // Default submit button
//                 Bulma.control.div [
//                     prop.style [ style.custom ("minWidth","100%")]
//                     prop.children[
//                         Bulma.button.button [
//                             // prop.type'.submit
//                             color.isPrimary
//                             prop.style [ style.custom ("minWidth","100%")]
//                             prop.onClick (fun _ ->
//                                 dispatch Subject
//                             )
//                             prop.text submit
//                             // If the form is loading animate the button with the loading animation
//                             if formState = Form.View.Loading then
//                                 button.isLoading
//                         ]
//                     ]
//                 ]
//             ]
//         ]

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type Action =
    | SubmitOnly of string
    | Custom of (Fable.Form.Simple.Form.View.State -> ReactElement)

let formAction stepProgress isSubmitted onPrevious onNext =

    match stepProgress with
    | ReadOnly -> Action.Custom(fun _ -> Html.div [])
    | First -> Action.Custom(fun formState -> simplyNextAction (t Intl.Next.Key) formState)
    | Middle ->
        Action.Custom(fun formState ->
            previousAndNextAction
                (t Intl.Previous.Key)
                (t Intl.Next.Key)
                formState
                onPrevious
                onNext
        )
    | Last ->
        if isSubmitted then
            Action.Custom(fun formState ->
                previousAction (t Intl.Previous.Key) formState onPrevious
            )
        else
            Action.Custom(fun formState ->
                previousAndSubmitAction
                    (t Intl.Previous.Key)
                    (t Intl.Submit.Key)
                    formState
                    onPrevious
            )
