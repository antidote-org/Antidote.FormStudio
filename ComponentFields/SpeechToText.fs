module Antidote.React.Components.SpeechToText

open Feliz
open Fable.Core
open Fable.Core.JsInterop
open Feliz.Bulma

open Fable.Form.Antidote.Field.ReactComponentField
// open Antidote.AI.GPT.ChatGPTv4

let private classes : CssModules.ComponentFields.SpeechToText = import "default" "./SpeechToText.module.scss"

let clinicalCopilotSystemMessage =
    [
        "You are a highly skilled proof-reader."
        "You receive a message that is spoken and recognized using Speech to Text."
        "The text recognized can be chaotic, and your job is to make sense of it and correct it."
        "You always respond with the cleaned up version of the message."
        "All your responses are formatted as markdown."
        "Breakdown the conversation into bullet points, numerical points, or bolded titles when nessesary."
    ] |> String.concat ". \n"


// let initChatModel: InitChatModel =
//     {
//         Model = "gpt-4"
//         Temperature = 0.0
//         SystemMessage = clinicalCopilotSystemMessage
//     }
// let model = initChatModel |> createChatModel

type SpeechRecognition =
    abstract start: unit -> unit
    abstract stop: unit -> unit
    abstract continuous: bool
    abstract interimResults: bool
    abstract lang: string
    abstract onresult: (obj -> unit)
    abstract onend: (obj -> unit)
    abstract onstart: (obj -> unit)
    abstract onerror: (obj -> unit)

[<Emit("new webkitSpeechRecognition()")>]
let webkitSpeechRecognition() : SpeechRecognition = jsNative

[<Emit("new SpeechRecognition()")>]
let speechRecognition() : SpeechRecognition = jsNative

type RecognitionSupport =
    | Webkit
    | Standard
    | Unsupported

let getSpeechRecognition () =
    let recognitionSupport =
        if Browser.Dom.window?webkitSpeechRecognition then
            Webkit
        elif Browser.Dom.window?SpeechRecognition then
            Standard
        else
            Unsupported

    let speechRecognition: SpeechRecognition option =
        match recognitionSupport with
        | Webkit ->
            Some (webkitSpeechRecognition())

        | Standard ->
            Some (speechRecognition())
        | Unsupported ->
            None

    speechRecognition


type SpeechToTextProps = {|
    SpeechRecognized: string -> unit
    Send: unit -> unit
    IsBusy: bool
|}

[<ReactComponent>]
let SpeechToText(props: ReactComponentFieldProps ) =
    let (isListening, setIsListening) = React.useState false
    let recognitionOpt, setRecognitionOpt = React.useState (getSpeechRecognition())

    // let cleanedupVersion, setCleandupVersion = React.useState ""

    let (isLoading, setIsLoading) = React.useState false

    let originalRecognition, setOriginalRecognition = React.useState ""

    let currentTextBoxValue, setCurrentTextBoxValue = React.useState props.Value

    // let cleanupTranscript () =
    //     let userMessage: ChatMessage =  { role = ChatMessageRole.User; content = currentTextBoxValue }
    //     async {
    //         setIsLoading true
    //         let! response = postRequest (model |> addMessage userMessage )
    //         match response with
    //         | Error e ->
    //             debuglog "AI Transcript Failed"
    //             setIsLoading false
    //         | Ok r ->
    //             let transcriptByAI = r.choices |> Array.map (fun c -> c.message.content) |> String.concat ", "
    //             debuglog "AI Transcript Response: {transcriptByAI}"
    //             // setCleandupVersion transcriptByAI
    //             setCurrentTextBoxValue transcriptByAI
    //             // props.OnChange (r.choices |> Array.map (fun c -> c.message.content) |> String.concat ", ")
    //             setIsLoading false
    //             setCurrentTextBoxValue transcriptByAI
    //             props.OnChange transcriptByAI
    //     } |> Async.StartImmediate

    let undoTranscript () =
        setCurrentTextBoxValue originalRecognition
        props.OnChange originalRecognition

    let recognitionEffect  =
        (fun () ->
            recognitionOpt?continuous <- true
            recognitionOpt?interimResults <- true
            recognitionOpt?lang <- "en-US"
            recognitionOpt?onresult <- (fun event ->
                // props.OnChange("ONRESULT")
                let currentTranscript =
                    event?results
                    |> Array.ofSeq
                    |> Array.map (fun result ->
                        // if result?isFinal then
                        //     debuglog ("Speech Recognition Final Result:", result)
                        // else
                        //     debuglog ("Speech Recognition Result:", result)
                        (unbox<string array>result).[0]
                    )
                    |> Array.map (fun result -> result?transcript)
                    |> String.concat ""
                printfn $"recognitionEffect: currentTranscript: {currentTranscript}"
                setCurrentTextBoxValue currentTranscript
                setOriginalRecognition currentTranscript
            )

            recognitionOpt?speechend <- (fun _ ->
                setIsListening false
            )

            recognitionOpt?onend <- (fun _ ->
                setIsListening false
            )

            recognitionOpt?onstart <- (fun _ ->
                setIsListening true
            )

            recognitionOpt?onerror <- (fun event -> log event?error )
        )

    React.useEffect (
        fun _ ->
            props.OnChange currentTextBoxValue
        ,[|box currentTextBoxValue|]
    )

    React.useEffectOnce(
        match recognitionOpt with
        | Some recognition -> recognitionEffect
        | None -> (fun _ ->
            printfn "Speech recognition is not supported on this browser"
            ()
        )
    )

    let toggleListening recognition =
        if isListening then
            printfn "Stopping recognition"
            recognition?stop()
        else
            printfn "Starting recognition"
            recognition?start()

    Bulma.control.div [
        control.hasIconsRight
        prop.style [ style.position.relative ]
        prop.children [
            if props.Disabled
            then
                Bulma.content [
                    Feliz.Markdown.Markdown.markdown props.Value
                ]
            else
                Bulma.textarea [
                    prop.placeholder "Write or click the microphone to transcribe"
                    prop.value (currentTextBoxValue
                        // if props.Disabled
                        // then currentTextBoxValue
                        // else props.Value
                    )
                    prop.onChange (fun str ->
                        setCurrentTextBoxValue str
                        props.OnChange str
                    )
                ]
                // Html.i [
                //                         prop.style [style.marginRight 8]
                //                         prop.onClick(
                //                             if props.Disabled
                //                             then (fun _ -> ())
                //                             else (fun _ -> cleanupTranscript())
                //                         )
                //                         prop.children [
                //                             Html.i [ prop.className "fas fa-signature" ]
                //                             // Icon [
                //                             //     icon.icon mdi.fileSign
                //                             //     icon.color "#26619c"
                //                             //     icon.width 20
                //                             // ]
                //                         ]
                //                     ]
                // Bulma.icon [
                //     icon.isRight
                //     prop.onClick (fun _ -> cleanupTranscript() )
                //     prop.style [
                //         style.position.absolute
                //         style.top (length.px -30) // Set top to 0 to align with the top edge
                //         style.right (length.px 60) // Set right to 0 to align with the right edge
                //         // Removed translateY to prevent vertical shifting
                //         style.pointerEvents.unset
                //         style.cursor.pointer
                //     ]

                //     prop.children [
                //         Html.i [
                //             prop.classes [
                //                 "fas"
                //                 "fa-signature"
                //                 if isListening then "is-hidden"
                //             ]
                //         ]
                //     ]
                // ]
                Bulma.icon [
                    icon.isRight
                    prop.onClick (fun _ -> undoTranscript ())
                    prop.style [
                        style.position.absolute
                        style.top (length.px -30) // Set top to 0 to align with the top edge
                        style.right (length.px 30) // Set right to 0 to align with the right edge
                        // Removed translateY to prevent vertical shifting
                        style.pointerEvents.unset
                        style.cursor.pointer
                    ]

                    prop.children [
                        Html.i [
                            prop.classes [
                                "fas"
                                "fa-undo"
                                if isListening then "is-hidden"
                            ]
                        ]
                    ]
                ]
                Bulma.icon [
                    icon.isRight
                    prop.onClick (fun _ ->
                        match recognitionOpt with
                        | Some recognition ->
                            toggleListening (recognition)
                        | None -> printfn "Why you here?"
                    )
                    prop.style [
                        style.position.absolute
                        style.top (length.px -30) // Set top to 0 to align with the top edge
                        style.right (length.px 0) // Set right to 0 to align with the right edge
                        // Removed translateY to prevent vertical shifting
                        style.pointerEvents.unset
                        style.cursor.pointer
                    ]

                    prop.children [
                        Html.i [
                            prop.classes [
                                if isListening then "fas fa-stop" else "fas fa-microphone"
                            ]
                        ]
                    ]
                ]

        ]
    ]
