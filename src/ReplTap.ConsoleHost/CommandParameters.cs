using System.Collections.Generic;
using System.Text;
using ReplTap.Core.History;

namespace ReplTap.ConsoleHost
{
    public class CommandParameters
    {
        public CommandParameters()
        {
            Text = new StringBuilder();
            InputHistory = new InputHistory();
            Variables = new List<string>();
        }

        public StringBuilder? Text { get; set; }
        public IInputHistory? InputHistory { get; set; }
        public List<string>? Variables { get; set; }
    }
}