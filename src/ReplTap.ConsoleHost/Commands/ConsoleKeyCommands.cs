using System;
using System.Collections.Generic;

namespace ReplTap.ConsoleHost.Commands
{
    public interface IConsoleKeyCommands
    {
        void WriteChar(IConsoleState state, char inputChar);
        Dictionary<(ConsoleKey, ConsoleModifiers), Action<IConsoleState>> GetInputKeyCommandMap();
    }

    public class ConsoleKeyCommands : IConsoleKeyCommands
    {
        private readonly INavigateCommands _navigateCommands;
        private readonly IEditCommands _editCommands;
        private readonly ICompletionsCommands _completionsCommands;
        private readonly IInputHistoryCommands _inputHistoryCommands;

        public ConsoleKeyCommands(INavigateCommands navigateCommands,
            IEditCommands editCommands, ICompletionsCommands completionsCommands,
            IInputHistoryCommands inputHistoryCommands)
        {
            _navigateCommands = navigateCommands;
            _inputHistoryCommands = inputHistoryCommands;
            _editCommands = editCommands;
            _completionsCommands = completionsCommands;
        }

        public void WriteChar(IConsoleState state, char inputChar)
        {
            _editCommands.WriteChar(state, inputChar);
        }

        public Dictionary<(ConsoleKey, ConsoleModifiers), Action<IConsoleState>> GetInputKeyCommandMap()
        {
            var emptyConsoleModifier = (ConsoleModifiers) 0;

            return new Dictionary<(ConsoleKey, ConsoleModifiers), Action<IConsoleState>>
            {
                {
                    (ConsoleKey.Tab, emptyConsoleModifier),
                    async state => await _completionsCommands.Completions(state)
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

    }
}
