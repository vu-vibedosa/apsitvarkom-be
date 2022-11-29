using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class ObjectIdentifyRequest
{
    public Guid? Id { get; set; }
}

public class ObjectIdentifyRequestValidator : AbstractValidator<ObjectIdentifyRequest>
{
    public ObjectIdentifyRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
    }
}