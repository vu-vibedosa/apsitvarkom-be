using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class TranslatedResponseStringValidator : AbstractValidator<TranslatedResponse<string>>
{
    public TranslatedResponseStringValidator()
    {
        RuleFor(l => l.En).NotNull();
        RuleFor(l => l.Lt).NotNull();
    }
}