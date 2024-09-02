namespace Antidote.Form.Designer.Bulma

open Fable.Form
open Elmish
open Feliz
open Feliz.Bulma
open Antidote.Form.Designer
open Antidote.Form.Designer.Bulma
open Fable.Form.Extensions

module FormList =

    [<NoComparison; NoEquality>]
    type Form<'Values, 'Field> =
        {
            Fields: Base.FilledField<'Field> list
            Delete: unit -> 'Values
        }

    type Attributes =
        {
            Label: string
            Add: string option
            Delete: string option
        }

    [<NoComparison; NoEquality>]
    type FormListBag<'Values, 'Field> =
        {
            Forms: Form<'Values, 'Field> list
            Add: unit -> 'Values
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type Config<'Values, 'ElementValues> =
        {
            Value: 'Values -> 'ElementValues list
            Update: 'ElementValues list -> 'Values -> 'Values
            Default: 'ElementValues
            Attributes: Attributes
        }

    [<NoComparison; NoEquality>]
    type ElementState<'Values, 'ElementValues> =
        {
            Index: int
            Update: 'ElementValues -> 'Values -> 'Values
            Values: 'Values
            ElementValues: 'ElementValues
        }

    let form<'Values, 'Field, 'ElementValues, 'Output>
        (tagger: FormListBag<'Values, 'Field> -> 'Field)
        (formConfig: Config<'Values, 'ElementValues>)
        (buildElement: ElementState<'Values, 'ElementValues> -> Base.FilledForm<'Output, 'Field>)
        : Base.Form<'Values, 'Output list, 'Field>
        =

        Base.custom (fun values ->
            let listOfElementValues = formConfig.Value values

            let elementForIndex index elementValues =
                buildElement
                    {
                        Update =
                            fun newElementValues values ->
                                let newList = List.setAt index newElementValues listOfElementValues

                                formConfig.Update newList values
                        Index = index
                        Values = values
                        ElementValues = elementValues
                    }

            let filledElements = List.mapi elementForIndex listOfElementValues

            let toForm (index: int) (form: Base.FilledForm<'Output, 'Field>) =
                {
                    Fields = form.Fields
                    Delete =
                        fun () ->
                            let previousForms = List.take index listOfElementValues

                            let nextForms = List.drop (index + 1) listOfElementValues

                            formConfig.Update (previousForms @ nextForms) values
                }

            let gatherResults
                (next: Base.FilledForm<'Output, 'Field>)
                (current: Result<'Output list, Error.Error * Error.Error list>)
                : Result<'Output list, Error.Error * Error.Error list>
                =

                match next.Result with
                | Ok output -> Result.map (fun x -> output :: x) current

                | Error(head, errors) ->
                    match current with
                    | Ok _ -> Error(head, errors)

                    | Error(currentHead, currentErrors) ->
                        Error(head, errors @ (currentHead :: currentErrors))

            let result = List.foldBack gatherResults filledElements (Ok [])

            let isEmpty =
                List.fold
                    (fun state (element: Base.FilledForm<'Output, 'Field>) ->
                        element.IsEmpty && state
                    )
                    false
                    filledElements

            {
                State =
                    tagger
                        {
                            Forms = List.mapi toForm filledElements
                            Add =
                                fun _ ->
                                    formConfig.Update
                                        (listOfElementValues @ [ formConfig.Default ])
                                        values
                            Attributes = formConfig.Attributes
                        }
                Result = result
                IsEmpty = isEmpty
            }
        )

    module View =

        [<NoComparison; NoEquality>]
        type FormListConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                Forms: ReactElement list
                Label: string
                Add:
                    {|
                        Action: unit -> 'Msg
                        Label: string
                    |} option
                Disabled: bool
            }

        [<NoComparison; NoEquality>]
        type FormListItemConfig<'Msg> =
            {
                Dispatch: Dispatch<'Msg>
                Fields: ReactElement list
                Delete:
                    {|
                        Action: unit -> 'Msg
                        Label: string
                    |} option
                Disabled: bool
            }

        let formList
            ({
                 Dispatch = dispatch
                 Forms = forms
                 Label = label
                 Add = add
                 Disabled = disabled
             }: FormListConfig<'Msg>)
            =

            let addButton =
                match disabled, add with
                | (false, Some add) ->
                    Bulma.button.a
                        [
                            prop.onClick (fun _ -> add.Action() |> dispatch)

                            prop.children
                                [
                                    Bulma.icon
                                        [
                                            icon.isSmall

                                            prop.children
                                                [ Html.i [ prop.className "fas fa-plus" ] ]
                                        ]

                                    Html.span add.Label
                                ]
                        ]

                | _ -> Html.none

            Bulma.field.div
                [
                    Bulma.control.div
                        [
                            Internal.View.fieldLabel label

                            yield! forms

                            addButton
                        ]
                ]

        let formListItem
            ({
                 Dispatch = dispatch
                 Fields = fields
                 Delete = delete
                 Disabled = disabled
             }: FormListItemConfig<'Msg>)
            =

            let removeButton =
                match disabled, delete with
                | (false, Some delete) ->
                    Bulma.button.a
                        [
                            prop.onClick (fun _ -> delete.Action() |> dispatch)

                            prop.children
                                [
                                    Bulma.icon
                                        [
                                            icon.isSmall

                                            prop.children
                                                [ Html.i [ prop.className "fas fa-times" ] ]
                                        ]

                                    if delete.Label <> "" then
                                        Html.span delete.Label
                                ]
                        ]

                | _ -> Html.none

            Html.div
                [
                    prop.className "form-list"

                    prop.children
                        [
                            yield! fields

                            Bulma.field.div
                                [
                                    field.isGrouped
                                    field.isGroupedRight

                                    prop.children [ Bulma.control.div [ removeButton ] ]
                                ]
                        ]
                ]

    type FormListField<'Values, 'Field, 'Output, 'Value, 'Attributes>
        (innerField: FormListBag<'Values, IField<'Values, 'Attributes>>)
        =

        interface IField<'Values, 'Attributes> with
            member this.RenderField
                (onBlur: OnBlur<'Msg>)
                (dispatch: Dispatch<'Msg>)
                (fieldConfig: Form.View.FieldConfig<'Values, 'Msg>)
                (filledField: FilledField<'Values, 'Attributes>)
                =
                View.formList
                    {
                        Dispatch = dispatch
                        Forms =
                            innerField.Forms
                            |> List.map (fun
                                             {
                                                 Fields = fields
                                                 Delete = delete
                                             } ->
                                View.formListItem
                                    {
                                        Dispatch = dispatch
                                        Fields =
                                            List.map (Internal.View.renderField dispatch fieldConfig) fields
                                        Delete =
                                            innerField.Attributes.Delete
                                            |> Option.map (fun deleteLabel ->
                                                {|
                                                    Action = delete >> fieldConfig.OnChange
                                                    Label = deleteLabel
                                                |}
                                            )
                                        Disabled = filledField.IsDisabled || fieldConfig.Disabled
                                    }
                            )
                        Label = innerField.Attributes.Label
                        Add =
                            innerField.Attributes.Add
                            |> Option.map (fun addLabel ->
                                {|
                                    Action = innerField.Add >> fieldConfig.OnChange
                                    Label = addLabel
                                |}
                            )
                        Disabled = filledField.IsDisabled || fieldConfig.Disabled
                    }

            member _.MapFieldValues
                (update: 'Values -> 'NewValues -> 'NewValues)
                (values: 'NewValues)
                : IField<'NewValues, 'Attributes>
                =
                FormListField
                    {
                        Forms =
                            List.map
                                (fun (form: Form<'Values, IField<'Values, 'Attributes>>) ->
                                    {
                                        Fields =
                                            List.map
                                                (fun
                                                    (filledField:
                                                        Base.FilledField<
                                                            IField<'Values, 'Attributes>
                                                         >) ->
                                                    {
                                                        State =
                                                            filledField.State.MapFieldValues
                                                                update
                                                                values
                                                        Error = filledField.Error
                                                        IsDisabled = filledField.IsDisabled
                                                    }
                                                )
                                                form.Fields
                                        Delete = fun _ -> update (form.Delete()) values
                                    }
                                )
                                innerField.Forms
                        Add = fun _ -> update (innerField.Add()) values
                        Attributes = innerField.Attributes
                    }
