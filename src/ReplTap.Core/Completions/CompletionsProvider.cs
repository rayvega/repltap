using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReplTap.Core.Completions
{
    public static class CompletionsProvider
    {
        public static async Task<IEnumerable<string>> GetCompletions(string code)
        {
            var results = await RoslynCompletionsProvider.GetCompletions(code);

            var completions = results.Items.Select(item => item.DisplayText);
            
            return completions;
        }
    }
}