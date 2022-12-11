using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class ObjectIdentifyRequestValidator : AbstractValidator<ObjectIdentifyRequest>
{
    public ObjectIdentifyRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
    }
}