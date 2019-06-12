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
        private readonly IConsoleWriter _consoleWriter;

        public InteractiveLoop(IConsole console, IConsoleReader consoleReader, IConsoleWriter consoleWriter,
            IReplEngine replEngine)
        {
            _console = console;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
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
                    _consoleWriter.WriteOutput(output);
                }
                catch (Exception exception)
                {
                    _consoleWriter.WriteError(exception.Message);
                }
            }
            
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
