using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CleaningEventUpdateRequest
{
    public Guid? Id { get; set; }

    public DateTime? StartTime { get; set; }

    public string? Notes { get; set; }
}

public class CleaningEventUpdateRequestValidator : AbstractValidator<CleaningEventUpdateRequest>
{
    public CleaningEventUpdateRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
    }
}
