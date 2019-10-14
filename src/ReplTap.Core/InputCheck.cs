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
            var hasSemicolon = input?.Contains(";") ?? false;
            var hasMultipleNewlines = input?.EndsWith("\r\r\r") ?? false;

            var isForceExecute = hasSemicolon || hasMultipleNewlines;

            return isForceExecute;
        }
    }
}