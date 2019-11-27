using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplTap.Core.Completions
{
    public interface ICompletionsProvider
    {
        Task<IEnumerable<string>> GetCompletions(string code);
    }

    public class CompletionsProvider : ICompletionsProvider
    {
        private readonly IRoslynCompletionsProvider _roslyn;
        private readonly ICompletionsParser _parser;
        private readonly ICompletionsFilter _filter;

        public CompletionsProvider(IRoslynCompletionsProvider roslyn, ICompletionsParser parser,
            ICompletionsFilter filter)
        {
            _roslyn = roslyn;
            _parser = parser;
            _filter = filter;
        }

        public async Task<IEnumerable<string>> GetCompletions(string code)
        {
            var results = await _roslyn.GetCompletions(code);

            var unfilteredCompletions = results
                .Items
                .Select(item => item.DisplayText);

            var filterText = _parser.ParseIncompleteText(code);
            var completions = _filter.Apply(unfilteredCompletions, filterText);

            return completions;
        }
    }
}