using System;
using System.Threading.Tasks;

namespace ReplTap
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("repltap - C# interactive repl");

            await InteractiveLoop.Run();
        }

    }
}
