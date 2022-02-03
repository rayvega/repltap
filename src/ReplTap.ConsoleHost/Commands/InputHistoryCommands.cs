using System;
using ReplTap.ConsoleHost.Extensions;

namespace ReplTap.ConsoleHost.Commands
{
    public interface IInputHistoryCommands
    {
        void NextInput(IConsoleState state);
        void PreviousInput(IConsoleState state);
    }

    public class InputHistoryCommands : IInputHistoryCommands
    {
        private readonly IConsole _console;

        public InputHistoryCommands(IConsole console)
        {
            _console = console;
        }

        public void NextInput(IConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory.GetNextInput();

            WriteText(state, input);
        }

        public void PreviousInput(IConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory.GetPreviousInput();

            WriteText(state, input);
        }

        private void WriteText(IConsoleState state, string text)
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

                var code = $"{line}{endOfLine}";
                _console.Write($"{prompt} {code}");

                lastLine = line;
            }

            var position = state.Prompt.Length + lastLine.Length + 1;

            _console.CursorLeft = position;
            state.ColPosition = position;
        }
    }
}
