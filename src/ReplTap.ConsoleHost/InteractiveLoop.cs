using System;
using System.Text;
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
        
        private string _prompt = Prompt.Standard;

        public InteractiveLoop(IConsole console, IConsoleReader consoleReader, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop)
        {
            _console = console;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
            _replEngine = replEngine;
            _loop = loop;
        }

        public async Task Run()
        {
            var codes = new StringBuilder();

            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{_prompt} ");
                    var input = await _consoleReader.ReadLine(_prompt);
                    codes.Append(input);
                    _console.WriteLine($"{_prompt} {input}");

                    var result = await _replEngine.Execute(codes.ToString());

                    if (result.State == OutputState.Continue)
                    {
                        _prompt = Prompt.Continue;
                    }
                    else
                    {
                        _consoleWriter.WriteOutput(result.Output);
                        codes.Clear();
                        _prompt = Prompt.Standard;
                    }
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
