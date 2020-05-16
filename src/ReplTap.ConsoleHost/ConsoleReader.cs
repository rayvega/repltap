using System;
using System.Text;
using System.Threading.Tasks;
using ReplTap.Core.Completions;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleReader
    {
        Task<string> ReadLine(string prompt, IInputHistory inputHistory);
    }

    public class ConsoleReader : IConsoleReader
    {
        private readonly IConsole _console;
        private readonly ICompletionsProvider _completionsProvider;

        public ConsoleReader(IConsole console, ICompletionsProvider completionsProvider)
        {
            _console = console;
            _completionsProvider = completionsProvider;
        }

        public async Task<string> ReadLine(string prompt, IInputHistory inputHistory)
        {
            var buffer = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = _console.ReadKey(intercept: true);

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

                    case ConsoleKey.UpArrow:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt))
                        {
                            buffer.Clear();
                            buffer.Append(inputHistory.GetPreviousInput());
                        }

                        break;

                    case ConsoleKey.DownArrow:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt))
                        {
                            buffer.Clear();
                            buffer.Append(inputHistory.GetNextInput());
                        }

                        break;

                    default:
                        buffer.Append(input.KeyChar);

                        break;
                }

                _console.ClearLine();
                _console.Write($"{prompt} {buffer}");

            } while (input.Key != ConsoleKey.Enter);

            var line = buffer.ToString().TrimEnd();

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
