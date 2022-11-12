using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private static readonly object _lock = new object();

    public FileLogger(string path)
    {
        _filePath = path;
        if (!File.Exists(_filePath))
            File.Create(_filePath);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
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
                if(exception is not null)
                    File.AppendAllText(_filePath, $"{exception} {exception.Message}\n {exception.StackTrace}");
            }
        }
    }
}
