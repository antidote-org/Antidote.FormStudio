module Antidote.FormStudio.UI.FieldPicker

open Feliz
open Feliz.Bulma
open Antidote.FormStudio.Types
open Antidote.FormStudio.Helper

type FieldPickerProps<'UserField> =
    {|
        FormSpec: FormSpec<'UserField>
        SelectedStepNumber: int
        FormSpecChanged: FormSpec<'UserField> -> unit
        RegisteredFields: RegisteredFields<'UserField>
    |}

[<ReactComponent>]
let FieldPicker (props: FieldPickerProps<'UserField>) =
    Bulma.panel [
        props.RegisteredFields.AsList
        |> List.map (fun designerFieldType ->
            Bulma.panelBlock.a [
                prop.onClick (fun e ->
                    //add designer field type to selected step via CLICK
                    let outFormSpec =
                        addDesignerFieldTypeToStep
                            designerFieldType.Key
                            props.SelectedStepNumber
                            props.FormSpec
                            props.RegisteredFields

                    props.FormSpecChanged outFormSpec
                )
                prop.draggable true
                prop.onDragStart (fun e ->
                    let data =
                        (DragSource.Designer_FormFieldType_Key designerFieldType.Key).ToPlainText

                    e.dataTransfer.setData ("text/plain", data) |> ignore
                )
                prop.children [
                    Bulma.panelIcon [
                        Html.i [
                            prop.className designerFieldType.Icon
                        ]
                    ]
                    Html.text designerFieldType.Key
                ]
            ]
        )
        |> React.fragment
    ]
