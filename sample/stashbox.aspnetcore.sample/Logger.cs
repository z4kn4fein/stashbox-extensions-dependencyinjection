using System.Diagnostics;

namespace Stashbox.AspNetCore.Sample
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class CustomLogger : ILogger
    {
        public void Log(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
