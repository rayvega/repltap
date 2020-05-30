using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Completion;

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
            if (string.IsNullOrEmpty(code))
            {
                return Enumerable.Empty<string>();
            }

            var results = await _roslyn.GetCompletions(code);

            if (results == CompletionList.Empty)
            {
                return Enumerable.Empty<string>();
            }

            var unfilteredCompletions = results
                .Items
                .Select(item => item.DisplayText);

            // parse last token of code then filter completions by matching against that token
            var lastToken = _parser.ParseLastToken(code);
            var completions = _filter.Apply(unfilteredCompletions, lastToken);

            return completions;
        }
    }
}