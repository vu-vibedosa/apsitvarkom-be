using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

public class FileLoggerProvider : ILoggerProvider
{
    private string path;

    public FileLoggerProvider(string _path)
    {
        path = _path;
        CreateLogger("Error");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(path);
    }

    public void Dispose()
    {
    }
}
