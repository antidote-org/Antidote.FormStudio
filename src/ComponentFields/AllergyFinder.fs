module Antidote.React.Components.AllergyFinder

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
let AllergyFinder (props: Field.ReactComponentField.ReactComponentFieldProps) =

    // Search term to call the API debounced request with
    let searchTerm, setSearchTerm = React.useState ""
    // Drug found via the API based on the input search term
    let foundAllergies, setFoundAllergies = React.useState<list<Allergy>> []

    // Step 1.) medication selected from found drugs in order to search for strengths via the Medication SPLID
    let selectedAllergies, setSelectedAllergies =
        let decodedAllergies =
            match Thoth.Json.Decode.Auto.fromString<Allergy list> (props.Value) with
            | Ok allergies -> allergies
            | Error e -> []

        React.useState<list<Allergy>> decodedAllergies

    let isSearchResultVisible, setIsSearchResultVisible = React.useState false
    let isReadOnly, setIsReadOnly = React.useState props.Disabled

    let validateAllergy (allergy: Allergy) =
        if System.String.IsNullOrWhiteSpace allergy.Name then
            Error $"Allergy Name cannot be empty for ID: {allergy.Id}"
        else
            Ok()

    let validateAndAdd (allergy: Allergy) =
        match validateAllergy allergy with
        | Ok _ ->
            let newAllergies =
                selectedAllergies
                @ [
                    allergy
                ]

            setSelectedAllergies newAllergies
        | Error _ -> ()

    let validateAndDone () =
        let allEntriesValid =
            selectedAllergies
            |> List.map validateAllergy
            |> List.filter (
                function
                | Ok _ -> false
                | Error _ -> true
            )
            |> List.isEmpty

        if allEntriesValid then
            setIsReadOnly true
            props.OnChange(selectedAllergies |> Thoth.Json.Encode.Auto.toString)
        else
            ()

    // This is calling the one endpoint for everything
    useDebounce (
        searchTerm,
        500,
        fun searchTerm ->
            if searchTerm.Length > 2 then
                async {

                    let getAllergiesRequest
                        : Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry =
                        Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchTerm searchTerm
                        |> Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry.AllergiesBySearchTerm

                    let! getAllergiesResult =
                        Antidote.Client.API.EndPoints.medicalRegistry.SearchMedicalRegistry
                            getAllergiesRequest

                    match getAllergiesResult with
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.AllergiesBySearchTerm allergies ->
                        setFoundAllergies allergies
                        setIsSearchResultVisible true
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.NoAllergiesFoundForTerm ->
                        setFoundAllergies []
                        setIsSearchResultVisible false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.InvalidRequest e ->
                        toast (Html.div "Invalid request for searching allergies registry")
                        |> ignore
                    | _ -> toast (Html.div "Incorrect medical registry endpoint called") |> ignore

                }
                |> Async.StartImmediate
            else
                ()
    )

    Html.div [
        //this may break
        // prop.key (
        //     props.Value
        //     |> Thoth.Json.Decode.Auto.fromString<Allergy list>
        //     |> Result.map (fun allergy -> allergy.Name)
        //     |> Map.tryHead
        //     |> function
        //         | Some allergy -> allergy
        //         | Error _ -> "none"
        // )
        prop.children [
            selectedAllergies
            |> List.map (fun selectedAllergy ->
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
                                    (if System.String.IsNullOrWhiteSpace selectedAllergy.Name then
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
                                            let newAllergies =
                                                selectedAllergies
                                                |> List.filter (fun x -> x <> selectedAllergy)

                                            setSelectedAllergies newAllergies
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
                                    prop.text selectedAllergy.Name
                                ]
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
                    Bulma.label "Search for an allergy"
                    Bulma.control.div [
                        Bulma.control.hasIconsLeft
                        Bulma.control.hasIconsRight
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "Start typing an allergy"
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
                                        setFoundAllergies []
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

                          prop.children[if foundAllergies.IsEmpty then
                                            Html.div [
                                                prop.onClick (fun _ -> ())
                                                prop.children [
                                                    Html.strong
                                                        "No allergies found for search term."
                                                    Html.hr [
                                                        prop.className "navbar-divider"
                                                    ]
                                                ]
                                            ]
                                        else
                                            foundAllergies
                                            |> List.map (fun allergy ->
                                                Html.div [
                                                    prop.onClick (fun _ ->
                                                        setIsSearchResultVisible false
                                                        setSearchTerm ""
                                                        setFoundAllergies []
                                                        validateAndAdd allergy |> ignore
                                                    )
                                                    prop.children [
                                                        Html.strong (allergy.Name)
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
