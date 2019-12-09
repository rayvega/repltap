namespace ReplTap.Core.History
{
    public interface IInputHistory
    {
        string GetPreviousInput();

        void Add(string code);

        void Reset();
    }
}