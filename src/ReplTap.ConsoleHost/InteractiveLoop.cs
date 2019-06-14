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
        private readonly ILoop _loop;

        public InteractiveLoop(IConsole console, IConsoleReader consoleReader, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop)
        {
            _console = console;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
            _replEngine = replEngine;
            _loop = loop;
        }

        private const string Prompt = ">";

        public async Task Run()
        {
            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{Prompt} ");
                    var input = await _consoleReader.ReadLine(Prompt);
                    _console.WriteLine($"{Prompt} {input}");

                    var result = await _replEngine.Execute(input);
                    _consoleWriter.WriteOutput(result.Output);
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
