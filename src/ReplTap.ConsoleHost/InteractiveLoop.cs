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

        public InteractiveLoop(IConsole console, IConsoleKeyHandler consoleKeyHandler, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop)
        {
            _console = console;
            _consoleKeyHandler = consoleKeyHandler;
            _consoleWriter = consoleWriter;
            _replEngine = replEngine;
            _loop = loop;
        }

        public async Task Run()
        {
            var codes = new StringBuilder();

            var state = new ConsoleState
            {
                ColPosition = _console.CursorLeft,
            };

            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{state.Prompt} ");
                    var input = _consoleKeyHandler.Process(state, Prompt.Standard, new InputHistory(), new List<string>()); // *TODO*: remove these unnecessary params
                    codes.AppendLine(input);
                    _console.WriteLine();

                    var result = await _replEngine.Execute(codes.ToString());

                    switch (result.State)
                    {
                        case OutputState.Continue:
                            state.Prompt = Prompt.Continue;
                            break;
                        case OutputState.Error:
                            _consoleWriter.WriteError(result.Output ?? "");
                            CompleteInput(codes, state, result.Variables);
                            break;
                        default:
                            _consoleWriter.WriteOutput(result.Output ?? "");
                            CompleteInput(codes, state, result.Variables);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    _console.WriteLine();
                    _consoleWriter.WriteError(exception);
                }
            }
        }

        private void CompleteInput(StringBuilder codes, ConsoleState state, List<string>? variables)
        {
            state.InputHistory.Add(codes.ToString().TrimEnd());
            codes.Clear();
            state.Variables = variables ?? state.Variables;

            state.Prompt = Prompt.Standard;
        }
    }
}
