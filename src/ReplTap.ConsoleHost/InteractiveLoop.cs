using System;
using System.Threading.Tasks;
using ReplTap.Core;

namespace ReplTap.ConsoleHost
{
    public static class InteractiveLoop
    {
        private const string Prompt = ">";

        public static async Task Run()
        {
            while (true)
            {
                try
                {
                    Console.Write($"{Prompt} ");
                    var input = await ConsoleUtil.ReadLine(Prompt);
                    Console.WriteLine($"{Prompt} {input}");

                    var output = await ReplEngine.Execute(input);
                    Console.WriteLine(output);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
            
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
