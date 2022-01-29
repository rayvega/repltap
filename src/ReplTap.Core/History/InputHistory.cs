using System;
using System.Collections.Generic;

namespace ReplTap.Core.History
{
    public interface IInputHistory
    {
        void Add(string code);

        string GetPreviousInput();

        string GetNextInput();

        string AllInputsAsString();
    }

    public class InputHistory : IInputHistory
    {
        private readonly List<string> _history = new();

        private const int DefaultMinIndex = -1;

        private int _currentPosition = DefaultMinIndex;

        private int MaxIndex()
        {
            return _history.Count - 1;
        }

        private int DefaultMaxIndex()
        {
            return MaxIndex() + 1;
        }

        private bool IsEmpty()
        {
            return _history.Count == 0;
        }

        public void Add(string code)
        {
            _history.Add(code);

            _currentPosition = DefaultMaxIndex();
        }

        public string GetPreviousInput()
        {
            if (IsEmpty() || _currentPosition == DefaultMinIndex)
            {
                return "";
            }

            _currentPosition--;

            if (_currentPosition == DefaultMinIndex)
            {
                return "";
            }

            var input = _history[_currentPosition];

            return input;
        }

        public string GetNextInput()
        {
            if (IsEmpty() || _currentPosition == DefaultMaxIndex())
            {
                return "";
            }

            _currentPosition++;

            if (_currentPosition == DefaultMaxIndex())
            {
                return "";
            }

            var input = _history[_currentPosition];

            return input;
        }

        public string AllInputsAsString()
        {
            return string.Join(Environment.NewLine, _history);
        }
    }
}
