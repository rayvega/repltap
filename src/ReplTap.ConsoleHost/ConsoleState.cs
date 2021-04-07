using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReplTap.Core.History;
using static System.Environment;
using PromptValues = ReplTap.ConsoleHost.Prompt;

namespace ReplTap.ConsoleHost
{
    public interface IConsoleState
    {
        string Prompt { get; set; }
        StringBuilder Text { get; init; }
        string[] TextSplitLines { get; }
        string CurrentLineText { get; set; }
        int ColPosition { get; set; }
        int RowPosition { get; set; }
        int TextColPosition { get; }
        IInputHistory InputHistory { get; init; }
        List<string> Variables { get; set; }
        bool IsTextEmpty();
        bool IsStartOfTextPosition();
        bool IsEndOfTextPosition();
    }

    public class ConsoleState : IConsoleState
    {
        public ConsoleState(IInputHistory inputHistory)
        {
            Prompt = PromptValues.Standard;
            Text = new StringBuilder();
            InputHistory = inputHistory;
            Variables = new List<string>();
        }

        public string Prompt { get; set; }

        public StringBuilder Text { get; init; }

        public string[] TextSplitLines
        {
            get
            {
                var code = Text.ToString();
                var lines = code.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

                return lines;
            }
        }

        public string CurrentLineText
        {
            // note: current line text is the last line but this will change in the future
            get => TextSplitLines.LastOrDefault() ?? "";
            set
            {
                var lastLineIndex = Text
                    .ToString()
                    .LastIndexOf(NewLine, StringComparison.Ordinal);

                var text = lastLineIndex < 0
                    ? ""
                    : $"{Text.ToString()[..lastLineIndex]}{NewLine}";

                Text.Clear();
                Text.Append($"{text}{value}");
            }
        }

        public int ColPosition { get; set; }

        public int TextColPosition => ColPosition - $"{Prompt} ".Length;

        public int RowPosition { get; set; }

        public bool IsTextEmpty() => Text.Length <= 0;

        public bool IsStartOfTextPosition() => TextColPosition <= 0;

        public bool IsEndOfTextPosition() => TextColPosition >= CurrentLineText.Length;

        public IInputHistory InputHistory { get; init; }

        public List<string> Variables { get; set; }
    }
}
