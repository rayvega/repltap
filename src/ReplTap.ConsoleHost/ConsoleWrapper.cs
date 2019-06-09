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

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
           return Console.ReadKey(intercept);
        }

        public void ClearLine()
        {
            var cursor = Console.CursorTop;
            Console.SetCursorPosition(0, cursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, cursor);
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
    }
}