namespace Apsitvarkom.Configuration;

public sealed class FileLoggerConfiguration
{
    public string Path { get; set; } = null!;
    public int? InformOnEachAmountOfBytes { get; set; }
}
