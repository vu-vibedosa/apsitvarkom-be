namespace Apsitvarkom.Models;

public enum SupportedLanguages
{
    English,
    Lithuanian
}

public class Translated<T>
{
    public T English { get; private set; } = default!;
    public T Lithuanian { get; private set; } = default!;

    public Translated() 
    { }

    public Translated(T english, T lithuanian)
    {
        English = english;
        Lithuanian = lithuanian;
    }

    public void Update(SupportedLanguages language, T value)
    {
        _ = language switch
        {
            SupportedLanguages.English => English = value,
            SupportedLanguages.Lithuanian => Lithuanian = value,
            _ => throw new InvalidOperationException()
        };
    }

    public static string GetSupportedLocale(SupportedLanguages language) =>
        language switch
        {
            SupportedLanguages.English => "en",
            SupportedLanguages.Lithuanian => "lt",
            _ => "en"
        };
}