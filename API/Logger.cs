namespace API
{
    public class Loger : ILoger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
    public interface ILoger
    {
        void Log(string message);
    }
}
