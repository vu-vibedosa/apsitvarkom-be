using Apsitvarkom.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Apsitvarkom.Utilities;

/// <summary>
/// Provides an instance of <see cref="FileLogger"/> that is meant to be added to <see cref="ILoggingBuilder"/>.
/// </summary>
[ProviderAlias("File")]
public class FileLoggerProvider : ILoggerProvider
{
    // Commiting a bit of a crime here by setting this to static
    public static FileLogger? Logger { get; private set; }

    private FileLoggerConfiguration _configuration;
    private readonly IDisposable _onChangeToken;

    /// <summary>Constructor for <see cref="FileLoggerProvider"/>.</summary>
    /// <param name="_path">Output file name.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerConfiguration> configurationMonitor)
    {
        _configuration = configurationMonitor.CurrentValue;
        _onChangeToken = configurationMonitor.OnChange(updatedConfiguration => _configuration = updatedConfiguration);
    }
    
    /// <summary>Creates an instance of <see cref="FileLogger"/>.</summary>
    public ILogger CreateLogger(string categoryName)
    {
        Logger ??= new FileLogger(_configuration);
        return Logger;
    }

    public void Dispose()
    {
        _onChangeToken?.Dispose();
        Logger?.Dispose();
        GC.SuppressFinalize(this);
    }
}
