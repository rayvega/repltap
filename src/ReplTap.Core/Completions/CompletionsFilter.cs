using System.Collections.Generic;
using System.Linq;

namespace ReplTap.Core.Completions
{
    public interface ICompletionsFilter
    {
        IEnumerable<string> Apply(IEnumerable<string> completions, string filterText);
    }

    public class CompletionsFilter : ICompletionsFilter
    {
        public IEnumerable<string> Apply(IEnumerable<string> completions, string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                return completions;
            }

            var filteredCompletions = completions
                .Where(item => item.StartsWith(filterText));

            return filteredCompletions;
        }
    }
}