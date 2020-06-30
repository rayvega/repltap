using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleKeyHandler
    {
        Task<string> Process(string prompt, IInputHistory inputHistory, List<string> variables);
    }

    public class ConsoleKeyHandler : IConsoleKeyHandler
    {
        private readonly IConsole _console;
        private readonly ICompletionsWriter _completionsWriter;

        public ConsoleKeyHandler(IConsole console, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _completionsWriter = completionsWriter;
        }

        public async Task<string> Process(string prompt, IInputHistory inputHistory, List<string> variables)
        {
            var text = new StringBuilder();
            ConsoleKeyInfo input;

            do
            {
                input = _console.ReadKey(intercept: true);

                switch (input.Key)
                {
                    case ConsoleKey.Tab:
                    {
                        var currentCode = text.ToString();

                        var allCode = $"{inputHistory.AllInputsAsString()}{Environment.NewLine}{currentCode}";

                        await _completionsWriter.WriteAllCompletions(allCode, variables);

                        break;
                    }

                    case ConsoleKey.Backspace:
                    {
                        if (text.Length > 0)
                        {
                            text.Length--;
                        }

                        break;
                    }

                    case ConsoleKey.UpArrow:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt))
                        {
                            text.Clear();
                            text.Append(inputHistory.GetPreviousInput());
                        }

                        break;

                    case ConsoleKey.DownArrow:
                        if (input.Modifiers.HasFlag(ConsoleModifiers.Alt))
                        {
                            text.Clear();
                            text.Append(inputHistory.GetNextInput());
                        }

                        break;

                    default:
                        text.Append(input.KeyChar);

                        break;
                }

                _console.ClearLine();
                _console.Write($"{prompt} {text}");

            } while (input.Key != ConsoleKey.Enter);

            // remove newline(s) from end to avoid cursor moving to start of line after navigating input history
            var line = text.ToString().TrimEnd();

            return line;
        }
    }
}