using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;
using ReplTap.Core;
using ReplTap.Core.Completions;

namespace ReplTap
{
    public static class Configuration
    {
        public static ServiceProvider GetServiceProvider()
        {
            var service = new ServiceCollection();

            service.AddSingleton<IConsole, ConsoleWrapper>();
            service.AddSingleton<IInteractiveLoop, InteractiveLoop>();
            service.AddSingleton<IConsoleUtil, ConsoleUtil>();
            service.AddSingleton<IReplEngine, ReplEngine>();
            service.AddSingleton<ICompletionsProvider, CompletionsProvider>();
            service.AddSingleton<IRoslynCompletionsProvider, RoslynCompletionsProvider>();
            
            var provider = service.BuildServiceProvider();
            
            return provider;
        }
    }
}