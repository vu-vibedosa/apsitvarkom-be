namespace Apsitvarkom.Api
{
    internal static class EnvironmentExtensions
    {
        internal static bool IsLocal(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("Local");
    }
}
