using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class TranslatedResponse<T>
{
    public T en { get; set; } = default!;
    public T lt { get; set; } = default!;
}

public class TranslatedResponseStringValidator : AbstractValidator<TranslatedResponse<string>>
{
    public TranslatedResponseStringValidator()
    {
        RuleFor(l => l.en).NotNull();
        RuleFor(l => l.lt).NotNull();
    }
}