using System;
using System.Collections.Generic;
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
        private readonly IConsoleKeyHandler _consoleKeyHandler;
        private readonly IReplEngine _replEngine;
        private readonly IConsoleWriter _consoleWriter;
        private readonly ILoop _loop;
        private readonly IConsoleState _consoleState;

        public InteractiveLoop(IConsole console, IConsoleKeyHandler consoleKeyHandler, IConsoleWriter consoleWriter,
            IReplEngine replEngine, ILoop loop, IConsoleState consoleState)
        {
            _console = console;
            _consoleKeyHandler = consoleKeyHandler;
            _consoleWriter = consoleWriter;
            _replEngine = replEngine;
            _loop = loop;
            _consoleState = consoleState;
        }

        public async Task Run()
        {
            var codes = new StringBuilder();

            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{_consoleState.Prompt} ");
                    _consoleState.ColPosition = _console.CursorLeft;
                    _consoleState.RowPosition = _console.CursorTop;

                    var input = _consoleKeyHandler.Process(_consoleState);
                    codes.AppendLine(input);
                    _console.WriteLine();

                    var result = await _replEngine.Execute(codes.ToString());

                    switch (result.State)
                    {
                        case OutputState.Continue:
                            _consoleState.Prompt = Prompt.Continue;
                            break;
                        case OutputState.Error:
                            _consoleWriter.WriteError(result.Output ?? "");
                            CompleteInput(codes, _consoleState, result.Variables);
                            break;
                        default:
                            _consoleWriter.WriteOutput(result.Output ?? "");
                            CompleteInput(codes, _consoleState, result.Variables);
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

        private void CompleteInput(StringBuilder codes, IConsoleState state, List<string>? variables)
        {
            state.InputHistory.Add(codes.ToString().TrimEnd());
            codes.Clear();
            state.Variables = variables ?? state.Variables;

            state.Prompt = Prompt.Standard;
            _consoleState.TextRowPosition = 0;

        }
    }
}
