using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

/// <summary>
/// Provides an instance of <see cref="FileLogger"/> that is meant to be added to <see cref="ILoggingBuilder"/>.
/// </summary>
public class FileLoggerProvider : ILoggerProvider
{
    private readonly string path;

    /// <summary>Constructor for <see cref="FileLoggerProvider"/>.</summary>
    /// <param name="_path">Output file name.</param>
    public FileLoggerProvider(string _path)
    {
        path = _path;
        CreateLogger("Error");
    }
    
    /// <summary>Creates an instance of <see cref="FileLogger"/>.</summary>
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(path);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
