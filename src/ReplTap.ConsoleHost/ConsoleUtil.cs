using System;
using System.Text;

namespace ReplTap.ConsoleHost
{
    public class ConsoleUtil
    {
        public static string ReadLine(string prompt)
        {
            var buffer = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Backspace)
                {
                    buffer.Length--;
                }
                else
                {
                    buffer.Append(input.KeyChar);
                }

                ClearLine();
                Console.Write($"{prompt} {buffer}");

            } while (input.Key != ConsoleKey.Enter);

            var line = buffer.ToString();

            return line;
        }

        private static void ClearLine()
        {
            int cursor = Console.CursorTop;
            Console.SetCursorPosition(0, cursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, cursor);
        }
    }
}