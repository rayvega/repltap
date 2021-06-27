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
        int TextRowPosition { get; set; }
        bool IsTextEmpty();
        bool IsStartOfTextPosition();
        bool IsEndOfTextPosition();
        bool IsStartOfRowTextPosition();
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
            get
            {
                var lines = TextSplitLines;
                var row = TextRowPosition;

                var text = lines.Length < 1 || lines.Length <= row
                    ? ""
                    : lines[row];

                return text;
            }
            set
            {
                var lines = TextSplitLines.ToList();
                var row = TextRowPosition;

                if (lines.Count < 1 || lines.Count <= row)
                {
                    lines.Add("");
                }

                lines[row] = value;

                Text.Clear();
                var newText = lines.Aggregate((lineA, lineB) => $"{lineA}{NewLine}{lineB}");
                Text.Append(newText);
            }
        }

        public int ColPosition { get; set; }

        public int TextColPosition => ColPosition - $"{Prompt} ".Length;

        public int TextRowPosition { get; set; }

        public int RowPosition { get; set; }

        public bool IsTextEmpty() => Text.Length <= 0;

        public bool IsStartOfTextPosition() => TextColPosition <= 0;

        public bool IsEndOfTextPosition() => TextColPosition >= CurrentLineText.Length;

        public bool IsStartOfRowTextPosition() => TextRowPosition <= 0;

        public IInputHistory InputHistory { get; init; }

        public List<string> Variables { get; set; }
    }
}
