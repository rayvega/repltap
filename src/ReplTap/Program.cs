using System;
using System.Threading.Tasks;
using ReplTap.ConsoleHost;

namespace ReplTap
{
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("repltap - C# interactive repl");

            await InteractiveLoop.Run();
        }

    }
}
