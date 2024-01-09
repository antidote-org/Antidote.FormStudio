module Antidote.FormStudio.i18n.Util

open System
open Fable.I18Next
open Resources

let changeLanguage (lang: string) =
    I18n.ChangeLanguage(lang) |> Promise.start

let t (key:string) = I18n.Translate key

type Intl = Keys.Intl

let defaultLanguage = "en"

let supportedLanguages =
    [
        "en", "English"
        "es", "Español"
        "fr", "Français"
        "cr", "Kreyòl"

    ]
    |> Map.ofList

I18n.Init(resources, defaultLanguage) |> Promise.start
