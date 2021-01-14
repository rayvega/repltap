using System;
using System.Collections.Generic;
using System.Text;
using ReplTap.Core.History;
using PromptValues = ReplTap.ConsoleHost.Prompt;

namespace ReplTap.ConsoleHost
{
    public class ConsoleState
    {
        public ConsoleState()
        {
            Prompt = PromptValues.Standard;
            Text = new StringBuilder();
            InputHistory = new InputHistory();
            Variables = new List<string>();
        }

        public string Prompt { get; internal init; }

        public StringBuilder? Text { get; init; }

        public string[] TextSplitLines
        {
            get
            {
                var code = Text?.ToString() ?? "";
                var lines = code.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

                return lines;
            }
        }

        public int LinePosition { get; set; }

        public int TextPosition => LinePosition - $"{Prompt} ".Length;

        public bool IsTextEmpty() => Text?.Length <= 0;

        public bool IsStartOfTextPosition() => TextPosition <= 0;

        public bool IsEndOfTextPosition() => TextPosition >= Text?.Length;

        public IInputHistory? InputHistory { get; init; }

        public List<string>? Variables { get; init; }
    }
}