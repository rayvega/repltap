using System;
using System.Threading.Tasks;
using ReplTap.Core;

namespace ReplTap.ConsoleHost
{
    public interface IInteractiveLoop
    {
        Task Run();
    }

    public class InteractiveLoop : IInteractiveLoop
    {
        private const string Prompt = ">";

        public async Task Run()
        {
            while (true)
            {
                try
                {
                    Console.Write($"{Prompt} ");
                    var input = await ConsoleUtil.ReadLine(Prompt);
                    Console.WriteLine($"{Prompt} {input}");

                    var output = await ReplEngine.Execute(input);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(output);
                    Console.ResetColor();
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(exception.Message);
                    Console.ResetColor();
                }
            }
            
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
