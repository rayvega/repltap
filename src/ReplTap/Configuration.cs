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
            var provider = service.BuildServiceProvider();
            
            return provider;
        }
    }
}