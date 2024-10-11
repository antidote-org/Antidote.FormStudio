module Antidote.React.Components.CPTFinder

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
let CPTFinder (props: Field.ReactComponentField.ReactComponentFieldProps) =

    // Search term to call the API debounced request with
    let searchTerm, setSearchTerm = React.useState ""
    // Drug found via the API based on the input search term
    let foundCPTCodes, setFoundCPTCodes = React.useState<list<CPT>> []

    // Step 1.) medication selected from found drugs in order to search for strengths via the Medication SPLID
    let selectedCPTCodes, setSelectedCPTCodes =
        let decodedAllergies =
            match Thoth.Json.Decode.Auto.fromString<CPT list> (props.Value) with
            | Ok allergies -> allergies
            | Error e -> []

        React.useState<list<CPT>> decodedAllergies

    let isSearchResultVisible, setIsSearchResultVisible = React.useState false
    let isReadOnly, setIsReadOnly = React.useState props.Disabled

    let validateCPT (cptCode: CPT) =
        if System.String.IsNullOrWhiteSpace cptCode.HCPCS then
            Error $"CPT HCPCS cannot be empty for ID: {cptCode.Id}"
        else
            Ok()

    let validateAndAdd (cptCode: CPT) =
        match validateCPT cptCode with
        | Ok _ ->
            let newCPTCodes =
                selectedCPTCodes
                @ [
                    cptCode
                ]

            setSelectedCPTCodes newCPTCodes
        | Error _ -> ()

    let validateAndDone () =
        let allEntriesValid =
            selectedCPTCodes
            |> List.map validateCPT
            |> List.filter (
                function
                | Ok _ -> false
                | Error _ -> true
            )
            |> List.isEmpty

        if allEntriesValid then
            setIsReadOnly true
            props.OnChange(selectedCPTCodes |> Thoth.Json.Encode.Auto.toString)
        else
            ()

    // This is calling the one endpoint for everything
    useDebounce (
        searchTerm,
        500,
        fun searchTerm ->
            if searchTerm.Length > 2 then
                async {

                    let getCPTRequest
                        : Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry =
                        Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchTerm searchTerm
                        |> Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry.CPTCodesBySearchTerm

                    let! getCPTResults =
                        Antidote.Client.API.EndPoints.medicalRegistry.SearchMedicalRegistry
                            getCPTRequest

                    match getCPTResults with
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.CPTCodesBySearchTerm cptCodes ->
                        setFoundCPTCodes cptCodes
                        setIsSearchResultVisible true
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.NoCPTCodesFoundForTerm ->
                        setFoundCPTCodes []
                        setIsSearchResultVisible false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.InvalidRequest e ->
                        toast (Html.div "Invalid request for searching CPT codes") |> ignore
                        ()
                    | _ -> toast (Html.div "Incorrect medical registry endpoint called") |> ignore

                }
                |> Async.StartImmediate
            else
                ()
    )

    Html.div [
        prop.children [
            selectedCPTCodes
            |> List.map (fun cptCode ->
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
                                    (if System.String.IsNullOrWhiteSpace cptCode.HCPCS then
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
                                                selectedCPTCodes
                                                |> List.filter (fun x -> x <> cptCode)

                                            setSelectedCPTCodes newCPTCodes
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
                                    prop.text cptCode.HCPCS
                                ]
                                Html.span [
                                    prop.style [
                                        style.fontWeight 400
                                    ]
                                    prop.text "Description"
                                ]
                                Html.small (" - " + cptCode.Description)
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
                    Bulma.label "Search for a CPT code"
                    Bulma.control.div [
                        Bulma.control.hasIconsLeft
                        Bulma.control.hasIconsRight
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Start typing a CPT code"
                                prop.disabled props.Disabled
                                prop.value (searchTerm)
                                prop.onChange (fun term -> setSearchTerm term)
                            ]
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
                                        setFoundCPTCodes []
                                        setIsSearchResultVisible false
                                    )
                                    prop.children [
                                        Html.i [
                                            prop.className "fas fa-times"
                                        ]
                                    ]
                                ]
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
                                        // if foundCPTCodes.IsEmpty
                                        // then
                                        //     Html.div [
                                        //         prop.onClick ( fun _ -> () )
                                        //         prop.children [
                                        //             Html.strong "No CPT codes found for search term."
                                        //             Html.hr [ prop.className "navbar-divider" ]
                                        //         ]
                                        //     ]
                                        // else
                                        foundCPTCodes
                                        |> List.map (fun cptCode ->
                                            Html.div [
                                                prop.onClick (fun _ ->
                                                    setIsSearchResultVisible false
                                                    setSearchTerm ""
                                                    setFoundCPTCodes []
                                                    validateAndAdd cptCode |> ignore
                                                )
                                                prop.children [
                                                    Html.strong cptCode.HCPCS
                                                    Html.small (" | " + cptCode.Description)
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
