using System;
using System.Text;
using System.Threading.Tasks;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost
{
    public static class ConsoleUtil
    {
        public static async Task<string> ReadLine(string prompt)
        {
            var buffer = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = Console.ReadKey(true);

                switch (input.Key)
                {
                    case ConsoleKey.Tab:
                    {
                        var currentCode = buffer.ToString();
                        await WriteAllCompletions(currentCode);
                        
                        break;
                    }
                    
                    case ConsoleKey.Backspace:
                    {
                        if (buffer.Length > 0)
                        {
                            buffer.Length--;
                        }

                        break;
                    }

                    default:
                        buffer.Append(input.KeyChar);
                        break;
                }

                ClearLine();
                Console.Write($"{prompt} {buffer}");

            } while (input.Key != ConsoleKey.Enter);

            var line = buffer.ToString();

            return line;
        }

        private static async Task WriteAllCompletions(string code)
        {
            var completions = await CompletionsProvider.GetCompletions(code);

            Console.WriteLine();
            
            foreach (var completion in completions)
            {
                Console.WriteLine(completion);
            }
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