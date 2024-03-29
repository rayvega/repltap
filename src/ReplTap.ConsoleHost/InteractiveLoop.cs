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
            while (_loop.Continue())
            {
                try
                {
                    _console.Write($"{_consoleState.Prompt} ");
                    _consoleState.ColPosition = _console.CursorLeft;
                    _consoleState.RowPosition = _console.CursorTop;

                    var input = _consoleKeyHandler.Process(_consoleState);
                    _console.WriteLine();

                    var result = await _replEngine.Execute(input);

                    switch (result.State)
                    {
                        case OutputState.Continue:
                            _consoleState.Prompt = Prompt.Continue;
                            _consoleState.TextRowPosition++;
                            break;
                        case OutputState.Error:
                            _consoleWriter.WriteError(result.Output);
                            _consoleState.CompleteInput(result.Variables);
                            break;
                        default:
                            _consoleWriter.WriteOutput(result.Output);
                            _consoleState.CompleteInput(result.Variables);
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
    }
}
