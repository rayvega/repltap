using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost
{
    public interface ICompletionsWriter
    {
        Task WriteAllCompletions(string code, List<string> variables);
    }

    public class CompletionsWriter : ICompletionsWriter
    {
        private readonly IConsole _console;
        private readonly ICompletionsProvider _completionsProvider;

        public CompletionsWriter(ICompletionsProvider completionsProvider, IConsole console)
        {
            _completionsProvider = completionsProvider;
            _console = console;
        }

        public async Task WriteAllCompletions(string code, List<string> variables)
        {
            var completions = await _completionsProvider.GetCompletions(code);

            var allCompletions = variables.Union(completions);

            _console.WriteLine();

            foreach (var completion in allCompletions)
            {
                _console.WriteLine(completion);
            }
        }
    }
}