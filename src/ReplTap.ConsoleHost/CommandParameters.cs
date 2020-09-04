using System.Collections.Generic;
using System.Text;
using ReplTap.Core.History;
using PromptValues = ReplTap.ConsoleHost.Prompt;

namespace ReplTap.ConsoleHost
{
    public class CommandParameters
    {
        public CommandParameters()
        {
            Text = new StringBuilder();
            InputHistory = new InputHistory();
            Variables = new List<string>();
            Prompt = PromptValues.Standard;
        }

        public StringBuilder? Text { get; set; }
        public IInputHistory? InputHistory { get; set; }
        public List<string>? Variables { get; set; }
        public int LinePosition { get; set; }
        public int TextPosition => LinePosition - $"{Prompt} ".Length;

        public bool IsStartOfTextPosition()
        {
            return TextPosition <= 0;
        }

        public string Prompt { get; internal set; }
    }
}