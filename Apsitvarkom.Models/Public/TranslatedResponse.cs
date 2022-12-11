namespace Apsitvarkom.Models.Public;

public class TranslatedResponse<T>
{
    public T En { get; set; } = default!;
    public T Lt { get; set; } = default!;
}