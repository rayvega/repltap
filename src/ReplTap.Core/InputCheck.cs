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
            var hasMultipleNewlines = input.EndsWith("\r\r\r");

            var isForceExecute = hasMultipleNewlines;

            return isForceExecute;
        }
    }
}