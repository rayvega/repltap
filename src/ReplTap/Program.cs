using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReplTap.ConsoleHost;

namespace ReplTap
{
    public static class Program
    {
        public static async Task Main()
        {
            var version = typeof(Program)
                .Assembly
                .GetName()
                .Version?
                .ToString()[..^2];

            Console.WriteLine($"repltap - C# interactive repl - v{version}");

            var provider = Configuration.GetServiceProvider();
            var interactiveLoop = provider.GetService<IInteractiveLoop>();

            await interactiveLoop.Run();
        }
    }
}
