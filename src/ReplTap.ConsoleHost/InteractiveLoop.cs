using System;
using System.Threading.Tasks;

namespace ReplTap.ConsoleHost
{
    public class InteractiveLoop
    {
        public async static Task Run()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                var output = await ReplEngine.Execute(input);
                Console.WriteLine(output);
            }
        }
    }
}
