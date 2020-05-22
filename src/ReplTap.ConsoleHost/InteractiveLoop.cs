using System;
using System.Text;
using System.Threading.Tasks;
using ReplTap.Core;
using ReplTap.Core.History;

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
        private readonly IInputHistory _inputHistory;
        private string _prompt = Prompt.Standard;

        public InteractiveLoop(IConsole console, IConsoleReader consoleReader, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop, IInputHistory inputHistory)
        {
            _console = console;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
            _replEngine = replEngine;
            _loop = loop;
            _inputHistory = inputHistory;
        }

        public async Task Run()
        {
            var codes = new StringBuilder();

            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{_prompt} ");
                    var input = await _consoleReader.ReadLine(_prompt, _inputHistory);
                    codes.Append(input);
                    _console.WriteLine($"{_prompt} {input}");

                    var result = await _replEngine.Execute(codes.ToString());

                    switch (result.State)
                    {
                        case OutputState.Continue:
                            _prompt = Prompt.Continue;
                            break;
                        case OutputState.Error:
                            _consoleWriter.WriteError(result.Output ?? "");
                            CompleteInput(codes, _inputHistory);
                            break;
                        default:
                            _consoleWriter.WriteOutput(result.Output ?? "");
                            CompleteInput(codes, _inputHistory);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    _console.WriteLine();
                    _consoleWriter.WriteError(exception);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private void CompleteInput(StringBuilder codes, IInputHistory inputHistory)
        {
            inputHistory.Add(codes.ToString());
            codes.Clear();
            _prompt = Prompt.Standard;
        }
    }
}
