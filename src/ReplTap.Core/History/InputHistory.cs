using System.Collections.Generic;
using System.Linq;

namespace ReplTap.Core.History
{
    public interface IInputHistory
    {
        string GetPreviousInput();

        void Add(string code);

        void Reset();
    }

    public class InputHistory : IInputHistory
    {
        private readonly List<string> _history = new List<string>();

        public string GetPreviousInput()
        {
            return _history.LastOrDefault();
        }

        public void Add(string code)
        {
            _history.Add(code);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}