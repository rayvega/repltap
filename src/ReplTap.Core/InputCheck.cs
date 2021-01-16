using static System.Environment;

namespace ReplTap.Core
{
    public interface IInputCheck
    {
        bool IsForceExecute(string input);
    }

    public class InputCheck : IInputCheck
    {
        public bool IsForceExecute(string input)
        {
            var hasMultipleNewlines = input.EndsWith($"{NewLine}{NewLine}{NewLine}");

            var isForceExecute = hasMultipleNewlines;

            return isForceExecute;
        }
    }
}