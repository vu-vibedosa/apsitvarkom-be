using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationIdentifyRequest
{
    public Guid? Id { get; set; }
}

public class PollutedLocationIdentifyRequestValidator : AbstractValidator<PollutedLocationIdentifyRequest>
{
    public PollutedLocationIdentifyRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
    }
}