namespace ReplTap.ConsoleHost.Commands
{
    public interface INavigateCommands
    {
        void MoveCursorLeft(ConsoleState state);

        void MoveCursorRight(ConsoleState state);
    }

    public class NavigateCommands : INavigateCommands
    {
        private readonly IConsole _console;

        public NavigateCommands(IConsole console)
        {
            _console = console;
        }

        public void MoveCursorLeft(ConsoleState state)
        {
            if (state.IsStartOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = --state.LinePosition;
        }

        public void MoveCursorRight(ConsoleState state)
        {
            if (state.IsEndOfTextPosition())
            {
                return;
            }

            _console.CursorLeft = ++state.LinePosition;
        }
    }
}
