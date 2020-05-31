using System.Collections.Generic;

namespace ReplTap.Core.Completions
{
    public interface IVariablesFilter
    {
        IEnumerable<string> Filter(string code, List<string> variables);
    }
}