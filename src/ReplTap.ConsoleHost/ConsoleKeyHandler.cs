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

            var parameters = new CommandParameters
            {
                Text = text,
                InputHistory = inputHistory,
                Variables = variables,
                Position = _console.CursorLeft,
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
                    runCommand(parameters);
                }
                else
                {
                    WriteText(parameters, input);
                }
            }

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = parameters.Text.ToString().TrimEnd();

            return line;
        }

        private void WriteText(CommandParameters parameters, ConsoleKeyInfo input)
        {
            var endText = parameters.Text?.Length < 0 || parameters.TextPosition > parameters.Text?.Length
                ? ""
                : parameters.Text?.ToString().Substring(parameters.TextPosition);

            _console.Write(input.KeyChar.ToString());
            _console.Write(endText);

            var startText = parameters.Text?.Length <= 0
                ? ""
                : parameters.Text?.ToString().Substring(0, parameters.TextPosition);

            parameters.Text?.Clear();
            parameters.Text?.Append($"{startText}{input.KeyChar}{endText}");
            parameters.Position++;
            _console.CursorLeft = parameters.Position;
        }
    }
}