using System;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleWriter
    {
        void WriteOutput(string text);
        void WriteError(string text);
        void WriteError(Exception exception);
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly IConsole _console;

        public ConsoleWriter(IConsole console)
        {
            _console = console;
        }

        public void WriteOutput(string text)
        {
            _console.ForegroundColor = ConsoleColor.Gray;
            _console.WriteLine(text);
            _console.ResetColor();
        }

        public void WriteError(string text)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine(text);
            _console.ResetColor();
        }

        public void WriteError(Exception exception)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.WriteLine(exception);
            _console.ResetColor();
        }
    }
}