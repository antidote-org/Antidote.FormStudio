module Antidote.React.Components.DrugFinder

open Feliz
open Feliz.Bulma

open Antidote.MedicalRegistry.Drugs.Types
open Fable.Core.JS
open Antidote.Core.V2.Types
open FSharp.Collections
open Fable.Form.Antidote
open type Toastify.Exports

[<Hook>]
let useDebounce (value: 'T, timeout:int, callback : 'T -> unit) =
    let initialCall, setInitialCall = React.useState(true)
    React.useEffect (
        fun _ ->
            if initialCall then
                { new System.IDisposable with
                    member __.Dispose() =
                        setInitialCall false
                }
            else
                let handler =
                    setTimeout (fun () ->
                        callback value
                    ) timeout

                { new System.IDisposable with
                    member __.Dispose() =
                        clearTimeout(handler)
                }
        , [| box value ; box timeout |]
    )

[<ReactComponent>]
let DrugFinder (props : Field.ReactComponentField.ReactComponentFieldProps) =
    let requiresMedicationCompoundStrength = true

    // Search term to call the API debounced request with
    let searchTerm, setSearchTerm = React.useState ""
    let (isLoading, setIsLoading) = React.useState false
    // Drug found via the API based on the input search term
    let foundDrugs, setFoundDrugs = React.useState<list<MedicationsBySearchTerm>> []
    let filteredDrugs, setFilteredDrugs = React.useState<list<MedicationsBySearchTerm>> []
    // Drug strength for selected medication if requires medication compound is true
    let drugStrengths, setDrugStrengths = React.useState<list<MedicationStrength>> []

    // Step 1.) medication selected from found drugs in order to search for strengths via the Medication SPLID
    let selectedMedication, setSelectedMedication =
        let decodedMedication =
            match Thoth.Json.Decode.Auto.fromString<MedicationsBySearchTerm>( props.Value ) with
            | Ok medication -> Some medication
            | Error e -> None
        React.useState<MedicationsBySearchTerm option> decodedMedication

    let setFoundDrugsWrapper (foundDrugs:list<MedicationsBySearchTerm>) =
        setFoundDrugs foundDrugs
        setFilteredDrugs foundDrugs

    let isSearchResultVisible, setIsSearchResultVisible = React.useState false

    // This is calling the one endpoint for everything
    useDebounce(
        searchTerm,
        500,
        fun searchTerm ->
            if searchTerm.Length > 2
            then
                async {
                    setIsLoading true
                    let getMedicationRequest : Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry =
                        Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchTerm searchTerm
                        |> Antidote.Core.V2.Domain.MedicalRegistry.Request.SearchMedicalRegistry.MedicationsBySearchTerm

                    let! getMedicationResult = Antidote.Client.API.EndPoints.medicalRegistry.SearchMedicalRegistry getMedicationRequest

                    match getMedicationResult with
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.MedicationsBySearchTerm medications ->
                        setFoundDrugsWrapper medications
                        setIsSearchResultVisible true
                        setIsLoading false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.NoMedicationsFoundForTerm ->
                        setFoundDrugs []
                        setIsLoading false
                    | Antidote.Core.V2.Domain.MedicalRegistry.Response.SearchMedicalRegistry.InvalidRequest e ->
                        setIsLoading false
                        toast ( Html.div "Invalid request for searching medication registry." ) |> ignore
                        ()
                    | _ ->
                        setIsLoading false
                        toast ( Html.div "Incorrect medical registry endpoint called." ) |> ignore

                } |> Async.StartImmediate
            else ()
    )

    Html.div [
        prop.className "field-test-drug"
        prop.key (
            props.Value
            |> Thoth.Json.Decode.Auto.fromString<MedicationSelection>
            |> Result.map (fun medication -> medication.SPLID.ToString())
            |> function
                | Ok splid -> splid
                | Error _ -> "0"
        )
        prop.children [

        Bulma.field.div [
            Bulma.label "Search for a medication"
            Bulma.control.div [
                Bulma.control.hasIconsLeft
                Bulma.control.hasIconsRight
                prop.children [
                    Bulma.input.text [
                        prop.placeholder "Start typing a medication"
                        prop.disabled props.Disabled
                        prop.value (
                            match selectedMedication with
                            | Some medication -> medication.ActiveIngredientName
                            | None -> searchTerm
                        )
                        // prop.value (
                        //     match selectedMedication with
                        //     | None -> searchTerm
                        //     | Some drug -> drug.ProductName
                        // )
                        prop.onChange (fun term ->
                            setSearchTerm term
                        )
                    ]

                    if isLoading then
                        Bulma.icon [
                            Bulma.icon.isLeft
                            prop.children[
                                Bulma.button.button [
                                    Bulma.button.isLoading
                                    prop.style [ style.borderColor.transparent; style.backgroundColor.transparent ]
                                ]
                            ]
                        ]
                    else
                        Bulma.icon [
                            Bulma.icon.isLeft
                            prop.children [ Html.i [ prop.className "fas fa-search" ] ]
                        ]
                    match selectedMedication, props.Disabled with
                    | Some _, false ->
                        Bulma.icon [
                            Bulma.icon.isRight
                            prop.style [
                                style.pointerEvents.unset
                                style.cursor.pointer
                                style.color.red
                            ]
                            prop.onClick (fun _ ->
                                setSearchTerm ""
                                setFoundDrugs []
                                setSelectedMedication None
                                setDrugStrengths []
                            )
                            prop.children [ Html.i [ prop.className "fas fa-times" ] ]
                        ]
                    | _ -> Html.none
                ]
            ]
        ]

        if isSearchResultVisible
        then
            Bulma.select [
                prop.defaultValue "-"
                prop.onChange (fun activeIngredientName ->
                    let filteredDrugs =
                        foundDrugs
                        |> List.filter (fun drug -> drug.ActiveIngredientName = activeIngredientName)
                    setFilteredDrugs filteredDrugs
                )
                prop.children [
                    Html.option [
                        prop.disabled true
                        prop.text "Narrow by Active Ingredient"
                        prop.value "-"
                    ]
                    foundDrugs
                    |> List.map (fun drug -> drug.ActiveIngredientName)
                    |> List.distinct
                    |> List.map (fun activeIngredientName ->
                        Html.option [
                            prop.text (activeIngredientName)
                            prop.value (activeIngredientName)
                        ]
                    ) |> React.fragment
                ]
            ]
            Bulma.select [
                prop.defaultValue "-"
                prop.onChange (fun strength ->
                    let filteredDrugs =
                        filteredDrugs
                        |> List.filter (fun drug -> drug.Strength = strength)
                    setFilteredDrugs filteredDrugs
                )
                prop.children [
                    Html.option [
                        prop.disabled true
                        prop.text "Narrow by Strength"
                        prop.value "-"
                    ]
                    filteredDrugs
                    |> List.map (fun drug -> drug.Strength)
                    |> List.distinct
                    |> List.map (fun strength ->
                        Html.option [
                            prop.text (strength)
                            prop.value (strength)
                        ]
                    ) |> React.fragment
                ]
            ]
            Bulma.box[
                prop.style [
                    style.maxHeight 400
                    style.overflow.auto
                    // style.position.absolute
                    // style.zIndex 100
                ]
                prop.children[
                    filteredDrugs
                    |> List.distinctBy (fun drug -> drug.ActiveIngredientName, drug.BrandName, drug.GenericName, drug.Strength)
                    |> List.map (fun drug ->
                        Html.div [
                            prop.onClick (
                                fun _ ->
                                    setIsSearchResultVisible false
                                    setSearchTerm ""
                                    setFoundDrugs []
                                    // setSelectedMedication (Some drug)
                                    props.OnChange (drug |> Thoth.Json.Encode.Auto.toString )

                                    // if requiresMedicationCompoundStrength
                                    // then
                                    //     setSelectedMedication (Some drug)
                                    // else  props.OnChange (drug |> Thoth.Json.Encode.Auto.toString )
                            )
                            prop.children [
                                Html.strong (drug.ActiveIngredientName + " | " + drug.Strength + " | " + drug.BrandName + " | " + drug.GenericName)
                                Html.hr [ prop.className "navbar-divider" ]
                            ]
                        ]
                    ) |> React.fragment
                ]
            ]
        ]
    ]

type PatientMedication = {|
    MedicationSelection: MedicationsBySearchTerm
    Frequency: string
    FillSupply: int
    PrescribedDate: System.DateTime
|}

module SearchTermMedication =
    open FsToolkit.ErrorHandling
    open System

    type InvalidSearchTermMedication =
        | InvalidActiveIngredientName of string
        | InvalidBrandName of string
        | InvalidGenericName of string
        | InvalidMedicationId of string
        | InvalidStrength of string
        | InvalidStrengthNumber of string
        | InvalidStrengthUnit of string

    // helper for client building the medication through form as component (HAS OPTIONS FOR ALL)
    let validateSearchTermMedication (searchTermMedication : Antidote.Core.V2.Types.MedicationsBySearchTerm) : Result<MedicationsBySearchTerm, InvalidSearchTermMedication list> =

        let activeIngredientName = searchTermMedication.ActiveIngredientName
        let brandName = searchTermMedication.BrandName
        let genericName = searchTermMedication.GenericName
        let medicationId = searchTermMedication.MedicationId
        let medicationStrength = searchTermMedication.Strength
        let medicationStrengthNumber = searchTermMedication.StrengthNumber
        let medicationStrengthUnit = searchTermMedication.StrengthUnit

        validation {

            let! _ =
                if String.IsNullOrWhiteSpace activeIngredientName
                then Error "Active Ingredient cannot be empty."
                else Ok activeIngredientName
                |> Result.mapError InvalidActiveIngredientName

            and! _ =
                if String.IsNullOrWhiteSpace medicationId
                then Error "Medication ID cannot be empty."
                else Ok medicationId
                |> Result.mapError InvalidMedicationId

            and! _ =
                if float medicationStrengthNumber <= 0.0
                then
                    Error "Medication strength must be greater than 0"
                    |> Result.mapError InvalidStrengthNumber
                else Ok medicationStrengthNumber

            and! _ =
                if System.String.IsNullOrWhiteSpace medicationStrengthUnit
                then
                    Error "Empty medication strength unit is invalid"
                    |> Result.mapError InvalidStrengthUnit
                else Ok medicationStrengthUnit


            return {
                ActiveIngredientName = activeIngredientName
                BrandName = brandName
                GenericName = genericName
                MedicationId = medicationId
                Strength = medicationStrength
                StrengthNumber = medicationStrengthNumber
                StrengthUnit = medicationStrengthUnit
            }
        }

    type InvalidMedication =
        | InvalidMedicationSelection of InvalidSearchTermMedication list
        | InvalidFrequency of string
        | InvalidFillSupply of string
        | InvalidPrescribedDate of string

    // helper for API / validating a directly constructed medication
    let validatePatientMedication (patientMedicationRecord : PatientMedication) : Result<PatientMedication, InvalidMedication list> =
        let selectedMedication = patientMedicationRecord.MedicationSelection
        let drugFrequency = patientMedicationRecord.Frequency
        let drugFillSupply = patientMedicationRecord.FillSupply
        let prescribeDate = patientMedicationRecord.PrescribedDate

        validation {
            let! _ =
                selectedMedication
                |> validateSearchTermMedication
                |> Result.mapError InvalidMedicationSelection

            and! _ =
                if String.IsNullOrWhiteSpace drugFrequency
                then Error "Can't have an empty drug frequency"
                else Ok drugFrequency
                |> Result.mapError InvalidFrequency

            and! _ =
                if drugFillSupply <= 0
                then Error "Fill supply must be a positive integer"
                else Ok drugFillSupply
                |> Result.mapError InvalidFillSupply

            and! _ =
                if DateTimeOffset.Now.Date >= prescribeDate.Date
                then Ok prescribeDate
                else Error "Presribe date cannot be in the future"
                |> Result.mapError InvalidPrescribedDate

            return {|
                MedicationSelection = selectedMedication
                Frequency = drugFrequency
                FillSupply = drugFillSupply
                PrescribedDate = prescribeDate
            |}

        }


/// <summary>Drug Finder with Frequency, Fill Supply and Prescribed Date</summary>
/// <param name="model">The <see cref="M:FormComposeState">FormComposeState</see> model wich contains a dictionary of forms</param>
/// <returns> Returns a json serialized : Set of {| MedicationSelection: MedicationSelection; Frequency: string; FillSupply: int; PrescribedDate: System.DateTime |} </returns>

// let test = Thoth.Json.Decode.Auto.fromString< {| MedicationSelection: MedicationSelection; Frequency: string; FillSupply: int; PrescribedDate: System.DateTime |}>("")
[<ReactComponent>]
let DrugFinderWithFrequency (props : Field.ReactComponentField.ReactComponentFieldProps) =
    // let patientMedications, setPatientMedications = React.useState<list<MedicationSelection>> []
    let tempPatientMedication, setTempPatientMedication = React.useState<PatientMedication option> None
    let isTempPatientMedicationValid, setIsTempPatientMedicationValid = React.useState false
    let drugFinderKey, setDrugFinderKey = React.useState (System.Guid.NewGuid().ToString())
    let isReadOnly, setIsReadOnly = React.useState props.Disabled

    let patientMedications, setPatientMedications =
        let decodedMedication =
            match Thoth.Json.Decode.Auto.fromString<Map<MedicationsBySearchTerm, PatientMedication>>( props.Value ) with
            | Ok medication -> medication
            | Error e -> Map.empty
        React.useState<Map<MedicationsBySearchTerm, PatientMedication>> decodedMedication

    let validDrugResult, setValidDrugResult = React.useState<Result<PatientMedication, SearchTermMedication.InvalidMedication list> option> None


    React.useEffect(
        fun _ ->
            let isTempPatientMedicationValid =
                match tempPatientMedication with
                | Some tempMed ->
                    SearchTermMedication.validatePatientMedication tempMed
                    |> function
                        | Ok _ -> true
                        | Error _ -> false
                | None -> false
            setIsTempPatientMedicationValid isTempPatientMedicationValid

        , [| box tempPatientMedication |]
    )

    let validateTempPatientMedicationWrapper () =
        match tempPatientMedication with
        | Some tempPatientMed ->
            let validationResultOpt =
                SearchTermMedication.validatePatientMedication tempPatientMed
                |> Some
            setValidDrugResult validationResultOpt
            validationResultOpt
        | None -> None

    let validateAndAdd () =
        match validateTempPatientMedicationWrapper() with
        | Some (Ok _) ->
            setTempPatientMedication None
            setDrugFinderKey (System.Guid.NewGuid().ToString())
        | None
        | Some (Error _) -> ()

    let validateAndDone () =
        match validateTempPatientMedicationWrapper() with
        | Some (Ok _)
        | None ->
            setTempPatientMedication None
            setIsReadOnly true
            props.OnChange (
                let patientMedicationsJsonSerialized =
                    patientMedications
                    |> Thoth.Json.Encode.Auto.toString
                patientMedicationsJsonSerialized
            )
        | Some (Error _) -> ()

    // ugly
    let checkForValidationError fieldName =
        match validDrugResult with
        | None
        | Some (Ok _) -> None
        | Some (Error errs) ->
            errs
            |> List.tryFind (
                fun x ->
                    match fieldName, x with
                    | "Frequency", SearchTermMedication.InvalidFrequency _ -> true
                    | "FillSupply", SearchTermMedication.InvalidFillSupply _ -> true
                    | "PrescribedDate", SearchTermMedication.InvalidPrescribedDate _ -> true
                    | "Medication", SearchTermMedication.InvalidMedicationSelection _ -> true
                    | _ -> false
            )
            |> function
                | Some (SearchTermMedication.InvalidFrequency err) -> Some err
                | Some (SearchTermMedication.InvalidFillSupply err) -> Some err
                | Some (SearchTermMedication.InvalidPrescribedDate err) -> Some err
                | Some (SearchTermMedication.InvalidMedicationSelection errs) -> None
                | None -> None

    React.fragment [
        patientMedications
        |> Map.map (fun k v ->
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
                                (
                                    match tempPatientMedication with
                                    | Some medication ->
                                        if isTempPatientMedicationValid && k = medication.MedicationSelection
                                        then "green"
                                        elif (not isTempPatientMedicationValid) && k = medication.MedicationSelection
                                        then "red"
                                        else "blue"
                                    | None ->
                                        "blue"
                                )
                            )
                            // style.borderRight (1, borderStyle.solid, "#F191AC")
                            style.padding 10
                            style.marginTop 10
                            style.borderRadius 5
                        ]
                        prop.children [
                            if not isReadOnly
                            then
                                Bulma.icon [
                                    prop.style [
                                        style.position.absolute
                                        style.right 25
                                    ]
                                    prop.onClick (fun _ ->
                                        let newPatientMedications =
                                            patientMedications
                                            |> Map.remove k
                                        match tempPatientMedication with
                                        | Some medication ->
                                            if k = medication.MedicationSelection
                                            then setTempPatientMedication None
                                            else ()
                                        | None -> ()

                                        setPatientMedications newPatientMedications
                                    )
                                    prop.children [
                                        Html.i [ prop.className "fas fa-times"]
                                    ]
                                ]
                            Bulma.title [
                                title.is6
                                color.hasTextBlack
                                prop.text v.MedicationSelection.BrandName
                            ]

                            Html.text (v.MedicationSelection.ActiveIngredientName)
                            Html.text (v.MedicationSelection.Strength)
                            Html.br []
                            Html.strong "Frequency: "
                            Html.small (v.Frequency + " ")
                            Html.strong "Fill Supply: "
                            Html.small (v.FillSupply.ToString() + " ")
                            Html.strong "Prescribed Date: "
                            Html.small (v.PrescribedDate.ToShortDateString())
                        ]
                    ]
                ]
            ]
        )
        |> Map.toList
        |> List.map snd |> React.fragment
        match isReadOnly, tempPatientMedication with
        | true, _
        | _, Some _ -> Html.none
        | _, None ->
            Html.div [
                prop.key drugFinderKey
                prop.children [
                    DrugFinder {
                        Value = ""
                        Disabled = false
                        OnChange = (fun medication ->
                            match Thoth.Json.Decode.Auto.fromString<MedicationsBySearchTerm>( medication ) with
                            | Ok medication ->
                                // let newMedications = (patientMedications @ [ medication ])
                                let tempMedication =
                                    {|
                                        MedicationSelection = medication
                                        Frequency = ""
                                        FillSupply = 0
                                        PrescribedDate = System.DateTime.Now
                                    |}
                                let newMedications =
                                    patientMedications
                                    |> Map.add medication tempMedication

                                setTempPatientMedication (Some tempMedication)
                                setPatientMedications newMedications
                            | Error e -> ()
                        )
                    }
                ]
            ]

        match tempPatientMedication with
        | Some tempPatientMedication ->
            Bulma.field.div [
                Bulma.label "Frequency"
                Bulma.control.div [
                    Bulma.control.hasIconsLeft
                    prop.children [
                        Bulma.input.text [
                            prop.onChange (fun (value : string) ->
                                let newTempPatientMedication =  {| tempPatientMedication with Frequency = value |}
                                setTempPatientMedication (Some newTempPatientMedication)
                                setPatientMedications (patientMedications |> Map.add tempPatientMedication.MedicationSelection newTempPatientMedication)
                            )
                        ]
                        Bulma.icon [
                            Bulma.icon.isSmall
                            Bulma.icon.isLeft
                            prop.children [
                                Html.i [ prop.className "fas fa-stopwatch" ]
                            ]
                        ]
                    ]
                ]
                match checkForValidationError "Frequency" with
                | Some err ->
                    Html.small [
                        prop.style [ style.color.red ]
                        prop.text err
                    ]
                | None -> Html.none
            ]
            Bulma.field.div [
                Bulma.label "Fill Supply"
                Bulma.control.div [
                    Bulma.control.hasIconsLeft
                    prop.children [
                        Bulma.input.number [
                            prop.onChange (fun (value : int) ->
                                let newTempPatientMedication =  {| tempPatientMedication with FillSupply = value |}
                                setTempPatientMedication (Some newTempPatientMedication)
                                setPatientMedications (patientMedications |> Map.add tempPatientMedication.MedicationSelection newTempPatientMedication)
                            )
                        ]
                        Bulma.icon [
                            Bulma.icon.isSmall
                            Bulma.icon.isLeft
                            prop.children [
                                Html.i [ prop.className "fas fa-sort-numeric-up" ]
                            ]
                        ]
                    ]
                ]
                match checkForValidationError "FillSupply" with
                | Some err ->
                    Html.small [
                        prop.style [ style.color.red ]
                        prop.text err
                    ]
                | None -> Html.none
            ]
            Bulma.field.div [
                Bulma.label "Prescribed Date"
                Bulma.control.div [
                    Bulma.control.hasIconsLeft
                    prop.children [
                        Bulma.input.date [
                            prop.defaultValue (System.DateTimeOffset.Now.Date.ToString("yyyy-MM-dd"))
                            prop.onChange (fun (value:string) ->
                                let newTempPatientMedication =  {| tempPatientMedication with PrescribedDate = System.DateTime.Parse(value) |}
                                setTempPatientMedication (Some newTempPatientMedication)
                                setPatientMedications (patientMedications |> Map.add tempPatientMedication.MedicationSelection newTempPatientMedication)
                            )
                        ]
                        Bulma.icon [
                            Bulma.icon.isSmall
                            Bulma.icon.isLeft
                            prop.children [
                                Html.i [ prop.className "fas fa-calendar" ]
                            ]
                        ]
                    ]
                ]
                match checkForValidationError "PrescribedDate" with
                | Some err ->
                    Html.small [
                        prop.style [ style.color.red ]
                        prop.text err
                    ]
                | None -> Html.none
            ]

        | None -> Html.none

        if not isReadOnly
        then
            Bulma.iconText [
                prop.style [
                    style.cursor.pointer
                ]
                prop.children [
                    Bulma.icon [
                        prop.children [
                            Html.i [ prop.className "fas fa-plus"]
                        ]
                        prop.onClick ( fun _ -> validateAndAdd () )
                    ]
                    Html.span [
                        prop.text "Add"
                        prop.onClick ( fun _ -> validateAndAdd () )
                    ]
                    Bulma.icon [
                        prop.children [
                            Html.i [ prop.className "fas fa-flag-checkered"]
                        ]
                        prop.onClick ( fun _ -> validateAndDone () )
                    ]
                    Html.span [
                        prop.text "Done"
                        prop.onClick ( fun _ -> validateAndDone () )
                    ]
                ]
            ]
        else
            if props.Disabled
            then Html.none
            else
                Bulma.iconText [
                    prop.style [
                        style.cursor.pointer
                    ]
                    prop.children [
                        Bulma.icon [
                            prop.children [
                                Html.i [ prop.className "fas fa-edit"]
                            ]
                        ]
                        Html.span [
                            prop.text "Edit"
                            prop.onClick (fun _ ->
                                setTempPatientMedication None
                                setDrugFinderKey (System.Guid.NewGuid().ToString())
                                setIsReadOnly false
                            )
                        ]
                    ]
                ]

    ]
