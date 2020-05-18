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
            Console.WriteLine("repltap - C# interactive repl");

            var provider = Configuration.GetServiceProvider();
            var interactiveLoop = provider.GetService<IInteractiveLoop>();

            await interactiveLoop.Run();
        }
    }
}
