using System.Collections.Generic;
using System.Linq;

namespace ReplTap.Core
{
    public static class CompletionsProvider
    {
        public static IEnumerable<string> GetCompletions(string code)
        {
            var completions = Enumerable
                .Range(1, 3)
                .Select(i => $"test suggestion {i}");
            
            return completions;
        }
    }
}