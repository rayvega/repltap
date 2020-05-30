using System;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ICompletionsWriter _completionsWriter;

        public ConsoleReader(IConsole console, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _completionsWriter = completionsWriter;
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

                        await _completionsWriter.WriteAllCompletions(currentCode);

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

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = buffer.ToString().TrimEnd();

            return line;
        }
    }
}
