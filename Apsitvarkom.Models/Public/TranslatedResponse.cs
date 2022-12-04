using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class TranslatedResponse<T>
{
    public T En { get; set; } = default!;
    public T Lt { get; set; } = default!;
}

public class TranslatedResponseStringValidator : AbstractValidator<TranslatedResponse<string>>
{
    public TranslatedResponseStringValidator()
    {
        RuleFor(l => l.En).NotNull();
        RuleFor(l => l.Lt).NotNull();
    }
}