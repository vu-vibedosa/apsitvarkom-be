using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class CleaningEventUpdateRequest
{
    public Guid? Id { get; set; }

    public Guid? PollutedLocationId { get; set; }

    public DateTime? StartTime { get; set; }

    public string? Notes { get; set; }
}

public class CleaningEventUpdateRequestValidator : AbstractValidator<CleaningEventUpdateRequest>
{
    public CleaningEventUpdateRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
        RuleFor(l => l.PollutedLocationId).NotNull();
        RuleFor(l => l.StartTime).NotNull();
    }
}
