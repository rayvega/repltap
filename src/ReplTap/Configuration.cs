using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;

namespace ReplTap
{
    public static class Configuration
    {
        public static ServiceProvider GetServiceProvider()
        {
            var service = new ServiceCollection();
            
            service.AddSingleton<IInteractiveLoop, InteractiveLoop>();
            service.AddSingleton<IConsoleUtil, ConsoleUtil>();
            
            var provider = service.BuildServiceProvider();
            
            return provider;
        }
    }
}