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
                {
                    (ConsoleKey.LeftArrow, (ConsoleModifiers) 0), MoveCursorLeft
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

            WriteFullLine(parameters.Prompt, currentCode);
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

        private void NextInput(CommandParameters parameters)
        {
            var inputHistory = parameters.InputHistory;
            var input = inputHistory?.GetNextInput();

            WriteInput(parameters, input);
        }

        private void PreviousInput(CommandParameters parameters)
        {
            var inputHistory = parameters.InputHistory;
            var input = inputHistory?.GetPreviousInput();

            WriteInput(parameters, input);
        }

        private void WriteInput(CommandParameters parameters, string? input)
        {
            var text = parameters.Text;

            text?.Clear();
            text?.Append(input);

            var code = text?.ToString() ?? "";
            var position = parameters.Prompt.Length + code.Length + 1;

            _console.ClearLine();
            WriteFullLine(parameters.Prompt, code);
            _console.MoveCursorLeft(position);
            parameters.Position = position;
        }

        private void MoveCursorLeft(CommandParameters parameters)
        {
            if (parameters.Position - parameters.Prompt.Length - 1 <= 0)
            {
                return;
            }

            _console.MoveCursorLeft(--parameters.Position);
        }

        private void WriteFullLine(string prompt, string? code)
        {
            _console.Write($"{prompt} {code}");
        }

    }
}