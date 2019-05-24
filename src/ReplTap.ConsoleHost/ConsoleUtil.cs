using System;
using System.Text;

namespace ReplTap.ConsoleHost
{
    public class ConsoleUtil
    {
        public static string ReadLine()
        {
            var buffer = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = Console.ReadKey();

                if (input.Key == ConsoleKey.Backspace)
                {
                    Console.WriteLine("\nTODO: implement backspace");
                }
                else
                {
                    buffer.Append(input.KeyChar);
                }
            } while (input.Key != ConsoleKey.Enter);

            var line = buffer.ToString();

            return line;
        }
    }
}