namespace ReplTap.ConsoleHost.Commands
{
    public interface IEditCommands
    {
        void WriteChar(ConsoleState state, char inputChar);
        void Backspace(ConsoleState state);
    }

    public class EditCommands : IEditCommands
    {
        private readonly IConsole _console;

        public EditCommands(IConsole console)
        {
            _console = console;
        }

        public void WriteChar(ConsoleState state, char inputChar)
        {
            var endText = state.IsTextEmpty() || state.TextPosition > state.Text.Length
                ? ""
                : state.CurrentLineText[state.TextPosition..];

            _console.Write($"{inputChar.ToString()}{endText}");

            var startText = state.IsTextEmpty()
                ? ""
                : state.CurrentLineText[..state.TextPosition];

            state.CurrentLineText = $"{startText}{inputChar}{endText}";

            _console.CursorLeft = ++state.ColPosition;
        }

        public void Backspace(ConsoleState state)
        {
            if (state.IsStartOfTextPosition() || state.IsTextEmpty())
            {
                return;
            }

            _console.CursorLeft = --state.ColPosition;

            var endText = state.CurrentLineText[(state.ColPosition - 1)..];

            _console.Write($"{endText} ");

            var startText = state.CurrentLineText[..state.TextPosition];

            state.CurrentLineText = $"{startText}{endText}";

            _console.CursorLeft = state.ColPosition;
        }

    }
}
