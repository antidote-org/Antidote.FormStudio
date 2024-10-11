module Antidote.React.Components.ICD10Finder

open Feliz
open Feliz.Bulma

open Antidote.MedicalRegistry.Drugs.Types
open Fable.Core.JS
open Antidote.Core.V2.Types
open FSharp.Collections
open Fable.Form.Antidote
open type Toastify.Exports

[<Hook>]
let useDebounce (value: 'T, timeout: int, callback: 'T -> unit) =
    let initialCall, setInitialCall = React.useState (true)

    React.useEffect (
        fun _ ->
            if initialCall then
                { new System.IDisposable with
                    member __.Dispose() = setInitialCall false
                }
            else
                let handler = setTimeout (fun () -> callback value) timeout

                { new System.IDisposable with
                    member __.Dispose() = clearTimeout (handler)
                }
        , [|
            box value
            box timeout
        |]
    )

[<ReactComponent>]
let ICD10Finder (props: Field.ReactComponentField.ReactComponentFieldProps) =
    let (isLoading, setIsLoading) = React.useState false
    // Search term to call the API debounced request with
    let searchTerm, setSearchTerm = React.useState ""
    // Drug found via the API based on the input search term
    let foundICD10Codes, setFoundICD10Codes = React.useState<list<ICD10>> []

    // Step 1.) medication selected from found drugs in order to search for strengths via the Medication SPLID
    let selectedICD10Codes, setSelectedICD10Codes =
        let decodedICD10Codes =
            match Thoth.Json.Decode.Auto.fromString<list<ICD10>> (props.Value) with
            | Ok icd10Codes -> icd10Codes
            | Error e -> []

        React.useState<list<ICD10>> decodedICD10Codes

    let isSearchResultVisible, setIsSearchResultVisible = React.useState false
    let isReadOnly, setIsReadOnly = React.useState props.Disabled

    let validateICD10 (icd10: ICD10) =
        if System.String.IsNullOrWhiteSpace icd10.Code then
            Error $"ICD10 Code cannot be empty for ID: {icd10.Id}"
        else
            Ok()

    let validateAndAdd (icd10: ICD10) =
        match validateICD10 icd10 with
        | Ok _ ->
            let newICD10Codes =
                selectedICD10Codes
                @ [
                    icd10
                ]

            setSelectedICD10Codes newICD10Codes
        | Error _ -> ()

    let validateAndDone () =
        let allEntriesValid =
            selectedICD10Codes
            |> List.map validateICD10
            |> List.filter (
                function
                | Ok _ -> false
                | Error _ -> true
            )
            |> List.isEmpty

        if allEntriesValid then
            setIsReadOnly true
            props.OnChange(selectedICD10Codes |> Thoth.Json.Encode.Auto.toString)
        else
            ()

    // This is calling the one endpoint for everything
    useDebounce (
        searchTerm,
        500,
        fun searchTerm ->
            if searchTerm.Length > 2 then
                async {
                    setIsLoading true

                    let getICD10Request
                        : Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry =
                        Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchTerm searchTerm
                        |> Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry.ICD10CodesBySearchTerm

                    let! getICD10Result =
                        Antidote.Client.API.EndPoints.medicalRegistry.SearchMedicalRegistry
                            getICD10Request

                    match getICD10Result with
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.ICD10CodesBySearchTerm icd10Codes ->
                        setFoundICD10Codes icd10Codes
                        setIsSearchResultVisible true
                        setIsLoading false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.NoICD10CodesFoundForTerm ->
                        setFoundICD10Codes []
                        setIsSearchResultVisible false
                        setIsLoading false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.InvalidRequest e ->
                        setIsLoading false
                        toast (Html.div "Invalid request for searching ICD10 codes") |> ignore
                        ()
                    | _ ->
                        setIsLoading false
                        toast (Html.div "Incorrect medical registry endpoint called") |> ignore

                }
                |> Async.StartImmediate
            else
                ()
    )

    Html.div [
        prop.children [
            selectedICD10Codes
            |> List.map (fun icd10Code ->
                Html.div [
                    prop.style [
                        style.backgroundColor "white"
                    ]
                    prop.children [
                        Html.div [
                            prop.style [
                                style.border (
                                    1,
                                    borderStyle.solid,
                                    (if System.String.IsNullOrWhiteSpace icd10Code.Code then
                                         "red"
                                     else
                                         "blue")
                                )
                                style.padding 10
                                style.marginTop 10
                                style.borderRadius 5
                            ]
                            prop.children [
                                if not isReadOnly then
                                    Bulma.icon [
                                        prop.style [
                                            style.position.absolute
                                            style.right 25
                                        ]
                                        prop.onClick (fun _ ->
                                            let newCPTCodes =
                                                selectedICD10Codes
                                                |> List.filter (fun x -> x <> icd10Code)

                                            setSelectedICD10Codes newCPTCodes
                                        )
                                        prop.children [
                                            Html.i [
                                                prop.className "fas fa-times"
                                            ]
                                        ]
                                    ]
                                Bulma.title [
                                    title.is6
                                    color.hasTextBlack
                                    prop.text icd10Code.Code
                                ]
                                Html.span [
                                    prop.style [
                                        style.fontWeight 400
                                    ]
                                    prop.text "Description"
                                ]
                                Html.small (" - " + icd10Code.ShortDescription)
                            ]
                        ]
                    ]
                ]
            )
            |> React.fragment

            if isReadOnly then
                Html.none
            else
                Bulma.field.div [
                    Bulma.label "Search for an ICD 10 code"
                    Bulma.control.div [
                        Bulma.control.hasIconsLeft
                        Bulma.control.hasIconsRight
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Start typing an ICD 10 code"
                                prop.disabled props.Disabled
                                prop.value (searchTerm)
                                prop.onChange (fun term -> setSearchTerm term)
                            ]
                            if isLoading then
                                Bulma.icon [
                                    Bulma.icon.isLeft
                                    prop.children[Bulma.button.button [
                                                      Bulma.button.isLoading
                                                      prop.style [
                                                          style.borderColor.transparent
                                                          style.backgroundColor.transparent
                                                      ]
                                                  ]]
                                ]
                            else
                                Bulma.icon [
                                    Bulma.icon.isLeft
                                    prop.children [
                                        Html.i [
                                            prop.className "fas fa-search"
                                        ]
                                    ]
                                ]
                            match System.String.IsNullOrWhiteSpace searchTerm, props.Disabled with
                            | false, false ->
                                Bulma.icon [
                                    Bulma.icon.isRight
                                    prop.style [
                                        style.pointerEvents.unset
                                        style.cursor.pointer
                                        style.color.red
                                    ]
                                    prop.onClick (fun _ ->
                                        setSearchTerm ""
                                        setFoundICD10Codes []
                                        setIsSearchResultVisible false
                                    )
                                    prop.children [
                                        Html.i [
                                            prop.className "fas fa-times"
                                        ]
                                    ]
                                ]
                            | true, false
                            | _ -> Html.none
                        ]
                    ]
                ]

            if isSearchResultVisible then
                Bulma.box[prop.style [
                              style.maxHeight 200
                              style.overflow.auto
                          ]

                          prop.children[
                                        // if foundICD10Codes.IsEmpty
                                        // then
                                        //     Html.div [
                                        //         prop.onClick ( fun _ -> () )
                                        //         prop.children [
                                        //             Html.strong "No ICD 10 codes found for search term."
                                        //             Html.hr [ prop.className "navbar-divider" ]
                                        //         ]
                                        //     ]
                                        // else
                                        foundICD10Codes
                                        |> List.map (fun icd10 ->
                                            Html.div [
                                                prop.onClick (fun _ ->
                                                    setIsSearchResultVisible false
                                                    setSearchTerm ""
                                                    setFoundICD10Codes []
                                                    validateAndAdd icd10 |> ignore
                                                )
                                                prop.children [
                                                    Html.strong (icd10.Code)
                                                    Html.small (" | " + icd10.ShortDescription)
                                                    Html.hr [
                                                        prop.className "navbar-divider"
                                                    ]
                                                ]
                                            ]
                                        )
                                        |> React.fragment]]

            if not isReadOnly then
                Bulma.iconText [
                    prop.style [
                        style.cursor.pointer
                    ]
                    prop.children [
                        Bulma.icon [
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-flag-checkered"
                                ]
                            ]
                            prop.onClick (fun _ -> validateAndDone ())
                        ]
                        Html.span [
                            prop.text "Done"
                            prop.onClick (fun _ -> validateAndDone ())
                        ]
                    ]
                ]
            else if props.Disabled then
                Html.none
            else
                Bulma.iconText [
                    prop.style [
                        style.cursor.pointer
                    ]
                    prop.children [
                        Bulma.icon [
                            prop.children [
                                Html.i [
                                    prop.className "fas fa-edit"
                                ]
                            ]
                        ]
                        Html.span [
                            prop.text "Edit"
                            prop.onClick (fun _ -> setIsReadOnly false)
                        ]
                    ]
                ]

        ]
    ]
