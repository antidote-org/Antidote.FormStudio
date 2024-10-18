module EasyBuild.Commands.Demo

open Spectre.Console.Cli
open SimpleExec
open EasyBuild.Workspace
open BlackFox.CommandLine
open System.IO

type DemoSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    member val IsWatch = false with get, set

type DemoCommand() =
    inherit Command<DemoSettings>()
    interface ICommandLimiter<DemoSettings>

    override __.Execute(context, settings) =

        Command.Run("pnpm", "install", workingDirectory = Workspace.``.``)

        let cssModulesFileInfo = FileInfo(VirtualWorkspace.src.``CssModules.fs``)

        cssModulesFileInfo.Delete()

        Command.Run("npx", "fcm", workingDirectory = Workspace.src.``.``)

        if settings.IsWatch then
            Async.Parallel [
                Command.RunAsync(
                    "dotnet",
                    CmdLine.empty
                    |> CmdLine.appendRaw "fable"
                    |> CmdLine.appendRaw "watch"
                    |> CmdLine.appendRaw Workspace.demo.``.``
                    |> CmdLine.appendRaw "--test:MSBuildCracker"
                    |> CmdLine.appendRaw "--verbose"
                    |> CmdLine.toString
                )
                |> Async.AwaitTask

                Command.RunAsync("npx", "vite", workingDirectory = Workspace.demo.``.``)
                |> Async.AwaitTask

                Command.RunAsync(
                    "npx",
                    "nodemon -e module.scss --exec \"fcm\"",
                    workingDirectory = Workspace.src.``.``
                )
                |> Async.AwaitTask
            ]
            |> Async.RunSynchronously
            |> ignore

        else
            Command.Run(
                "dotnet",
                CmdLine.empty
                |> CmdLine.appendRaw "fable"
                |> CmdLine.appendRaw Workspace.demo.``.``
                |> CmdLine.appendRaw "--test:MSBuildCracker"
                |> CmdLine.toString
            )

            Command.Run("npx", "vite build", workingDirectory = Workspace.demo.``.``)

        0
