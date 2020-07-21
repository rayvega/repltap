using System;

namespace ReplTap.ConsoleHost
{
    public interface IConsole
    {
        void Write(string text);
        void WriteLine(string text = "");
        ConsoleKeyInfo ReadKey(bool intercept);
        void ClearLine();
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
        void WriteLine(Exception exception);
        void MoveCursorLeft(int position);
    }

    public class ConsoleWrapper : IConsole
    {
        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text = "")
        {
            if (text == string.Empty)
            {
                Console.WriteLine();

                return;
            }

            Console.WriteLine(text);
        }

        public void WriteLine(Exception exception)
        {
            Console.WriteLine(exception);
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }

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

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void MoveCursorLeft(int position)
        {
            Console.CursorLeft = position;
        }
    }
}