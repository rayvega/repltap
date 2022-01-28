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
            if (Console.IsInputRedirected)
            {
                Console.WriteLine("Error! Either input is redirected from standard input or app doesn't have console. Unable to run console app.");

                return;
            }

            var version = typeof(Program)
                .Assembly
                .GetName()
                .Version?
                .ToString()[..^2];

            Console.WriteLine($"repltap - C# interactive repl - v{version}");

            var provider = Configuration.GetServiceProvider();
            var interactiveLoop = provider.GetRequiredService<IInteractiveLoop>();


            await interactiveLoop.Run();
        }
    }
}
