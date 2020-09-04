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
                    WriteText(state, input);
                }
            }

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = state.Text.ToString().TrimEnd();

            return line;
        }

        private void WriteText(ConsoleState state, ConsoleKeyInfo input)
        {
            var endText = state.Text?.Length < 0 || state.TextPosition > state.Text?.Length
                ? ""
                : state.Text?.ToString().Substring(state.TextPosition);

            _console.Write(input.KeyChar.ToString());
            _console.Write(endText);

            var startText = state.Text?.Length <= 0
                ? ""
                : state.Text?.ToString().Substring(0, state.TextPosition);

            state.Text?.Clear();
            state.Text?.Append($"{startText}{input.KeyChar}{endText}");
            state.LinePosition++;
            _console.CursorLeft = state.LinePosition;
        }
    }
}