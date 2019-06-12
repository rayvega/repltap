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
        private readonly IConsole _console;
        private readonly IConsoleReader _consoleReader;
        private readonly IReplEngine _replEngine;

        public InteractiveLoop(IConsole console, IConsoleReader consoleReader, IReplEngine replEngine)
        {
            _console = console;
            _consoleReader = consoleReader;
            _replEngine = replEngine;
        }

        private const string Prompt = ">";

        public async Task Run()
        {
            while (true)
            {
                try
                {
                    _console.Write($"{Prompt} ");
                    var input = await _consoleReader.ReadLine(Prompt);
                    _console.WriteLine($"{Prompt} {input}");

                    var output = await _replEngine.Execute(input);
                    _console.ForegroundColor = ConsoleColor.Gray;
                    _console.WriteLine(output);
                    _console.ResetColor();
                }
                catch (Exception exception)
                {
                    _console.ForegroundColor = ConsoleColor.Red;
                    _console.WriteLine(exception.Message);
                    _console.ResetColor();
                }
            }
            
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
