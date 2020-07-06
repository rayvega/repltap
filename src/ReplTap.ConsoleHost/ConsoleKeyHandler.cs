using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ICompletionsWriter _completionsWriter;

        public ConsoleKeyHandler(IConsole console, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _completionsWriter = completionsWriter;
        }

        private class CommandParameters
        {
            public CommandParameters()
            {
                Text = new StringBuilder();
                InputHistory = new InputHistory();
                Variables = new List<string>();
            }

            public StringBuilder? Text { get; set; }
            public IInputHistory? InputHistory { get; set; }
            public List<string>? Variables { get; set; }
        }

        public string Process(string prompt, IInputHistory inputHistory, List<string> variables)
        {
            var text = new StringBuilder();
            ConsoleKeyInfo input;

            var parameters = new CommandParameters
            {
                Text = text,
                InputHistory = inputHistory,
                Variables = variables
            };

            var inputKeyCommandMap = GetInputKeyCommandMap();

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
                }

                _console.ClearLine();
                _console.Write($"{prompt} {text}");

            } while (input.Key != ConsoleKey.Enter);

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = text.ToString().TrimEnd();

            return line;
        }

        private Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>> GetInputKeyCommandMap()
        {
            return new Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>>
            {
                {
                    (ConsoleKey.Tab, (ConsoleModifiers) 0),
                    async parameters => await Completions(parameters)
                },
                {
                    (ConsoleKey.Backspace, (ConsoleModifiers) 0), DeleteCharBackward
                },
                {
                    (ConsoleKey.UpArrow, ConsoleModifiers.Alt), PreviousInput
                },
                {
                    (ConsoleKey.DownArrow, ConsoleModifiers.Alt), NextInput
                },
            };
        }

        private async Task Completions(CommandParameters parameters)
        {
            var text = parameters.Text;
            var inputHistory = parameters.InputHistory;
            var variables = parameters.Variables ?? new List<string>();

            var currentCode = text?.ToString();

            var allCode = $"{inputHistory?.AllInputsAsString()}{Environment.NewLine}{currentCode}";

            await _completionsWriter.WriteAllCompletions(allCode, variables);
        }

        private static void DeleteCharBackward(CommandParameters parameters)
        {
            if (parameters.Text?.Length > 0)
            {
                parameters.Text.Length--;
            }
        }

        private static void NextInput(CommandParameters parameters)
        {
            var inputHistory = parameters.InputHistory;
            var text = parameters.Text;

            text?.Clear();
            text?.Append(inputHistory?.GetNextInput());
        }

        private static void PreviousInput(CommandParameters parameters)
        {
            var inputHistory = parameters.InputHistory;
            var text = parameters.Text;

            text?.Clear();
            text?.Append(inputHistory?.GetPreviousInput());
        }
    }
}