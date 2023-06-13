namespace ReplTap.ConsoleHost.Commands
{
    public interface IEditCommands
    {
        void WriteChar(IConsoleState state, char inputChar);
        void Backspace(IConsoleState state);
    }

    public class EditCommands : IEditCommands
    {
        private readonly IConsole _console;

        public EditCommands(IConsole console)
        {
            _console = console;
        }

        public void WriteChar(IConsoleState state, char inputChar)
        {
            var endText = state.IsTextEmpty() || state.TextColPosition > state.Text.Length
                ? ""
                : state.CurrentLineText[state.TextColPosition..];

            _console.Write($"{inputChar.ToString()}{endText}");

            var startText = state.IsTextEmpty()
                ? ""
                : state.CurrentLineText[..state.TextColPosition];

            state.CurrentLineText = $"{startText}{inputChar}{endText}";

            _console.CursorLeft = ++state.ColPosition;
        }

        public void Backspace(IConsoleState state)
        {
            if (state.IsStartOfTextPosition() || state.IsTextEmpty())
            {
                return;
            }

            _console.CursorLeft = --state.ColPosition;

            var endText = state.CurrentLineText[(state.ColPosition - 1)..];

            _console.Write($"{endText} ");

            var startText = state.CurrentLineText[..state.TextColPosition];

            state.CurrentLineText = $"{startText}{endText}";

            _console.CursorLeft = state.ColPosition;
        }
    }
}
