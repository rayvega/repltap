using System;
using System.Threading.Tasks;

namespace ReplTap.ConsoleHost.Commands
{
    public interface ICompletionsCommands
    {
        Task Completions(IConsoleState state);
    }

    public class CompletionsCommands : ICompletionsCommands
    {
        private readonly IConsole _console;
        private readonly ICompletionsWriter _completionsWriter;

        public CompletionsCommands(IConsole console, ICompletionsWriter completionsWriter)
        {
            _console = console;
            _completionsWriter = completionsWriter;
        }

        public async Task Completions(IConsoleState state)
        {
            var text = state.Text;
            var inputHistory = state.InputHistory;
            var variables = state.Variables;

            var currentCode = text.ToString();

            var allCode = $"{inputHistory.AllInputsAsString()}{Environment.NewLine}{currentCode}";

            await _completionsWriter.WriteAllCompletions(allCode, variables);

            WriteFullLine(state.Prompt, currentCode);
        }

        private void WriteFullLine(string prompt, string code)
        {
            _console.Write($"{prompt} {code}");
        }
    }
}
