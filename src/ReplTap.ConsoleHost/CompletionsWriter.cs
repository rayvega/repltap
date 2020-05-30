using System.Threading.Tasks;
using ReplTap.Core.Completions;

namespace ReplTap.ConsoleHost
{
    public interface ICompletionsWriter
    {
        Task WriteAllCompletions(string code);
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

        public async Task WriteAllCompletions(string code)
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