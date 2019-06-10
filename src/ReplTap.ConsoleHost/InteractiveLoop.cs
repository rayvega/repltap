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
        private readonly IConsoleUtil _consoleUtil;
        private readonly IReplEngine _replEngine;

        public InteractiveLoop(IConsoleUtil consoleUtil, IReplEngine replEngine)
        {
            _consoleUtil = consoleUtil;
            _replEngine = replEngine;
        }

        private const string Prompt = ">";

        public async Task Run()
        {
            while (true)
            {
                try
                {
                    Console.Write($"{Prompt} ");
                    var input = await _consoleUtil.ReadLine(Prompt);
                    Console.WriteLine($"{Prompt} {input}");

                    var output = await _replEngine.Execute(input);
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
