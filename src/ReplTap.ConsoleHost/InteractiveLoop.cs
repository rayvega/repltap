using System;
using System.Collections.Generic;
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
        private readonly IConsoleKeyHandler _consoleKeyHandler;
        private readonly IReplEngine _replEngine;
        private readonly IConsoleWriter _consoleWriter;
        private readonly ILoop _loop;
        private readonly IInputHistory _inputHistory;
        private List<string> _variables = new List<string>();
        private string _prompt = Prompt.Standard;

        public InteractiveLoop(IConsole console, IConsoleKeyHandler consoleKeyHandler, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop, IInputHistory inputHistory)
        {
            _console = console;
            _consoleKeyHandler = consoleKeyHandler;
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
                    var input = _consoleKeyHandler.Process(_prompt, _inputHistory, _variables);
                    codes.Append(input);
                    _console.WriteLine();

                    var result = await _replEngine.Execute(codes.ToString());

                    switch (result.State)
                    {
                        case OutputState.Continue:
                            _prompt = Prompt.Continue;
                            break;
                        case OutputState.Error:
                            _consoleWriter.WriteError(result.Output ?? "");
                            CompleteInput(codes, _inputHistory, result.Variables);
                            break;
                        default:
                            _consoleWriter.WriteOutput(result.Output ?? "");
                            CompleteInput(codes, _inputHistory, result.Variables);
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

        private void CompleteInput(StringBuilder codes, IInputHistory inputHistory, List<string>? variables)
        {
            inputHistory.Add(codes.ToString());
            codes.Clear();
            _variables = variables ?? _variables;

            _prompt = Prompt.Standard;
        }
    }
}
