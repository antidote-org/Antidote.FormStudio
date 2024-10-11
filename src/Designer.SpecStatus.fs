namespace Antidote.FormStudio.SpecStatus

open FsToolkit.ErrorHandling

module Types =

    [<RequireQualifiedAccess>]
    type SpecStatus =
        | Template
        | Draft
        | Published
        | UnPublished
        | Archived
        | Deleted

    module SpecStatus =

        // type T = private | SpecStatus of SpecStatus

        /// <summary>
        /// Retrieve the corresponding F# discriminated union value for the given index
        /// </summary>
        /// <param name="specStatusIndex"></param>
        /// <returns>F# value match the provided index</returns>
        let fromIndex (specStatusIndex: int) =
            match specStatusIndex with
            | 0 -> SpecStatus.Template
            | 1 -> SpecStatus.Draft
            | 2 -> SpecStatus.Published
            | 3 -> SpecStatus.UnPublished
            | 4 -> SpecStatus.Archived
            | _ -> failwith $"Invalid SpecStatus index: $%i{specStatusIndex}"

        /// <summary>
        /// Retrieve the corresponding index for the given F# discriminated union value
        /// </summary>
        /// <param name="specStatus"></param>
        /// <returns>The index corresponding to the given F# discriminated union value</returns>
        let toIndex (specStatus: SpecStatus) =
            match specStatus with
            | SpecStatus.Template -> 0
            | SpecStatus.Draft -> 1
            | SpecStatus.Published -> 2
            | SpecStatus.UnPublished -> 3
            | SpecStatus.Archived -> 4
            | SpecStatus.Deleted -> 5

        /// <summary>
        /// Retrieve the corresponding index for the given F# discriminated union value
        /// </summary>
        /// <param name="specStatus"></param>
        /// <returns>The index corresponding to the given F# discriminated union value</returns>
        let toString (specStatus: SpecStatus) =
            match specStatus with
            | SpecStatus.Template -> "Template"
            | SpecStatus.Draft -> "Draft"
            | SpecStatus.Published -> "Published"
            | SpecStatus.UnPublished -> "UnPublished"
            | SpecStatus.Archived -> "Archived"
            | SpecStatus.Deleted -> "Deleted"

        /// <summary>
        /// Retrieve the corresponding index for the given F# discriminated union value
        /// </summary>
        /// <param name="taskStatusString"></param>
        /// <returns>The index corresponding to the given F# discriminated union value</returns>
        let fromString (specStatusString: string) =
            match specStatusString with
            | "Template" -> SpecStatus.Template
            | "Draft" -> SpecStatus.Draft
            | "Published" -> SpecStatus.Published
            | "UnPublished" -> SpecStatus.UnPublished
            | "Archived" -> SpecStatus.Archived
            | "Deleted" -> SpecStatus.Deleted
            | _ -> failwith "Invalid SpecStatus string"

        let validateIndex (specStatusIndex: int) =
            match specStatusIndex with
            | 0
            | 1
            | 2
            | 3
            | 4
            | 5 -> true
            | _ -> false

        let validateString (specStatusString: string) =
            match specStatusString with
            | "Template"
            | "Draft"
            | "Published"
            | "UnPublished"
            | "Archived"
            | "Deleted" -> true
            | _ -> false

// let tryParseIndex (specStatusIndex: int) = result {
//     do! Validators.Int.isNonNegative specStatusIndex "Spec Status cannot be negative"
//     do!
//         match validateIndex specStatusIndex with
//         | false -> Error $"Spec Status index {specStatusIndex} was invalid."
//         | true -> Ok()
//     return fromIndex specStatusIndex
// }

// let tryParseString (specStatusString: string) = result {
//     do! Validators.String.nonEmpty specStatusString "Spec Status cannot be empty"
//     do!
//         match validateString specStatusString with
//         | false -> Error $"Spec Status string {specStatusString} is invalid."
//         | true -> Ok()
//     return fromString specStatusString
// }
