using System;
using ReplTap.ConsoleHost.Commands;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleKeyHandler
    {
        string Process(IConsoleState consoleState);
    }

    public class ConsoleKeyHandler : IConsoleKeyHandler
    {
        private readonly IConsole _console;
        private readonly IConsoleKeyCommands _consoleKeyCommands;

        public ConsoleKeyHandler(IConsole console, IConsoleKeyCommands consoleKeyCommands)
        {
            _console = console;
            _consoleKeyCommands = consoleKeyCommands;
        }

        public string Process(IConsoleState state)
        {
            var inputKeyCommandMap = _consoleKeyCommands.GetInputKeyCommandMap();

            while (true)
            {
                var input = _console.ReadKey(intercept: true);

                if (input.Key == ConsoleKey.Enter)
                {
                    // set current row position back to bottom of text area especially if multiline edit
                    _console.CursorTop += state.TextSplitLines.Length;

                    break;
                }

                if (inputKeyCommandMap.TryGetValue((input.Key, input.Modifiers), out var runCommand))
                {
                    runCommand(state);
                }
                else
                {
                    _consoleKeyCommands.WriteChar(state, input.KeyChar);
                }
            }

            var line = state.Text.ToString();

            return line;
        }

    }
}
