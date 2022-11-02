using Microsoft.Extensions.Logging;

namespace Apsitvarkom.Utilities;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder factory, string filePath)
    {
        factory.AddProvider(new FileLoggerProvider(filePath));
        return factory;
    }
}
