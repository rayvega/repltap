using System.Collections.Generic;

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
        private int _currentPosition = -1;

        public string GetPreviousInput()
        {
            if (_currentPosition < 0)
            {
                return string.Empty;
            }

            var input = _history[_currentPosition];

            _currentPosition--;

            return input;
        }

        public void Add(string code)
        {
            _currentPosition++;

            _history.Add(code);
        }

        public void Reset()
        {
            _currentPosition = _history.Count <= 0
                ? -1
                : _history.Count - 1;
        }
    }
}
