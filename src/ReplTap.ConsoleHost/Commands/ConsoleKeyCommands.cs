using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReplTap.ConsoleHost.Extensions;

namespace ReplTap.ConsoleHost.Commands
{
    public interface IConsoleKeyCommands
    {
        void WriteChar(ConsoleState state, char inputChar);
        Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>> GetInputKeyCommandMap();
    }

    public class ConsoleKeyCommands : IConsoleKeyCommands
    {
        private readonly IConsole _console;
        private readonly INavigateCommands _navigateCommands;
        private readonly IEditCommands _editCommands;
        private readonly ICompletionsWriter _completionsWriter;

        public ConsoleKeyCommands(IConsole console, INavigateCommands navigateCommands,
            IEditCommands editCommands, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _navigateCommands = navigateCommands;
            _completionsWriter = completionsWriter;
            _editCommands = editCommands;
        }

        public void WriteChar(ConsoleState state, char inputChar)
        {
            _editCommands.WriteChar(state, inputChar);
        }

        public Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>> GetInputKeyCommandMap()
        {
            var emptyConsoleModifier = (ConsoleModifiers) 0;

            return new Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>>
            {
                {
                    (ConsoleKey.Tab, emptyConsoleModifier),
                    async state => await Completions(state)
                },
                {
                    (ConsoleKey.Backspace, emptyConsoleModifier), _editCommands.Backspace
                },
                {
                    (ConsoleKey.UpArrow, ConsoleModifiers.Alt), PreviousInput
                },
                {
                    (ConsoleKey.DownArrow, ConsoleModifiers.Alt), NextInput
                },
                {
                    (ConsoleKey.LeftArrow, emptyConsoleModifier), _navigateCommands.MoveCursorLeft
                },
                {
                    (ConsoleKey.RightArrow, emptyConsoleModifier), _navigateCommands.MoveCursorRight
                },
            };
        }

        private async Task Completions(ConsoleState state)
        {
            var text = state.Text;
            var inputHistory = state.InputHistory;
            var variables = state.Variables;

            var currentCode = text.ToString();

            var allCode = $"{inputHistory.AllInputsAsString()}{Environment.NewLine}{currentCode}";

            await _completionsWriter.WriteAllCompletions(allCode, variables);

            WriteFullLine(state.Prompt, currentCode);
        }

        private void NextInput(ConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory.GetNextInput();

            WriteText(state, input);
        }

        private void PreviousInput(ConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory.GetPreviousInput();

            WriteText(state, input);
        }

        private void WriteText(ConsoleState state, string? text)
        {
            // remove old lines
            var oldCodeLines = state.TextSplitLines;

            for (var index = oldCodeLines.Length - 1; index >= 0; index--)
            {
                _console.ClearLine();

                // if a single or last line of code do not move up in console
                if (index == 0)
                {
                    break;
                }

                _console.CursorTop -= 1;
            }

            // add new lines
            state.Text.ReplaceWith(text);

            var codeLines = state.TextSplitLines;
            var lastLine = "";

            for (var index = 0; index < codeLines.Length; index++)
            {
                var line = codeLines[index];

                var prompt = index == 0
                    ? state.Prompt
                    : Prompt.Continue;

                // if not the last line keep adding newline before writing to console
                var endOfLine = index == codeLines.Length - 1
                    ? ""
                    : Environment.NewLine;

                WriteFullLine(prompt, $"{line}{endOfLine}");

                lastLine = line;
            }

            var position = state.Prompt.Length + lastLine.Length + 1;

            _console.CursorLeft = position;
            state.LinePosition = position;
        }

        private void WriteFullLine(string prompt, string? code)
        {
            _console.Write($"{prompt} {code}");
        }
    }
}
