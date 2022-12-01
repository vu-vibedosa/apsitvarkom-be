using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class TranslatedResponse<T>
{
    public T en { get; set; }
    public T lt { get; set; }
}

public class TranslatedResponseValidator<T> : AbstractValidator<TranslatedResponse<T>>
{
    public TranslatedResponseValidator()
    {
        RuleFor(l => l.en).NotNull();
        RuleFor(l => l.lt).NotNull();
    }
}