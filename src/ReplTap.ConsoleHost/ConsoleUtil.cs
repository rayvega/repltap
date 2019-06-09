using System;
using System.Text;
using System.Threading.Tasks;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleUtil
    {
        Task<string> ReadLine(string prompt);
    }

    public class ConsoleUtil : IConsoleUtil
    {
        private readonly IConsole _console;
        private readonly ICompletionsProvider _completionsProvider;

        public ConsoleUtil(IConsole console, ICompletionsProvider completionsProvider)
        {
            _console = console;
            _completionsProvider = completionsProvider;
        }

        public async Task<string> ReadLine(string prompt)
        {
            var buffer = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = _console.ReadKey(true);

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

                _console.ClearLine();
                _console.Write($"{prompt} {buffer}");

            } while (input.Key != ConsoleKey.Enter);

            var line = buffer.ToString();

            return line;
        }

        private async Task WriteAllCompletions(string code)
        {
            var completions = await _completionsProvider.GetCompletions(code);

            _console.WriteLine();
            
            foreach (var completion in completions)
            {
                _console.WriteLine(completion);
            }
        }
    }
}