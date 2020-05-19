namespace ReplTap.ConsoleHost
{
    public interface ILoop
    {
        bool Continue();
    }

    public class Loop : ILoop
    {
        public bool Continue()
        {
            return true;
        }
    }
}