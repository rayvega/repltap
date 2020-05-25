using System.Linq;
using System.Text;

namespace ReplTap.Core.Completions
{
    public interface ICompletionsParser
    {
        string ParseLastToken(string code);
    }

    public class CompletionsParser : ICompletionsParser
    {
        public string ParseLastToken(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(code.Length);

            for (var index = code.Length - 1; index >= 0; index--)
            {
                var current = code[index];

                if (!char.IsLetterOrDigit(current))
                {
                    break;
                }

                builder.Append(current);
            }

            var lastToken = new string(builder.ToString().Reverse().ToArray());

            return lastToken;
        }
    }
}