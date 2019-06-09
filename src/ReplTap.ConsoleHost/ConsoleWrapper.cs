using System;

namespace ReplTap.ConsoleHost
{
    public interface IConsole
    {
        void Write(string text);
        void WriteLine(string text);
        ConsoleColor ForegroundColor { get; set; }
        void ResetColor();
    }
    
    public class ConsoleWrapper : IConsole
    {
        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
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