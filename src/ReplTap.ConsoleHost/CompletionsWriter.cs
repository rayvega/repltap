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
        private readonly IVariablesFilter _variablesFilter;

        public CompletionsWriter(ICompletionsProvider completionsProvider, IConsole console, IVariablesFilter variablesFilter)
        {
            _completionsProvider = completionsProvider;
            _console = console;
            _variablesFilter = variablesFilter;
        }

        public async Task WriteAllCompletions(string code, List<string> variables)
        {
            var completions = await _completionsProvider.GetCompletions(code);
            var filteredVariables = _variablesFilter.Filter(code, variables);

            var allCompletions = filteredVariables.Union(completions);

            _console.WriteLine();

            foreach (var completion in allCompletions)
            {
                _console.WriteLine(completion);
            }
        }
    }
}