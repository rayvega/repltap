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
            return input?.Contains(";") ?? false;
        }
    }
}