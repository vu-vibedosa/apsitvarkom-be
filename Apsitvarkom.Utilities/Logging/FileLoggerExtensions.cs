using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

/// <summary>Used for registering <see cref="FileLogger"/> to the program.</summary>
public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
    {
        builder.AddProvider(new FileLoggerProvider(filePath));
        return builder;
    }
}
