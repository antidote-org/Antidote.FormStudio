module Antidote.React.Components.FormWizard.Processors.Components

open Feliz
open Feliz.Bulma
open Feliz.UseElmish
open Elmish
open Antidote.Core.FormProcessor
open Antidote.Core.FormProcessor.Types
open Antidote.Core.FormProcessor.Spec.v2_0_1
open Fable.Core.JsInterop
open Antidote.Core.V2.Utils.JS
open Fable.Form.Antidote
open Antidote.Core.FormProcessor.Helper
open Antidote.FormStudio.i18n.Util

let private classes: CssModules.UI.Default =
    import "default" "./Default.module.scss"

module ADA =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 4) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Not high risk. T2DM screening not recommended by ADA guidelines." ]
            else if (score > 4) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [ "Scores ≥ 5 should be formally screened for diabetes, per ADA guidelines." ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]]
            ]

let clinicalCode (code: string * string) =
    Html.p
        [
            prop.className ""
            prop.style [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
            prop.children
                [
                    Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                    Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                ]
        ]

module CAGE =
    let private classes: CssModules.UI.CAGE = import "default" "./CAGE.module.scss"

    let screeningVerbiage score : ResizeArray<string> =
        let screeningVerbiage: ResizeArray<string> =
            match score with
            | 0
            | 1 -> ResizeArray [ "Screening Negative" ]
            | 2
            | 3
            | 4 ->
                ResizeArray
                    [
                        "Scores of 2 or higher had a 93% sensitivity / 76% specificity for the identification of “excessive drinking” and a 91% sensitivity / 77% specificity for the identification of alcoholism."
                    ]
            | _ -> ResizeArray []

        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screeningVerbiage score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]

                              if (flags |> Set.contains "EyeOpener") then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [
                                                          Html.span
                                                              "Some clinicians also consider the eye opener question as highly concerning for unhealthy drinking behavior, even if all other questions are answered negatively."
                                                      ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none

                              ]
            ]

module CAT =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 10) then
                [ (t Intl.CatLowRisk.Key) ]
            else if (score >= 11 && score <= 20) then
                [ (t Intl.CatModerateRisk.Key) ]
            else if (score >= 21 && score <= 30) then
                [ (t Intl.CatHighRisk.Key) ]
            else if (score >= 31 && score <= 40) then
                [ (t Intl.CatVeryHighRisk.Key) ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module CKD =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 3) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Low Risk" ]
            else if (score > 3) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [ "Escalate" ]
            else
                []
            |> ResizeArray

        // screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score suicidal =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]

                              if suicidal then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [ Html.span (t Intl.SuicidalWarning.Key) ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none

                              ]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module LACE =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 4) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Low risk of addmission" ]
            else if (score >= 5 && score <= 9) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [ "Moderate risk of addmission" ]
            else if (score >= 10 && score <= 14) then
                // [(t Intl.ScreeningTenToFourteen.Key)]
                [ "High risk of addmission" ]
            else
                []
            |> ResizeArray

        // screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module MWC =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 27) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Low Risk" ]
            else if (score >= 28 && score <= 56) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Moderate Risk" ]
            else if (score > 56) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [ "High Risk" ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score suicidal =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]

                              if suicidal then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [ Html.span (t Intl.SuicidalWarning.Key) ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none

                              ]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module PHQ9 =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 4) then
                [ (t Intl.ScreeningZeroToFour.Key) ]
            else if (score >= 5 && score <= 9) then
                [ (t Intl.ScreeningFiveToNine.Key) ]
            else if (score >= 10 && score <= 14) then
                [ (t Intl.ScreeningTenToFourteen.Key) ]
            else if (score >= 15 && score <= 19) then
                [ (t Intl.ScreeningFifteenToNineteen.Key) ]
            else if (score >= 20) then
                [ (t Intl.ScreeningTwentyOrGreater.Key) ]
            else
                []
            |> ResizeArray

        screeningVerbiage.Add(t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]

                              if (flags |> Set.contains "IsSuicidal") then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [ Html.span (t Intl.SuicidalWarning.Key) ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module DSM5TR =
    let private classes: CssModules.UI.Default =
        import "default" "./Default.module.scss"

    let screenings score =
        let screeningVerbiage =
            if (score >= 0 && score <= 1) then
                [ "Zero or one symptom indicates low risk for a substance use disorder." ]
            else if (score >= 2 && score <= 3) then
                [ "Two or three symptoms indicate a mild substance use disorder." ]
            else if (score >= 4 && score <= 5) then
                [ "Four or five symptoms indicate a moderate substance use disorder." ]
            else if (score >= 6) then
                [ "Six or more symptoms indicate a severe substance use disorder." ]
            else
                []
            |> ResizeArray

        // screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]
                              // Html.div [
                              //     prop.style [style.color "#FFF";]
                              //     prop.children [
                              //         for a in screening score do
                              //             Html.div [ Html.span a ]
                              //             Html.br []
                              //     ]
                              // ]
                              if ((flags |> Set.count) > 0) then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [
                                                          Html.span
                                                              "WARNING: Patient is at risk for substance use disorder. Substance use disorders should be evaluated by a psychiatrist, psychologist, or licensed counselor specializing in drug and alcohol addictions."
                                                      ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module SCORED =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 3) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Low Risk" ]
            else if (score > 3) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [
                    "Escalate: Individuals from general healthy adult populations with a cumulative score ≥ 4 have demonstrated an approximately 20% chance of having CKD, defined as an estimated glomerular filtration rate (eGFR) of <60 ml/min/1.732."
                ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module SDOH =
    let private classes: CssModules.UI.Default =
        import "default" "./Default.module.scss"

module Seattle7 =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 30) then
                [ (Antidote.FormStudio.i18n.Util.t Intl.DailyAngina.Key) ]
            else if (score >= 31 && score <= 60) then
                [ (Antidote.FormStudio.i18n.Util.t Intl.WeeklyAngina.Key) ]
            else if (score >= 61 && score <= 99) then
                [ (Antidote.FormStudio.i18n.Util.t Intl.MonthlyAngina.Key) ]
            else if (score = 100) then
                [ (Antidote.FormStudio.i18n.Util.t Intl.NoAngina.Key) ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score flags =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (Antidote.FormStudio.i18n.Util.t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module MSE =

    let screening score =
        let screeningVerbiage =
            if (score >= 0 && score <= 2) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Low Risk" ]
            else if (score >= 3 && score <= 5) then
                // [(t Intl.ScreeningZeroToFour.Key)]
                [ "Moderate Risk" ]
            else if (score > 5) then
                // [(t Intl.ScreeningFiveToNine.Key)]
                [ "High Risk" ]
            else
                []
            |> ResizeArray

        //screeningVerbiage.Add (t Intl.FunctionalPatient.Key)
        screeningVerbiage

    let formResult score suicidal =
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[Html.span
                                  [
                                      prop.className classes.pointsScore
                                      // prop.style [ style.fontSize 30; style.fontWeight 700; style.color "#ceff00" ]
                                      prop.text (score.ToString())
                                  ]

                              Html.span
                                  [
                                      prop.className classes.pointsLabel
                                      prop.text (t Intl.Points.Key)
                                  ]

                              Html.div
                                  [
                                      prop.style [ style.color "#FFF" ]
                                      prop.children
                                          [
                                              for a in screening score do
                                                  Html.div [ Html.span a ]
                                                  Html.br []
                                          ]
                                  ]

                              if suicidal then
                                  Html.div
                                      [
                                          prop.style [ style.color "#ceff00" ]
                                          prop.children
                                              [
                                                  Html.div
                                                      [ Html.span (t Intl.SuicidalWarning.Key) ]
                                                  Html.br []
                                              ]
                                      ]
                              else
                                  Html.none

                              ]
            ]

    let clinicalCode (code: string * string) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.strong [ color.hasTextInfo; prop.text (code |> fst) ]
                        Html.strong [ color.hasTextSuccessDark; prop.text (code |> snd) ]
                    ]
            ]

module Helpers =

    let clinicalNote (note: FieldResult) =
        Html.p
            [
                prop.className ""
                prop.style
                    [ style.paddingBottom 10; style.borderBottom (1, borderStyle.solid, "#CCC") ]
                prop.children
                    [
                        Html.span "When "
                        Html.strong [ color.hasTextInfo; prop.text note.Patient ]
                        Html.span " was asked: "
                        Html.strong [ color.hasTextSuccessDark; prop.text note.Question ]
                        // Html.br []
                        Html.span " The patient reported: "
                        Html.strong [ color.hasTextDangerDark; prop.text note.Answer ]
                    ]
            ]

let formResultRenderer (resultList: List<RenderingMode>) =
    if resultList.Length > 0 then
        Html.div
            [
                prop.className classes.resultWindow
                prop.children[

                              resultList
                              |> List.ofSeq
                              |> Seq.map (fun i ->
                                  match i with

                                  | RenderingMode.LargeWithSubtitle(large, subTitle) ->

                                      React.fragment
                                          [
                                              Html.span
                                                  [
                                                      prop.className classes.pointsScore
                                                      prop.text (large)
                                                  ]

                                              Html.span
                                                  [
                                                      prop.className classes.pointsLabel
                                                      prop.text subTitle
                                                  ]

                                          ]
                                  | RenderingMode.Normal str ->
                                      Html.div
                                          [
                                              prop.style
                                                  [ style.color "#FFF"; style.display.block ]
                                              prop.text str
                                          ]

                                  | RenderingMode.Warning str ->
                                      Html.span
                                          [
                                              prop.style
                                                  [ style.color "#ceff00"; style.display.block ]
                                              prop.children
                                                  [ Html.span [ Html.span str ]; Html.br [] ]
                                          ]

                              )
                              |> React.fragment]
            ]
    else
        Html.none

let private resultOutput =
    [
        "ada", Antidote.Core.FormProcessor.Processors.ADA.screenings
        "cage", Antidote.Core.FormProcessor.Processors.CAGE.screenings
        "cat", Antidote.Core.FormProcessor.Processors.CAT.screenings
        "lace", Antidote.Core.FormProcessor.Processors.LACE.screenings
        "phq9", Antidote.Core.FormProcessor.Processors.PHQ9.screenings
        "seattle7", Antidote.Core.FormProcessor.Processors.Seattle7.screenings
        "scored", Antidote.Core.FormProcessor.Processors.SCORED.screenings
        // added, disable if not required but they existed here
        // "ckdia",        Antidote.Core.FormProcessor.Processors.CKD.screenings
        // "mwc",          Antidote.Core.FormProcessor.Processors.MWC.screenings
        // "mse",          Antidote.Core.FormProcessor.Processors.MSE.screenings
        "dsm5tr", Antidote.Core.FormProcessor.Processors.DSM5TR.screenings
    ]

let getResultOutput (formName: string) =
    resultOutput
    |> Map.ofList
    |> Map.tryFind (formName.ToLower())
    |> Option.defaultValue (fun score flags ->
        [ RenderingMode.LargeWithSubtitle($"{score}", ((t Intl.Points.Key))) ]
    )
