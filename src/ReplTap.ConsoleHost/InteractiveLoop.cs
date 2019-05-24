using System;
using System.Threading.Tasks;

namespace ReplTap.ConsoleHost
{
    public class InteractiveLoop
    {
        private const string Prompt = ">";

        public async static Task Run()
        {
            while (true)
            {
                Console.Write($"{Prompt} ");
                var input = ConsoleUtil.ReadLine();
                Console.WriteLine($"{Prompt} {input}");

                var output = await ReplEngine.Execute(input);
                Console.WriteLine(output);
            }
        }
    }
}
