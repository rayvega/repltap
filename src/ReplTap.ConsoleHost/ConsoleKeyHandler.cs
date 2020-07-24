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
            ConsoleKeyInfo input;

            var parameters = new CommandParameters
            {
                Text = text,
                InputHistory = inputHistory,
                Variables = variables,
                Position = Console.CursorLeft,
                Prompt = prompt,
            };

            var inputKeyCommandMap = _consoleKeyCommands.GetInputKeyCommandMap();

            do
            {
                input = _console.ReadKey(intercept: true);

                if (inputKeyCommandMap.TryGetValue((input.Key, input.Modifiers), out var runCommand))
                {
                    runCommand(parameters);
                }
                else
                {
                    text.Append(input.KeyChar);
                    Console.Write(input.KeyChar);
                    parameters.Position++;
                }

            } while (input.Key != ConsoleKey.Enter);

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = text.ToString().TrimEnd();

            return line;
        }
    }
}