using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleKeyCommands
    {
        Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>> GetInputKeyCommandMap();
    }

    public class ConsoleKeyCommands : IConsoleKeyCommands
    {
        private readonly IConsole _console;
        private readonly ICompletionsWriter _completionsWriter;

        public ConsoleKeyCommands(IConsole console, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _completionsWriter = completionsWriter;
        }

        public Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>> GetInputKeyCommandMap()
        {
            return new Dictionary<(ConsoleKey, ConsoleModifiers), Action<CommandParameters>>
            {
                {
                    (ConsoleKey.Tab, (ConsoleModifiers) 0),
                    async parameters => await Completions(parameters)
                },
                {
                    (ConsoleKey.Backspace, (ConsoleModifiers) 0), Backspace
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

            _console.Write($"{parameters.Prompt} {currentCode}");
        }

        private void Backspace(CommandParameters parameters)
        {
            if (!(parameters.Text?.Length > 0))
            {
                return;
            }

            parameters.Text.Length--;

            _console.MoveCursorLeft(--parameters.Position);
            _console.Write(" ");
            _console.MoveCursorLeft(parameters.Position);
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