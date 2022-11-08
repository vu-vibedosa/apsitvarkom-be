
using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private static readonly object _lock = new object();

    public FileLogger(string path)
    {
        _filePath = path;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel is LogLevel.Error;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter is not null)
        {
            lock (_lock)
            {
                string filePath = Path.Combine(_filePath, "ApsitvarkomApi.log");
                var n = Environment.NewLine;
                string exc = "";
                if (exception is not null)
                    exc = $"{exception} : {DateTime.Now}{n}";

                File.AppendAllText(filePath, logLevel.ToString() + formatter(state, exception) + n + exc);
            }
        }
    }

    public void Log(string message)
    {
            lock (_lock)
            {
                string filePath = Path.Combine(_filePath, "ApsitvarkomApi.log");
                var n = Environment.NewLine;
                string exc = "";
                File.AppendAllText(filePath, message + n + exc);
            }
        }
}
