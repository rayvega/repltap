namespace ReplTap.ConsoleHost.Commands
{
    public interface INavigateCommands
    {
        void MoveCursorLeft(IConsoleState state);

        void MoveCursorRight(IConsoleState state);

        void MoveCursorUp(IConsoleState state);
    }

    public class NavigateCommands : INavigateCommands
    {
        private readonly IConsole _console;

        public NavigateCommands(IConsole console)
        {
            _console = console;
        }

        public void MoveCursorLeft(IConsoleState state)
        {
            if (state.IsStartOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = --state.ColPosition;
        }

        public void MoveCursorRight(IConsoleState state)
        {
            if (state.IsEndOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = ++state.ColPosition;
        }

        public void MoveCursorUp(IConsoleState state)
        {
            if (state.IsStartOfRowTextPosition())
            {
                return;
            }

            --state.TextRowPosition;
            _console.CursorTop = --state.RowPosition;
        }
    }
}
