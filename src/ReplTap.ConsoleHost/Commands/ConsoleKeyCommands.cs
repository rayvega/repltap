using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly IInputHistoryCommands _inputHistoryCommands;

        public ConsoleKeyCommands(IConsole console, INavigateCommands navigateCommands,
            IEditCommands editCommands, ICompletionsWriter completionsWriter,
            IInputHistoryCommands inputHistoryCommands)
        {
            _console = console;
            _navigateCommands = navigateCommands;
            _completionsWriter = completionsWriter;
            _inputHistoryCommands = inputHistoryCommands;
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
                    (ConsoleKey.UpArrow, ConsoleModifiers.Alt), state => _inputHistoryCommands.PreviousInput(state)
                },
                {
                    (ConsoleKey.DownArrow, ConsoleModifiers.Alt), state => _inputHistoryCommands.NextInput(state)
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

        private void WriteFullLine(string prompt, string? code)
        {
            _console.Write($"{prompt} {code}");
        }
    }
}
