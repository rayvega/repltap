using System;
using System.Collections.Generic;
using System.Text;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleKeyHandler
    {
        string Process(string prompt, IInputHistory inputHistory, List<string> variables);
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

        public string Process(string prompt, IInputHistory inputHistory, List<string> variables)
        {
            var text = new StringBuilder();

            var state = new ConsoleState
            {
                Text = text,
                InputHistory = inputHistory,
                Variables = variables,
                LinePosition = _console.CursorLeft,
                Prompt = prompt,
            };

            var inputKeyCommandMap = _consoleKeyCommands.GetInputKeyCommandMap();

            while (true)
            {
                var input = _console.ReadKey(intercept: true);

                if (input.Key == ConsoleKey.Enter)
                {
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

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = state.Text.ToString().TrimEnd();

            return line;
        }

    }
}