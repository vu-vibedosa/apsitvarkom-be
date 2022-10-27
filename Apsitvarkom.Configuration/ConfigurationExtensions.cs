using Microsoft.Extensions.Configuration;

namespace Apsitvarkom.Configuration;

public static class ConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
    {
        return configuration.GetValue<T>(key) ?? throw new ArgumentNullException(key, "Required configuration value could not be found");
    }
}