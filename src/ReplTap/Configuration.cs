using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;
using ReplTap.ConsoleHost.Commands;
using ReplTap.Core;
using ReplTap.Core.Completions;
using ReplTap.Core.History;

namespace ReplTap
{
    public static class Configuration
    {
        public static ServiceProvider GetServiceProvider()
        {
            var service = new ServiceCollection();

            service.AddSingleton<IConsole, ConsoleWrapper>();
            service.AddSingleton<IInteractiveLoop, InteractiveLoop>();
            service.AddSingleton<ILoop, Loop>();
            service.AddSingleton<IInputCheck, InputCheck>();
            service.AddSingleton<IConsoleKeyHandler, ConsoleKeyHandler>();
            service.AddSingleton<IConsoleKeyCommands, ConsoleKeyCommands>();
            service.AddSingleton<INavigateCommands, NavigateCommands>();
            service.AddSingleton<IEditCommands, EditCommands>();
            service.AddSingleton<IConsoleWriter, ConsoleWriter>();
            service.AddSingleton<ICompletionsWriter, CompletionsWriter>();

            service.AddSingleton<IInputHistory, InputHistory>();
            service.AddSingleton<IReplEngine, ReplEngine>();
            service.AddSingleton<IScriptOptionsBuilder, ScriptOptionsBuilder>();
            service.AddSingleton<ICompletionsProvider, CompletionsProvider>();
            service.AddSingleton<IVariablesFilter, VariablesFilter>();
            service.AddSingleton<IRoslynCompletionsProvider, RoslynCompletionsProvider>();
            service.AddSingleton<ICompletionsParser, CompletionsParser>();
            service.AddSingleton<ICompletionsFilter, CompletionsFilter>();

            var provider = service.BuildServiceProvider();

            return provider;
        }
    }
}
