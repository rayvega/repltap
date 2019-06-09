using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;
using ReplTap.Core;

namespace ReplTap
{
    public static class Configuration
    {
        public static ServiceProvider GetServiceProvider()
        {
            var service = new ServiceCollection();
            
            service.AddSingleton<IInteractiveLoop, InteractiveLoop>();
            service.AddSingleton<IConsoleUtil, ConsoleUtil>();
            service.AddSingleton<IReplEngine, ReplEngine>();
            
            var provider = service.BuildServiceProvider();
            
            return provider;
        }
    }
}