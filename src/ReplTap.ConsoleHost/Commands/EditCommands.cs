namespace ReplTap.ConsoleHost.Commands
{
    public interface IEditCommands
    {
        void Backspace(ConsoleState state);
    }

    public class EditCommands : IEditCommands
    {
        private readonly IConsole _console;

        public EditCommands(IConsole console)
        {
            _console = console;
        }

        public void Backspace(ConsoleState state)
        {
            if (state.IsStartOfTextPosition() || state.IsTextEmpty())
            {
                return;
            }

            _console.CursorLeft = --state.LinePosition;

            var endText = state.CurrentLineText[(state.LinePosition - 1)..];

            _console.Write($"{endText} ");

            var startText = state.CurrentLineText[..state.TextPosition];

            state.CurrentLineText = $"{startText}{endText}";

            _console.CursorLeft = state.LinePosition;
        }

    }
}
