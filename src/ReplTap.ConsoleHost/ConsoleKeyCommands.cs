using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReplTap.ConsoleHost.Extensions;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleKeyCommands
    {
        void WriteChar(ConsoleState state, char inputChar);
        Dictionary<(ConsoleKey, ConsoleModifiers), Action<ConsoleState>> GetInputKeyCommandMap();
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

        public void WriteChar(ConsoleState state, char inputChar)
        {
            var endText = state.IsTextEmpty() || state.TextPosition > state.Text?.Length
                ? ""
                : state.Text?.Slice(state.TextPosition..);

            _console.Write($"{inputChar.ToString()}{endText}");

            var startText = state.IsTextEmpty()
                ? ""
                : state.Text?.Slice(..state.TextPosition);

            state.Text?.ReplaceWith($"{startText}{inputChar}{endText}");

            _console.CursorLeft = ++state.LinePosition;
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
                    (ConsoleKey.Backspace, emptyConsoleModifier), Backspace
                },
                {
                    (ConsoleKey.UpArrow, ConsoleModifiers.Alt), PreviousInput
                },
                {
                    (ConsoleKey.DownArrow, ConsoleModifiers.Alt), NextInput
                },
                {
                    (ConsoleKey.LeftArrow, emptyConsoleModifier), MoveCursorLeft
                },
                {
                    (ConsoleKey.RightArrow, emptyConsoleModifier), MoveCursorRight
                },
            };
        }

        private async Task Completions(ConsoleState state)
        {
            var text = state.Text;
            var inputHistory = state.InputHistory;
            var variables = state.Variables ?? new List<string>();

            var currentCode = text?.ToString();

            var allCode = $"{inputHistory?.AllInputsAsString()}{Environment.NewLine}{currentCode}";

            await _completionsWriter.WriteAllCompletions(allCode, variables);

            WriteFullLine(state.Prompt, currentCode);
        }

        private void Backspace(ConsoleState state)
        {
            if (state.IsStartOfTextPosition() || state.IsTextEmpty() || state.Text == null)
            {
                return;
            }

            _console.CursorLeft = --state.LinePosition;

            var endText = state.Text.Slice((state.LinePosition - 1)..);

            _console.Write($"{endText} ");

            var startText = state.Text.Slice(..state.TextPosition);

            state.Text.ReplaceWith($"{startText}{endText}");

            _console.CursorLeft = state.LinePosition;
        }

        private void NextInput(ConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory?.GetNextInput();

            WriteText(state, input);
        }

        private void PreviousInput(ConsoleState state)
        {
            var inputHistory = state.InputHistory;
            var input = inputHistory?.GetPreviousInput();

            WriteText(state, input);
        }

        private void WriteText(ConsoleState state, string? text)
        {
            state.Text?.ReplaceWith(text);

            var code = state.Text?.ToString() ?? "";
            var position = state.Prompt.Length + code.Length + 1;

            _console.ClearLine();
            WriteFullLine(state.Prompt, code);

            _console.CursorLeft = position;
            state.LinePosition = position;
        }

        private void MoveCursorLeft(ConsoleState state)
        {
            if (state.IsStartOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = --state.LinePosition;
        }

        private void MoveCursorRight(ConsoleState state)
        {
            if (state.IsEndOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = ++state.LinePosition;
        }

        private void WriteFullLine(string prompt, string? code)
        {
            _console.Write($"{prompt} {code}");
        }
    }
}