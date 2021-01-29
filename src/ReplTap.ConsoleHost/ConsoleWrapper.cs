using System;

namespace ReplTap.ConsoleHost
{
    public interface IConsole
    {
        void Write(string? text);
        void WriteLine(string text = "");
        ConsoleKeyInfo ReadKey(bool intercept);
        void ClearLine();
        ConsoleColor ForegroundColor { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        void ResetColor();
        void WriteLine(Exception exception);
    }

    public class ConsoleWrapper : IConsole
    {
        public void Write(string? text) => Console.Write(text);

        public void WriteLine(string text = "")
        {
            if (text == string.Empty)
            {
                Console.WriteLine();

                return;
            }

            Console.WriteLine(text);
        }

        public void WriteLine(Exception exception) => Console.WriteLine(exception);

        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

        public void ClearLine()
        {
            // `SetCursorPosition()` no longer worked properly after migrating to dotnet 3.1
            Console.Write('\r');
            Console.Write(new string(' ', Console.WindowWidth));
            Console.Write('\r');
        }

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }

        public void ResetColor() => Console.ResetColor();
    }
}