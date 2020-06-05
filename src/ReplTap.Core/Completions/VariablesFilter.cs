using System.Collections.Generic;

namespace ReplTap.Core.Completions
{
    public interface IVariablesFilter
    {
        IEnumerable<string> Filter(string code, List<string> variables);
    }

    public class VariablesFilter : IVariablesFilter
    {
        private readonly ICompletionsParser _parser;
        private readonly ICompletionsFilter _filter;

        public VariablesFilter(ICompletionsParser parser, ICompletionsFilter filter)
        {
            _parser = parser;
            _filter = filter;
        }

        public IEnumerable<string> Filter(string code, List<string> variables)
        {
            // parse last token of code then filter variables by matching against that token
            var lastToken = _parser.ParseLastToken(code);
            var filterVariables = _filter.Apply(variables, lastToken);

            return filterVariables;
        }
    }
}