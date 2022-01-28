using System;
using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core;
using ReplTap.Core.Completions;
using ReplTap.Core.History;

namespace ReplTap;

public static class Configuration
{
    public static IServiceProvider GetServiceProvider()
    {
        var service = new ServiceCollection();

        service.AddSingleton<IConsole, ConsoleWrapper>()
            .AddSingleton<IInteractiveLoop, InteractiveLoop>()
            .AddSingleton<ILoop, Loop>()
            .AddSingleton<IInputCheck, InputCheck>()
            .AddSingleton<IConsoleKeyHandler, ConsoleKeyHandler>()
            .AddSingleton<IConsoleState, ConsoleState>()
            .AddSingleton<IConsoleKeyCommands, ConsoleKeyCommands>()
            .AddSingleton<INavigateCommands, NavigateCommands>()
            .AddSingleton<IEditCommands, EditCommands>()
            .AddSingleton<IInputHistoryCommands, InputHistoryCommands>()
            .AddSingleton<ICompletionsCommands, CompletionsCommands>()
            .AddSingleton<IConsoleWriter, ConsoleWriter>()
            .AddSingleton<ICompletionsWriter, CompletionsWriter>()

            .AddSingleton<IInputHistory, InputHistory>()
            .AddSingleton<IReplEngine, ReplEngine>()
            .AddSingleton<IScriptOptionsBuilder, ScriptOptionsBuilder>()
            .AddSingleton<ICompletionsProvider, CompletionsProvider>()
            .AddSingleton<IVariablesFilter, VariablesFilter>()
            .AddSingleton<IRoslynCompletionsProvider, RoslynCompletionsProvider>()
            .AddSingleton<ICompletionsParser, CompletionsParser>()
            .AddSingleton<ICompletionsFilter, CompletionsFilter>();

        var provider = service.BuildServiceProvider();

        return provider;
    }
}
