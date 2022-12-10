using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventCreateRequestValidator : AbstractValidator<CleaningEventCreateRequest>
{
    private readonly IRepository<CleaningEvent> _cleaningEventRepository;

    public CleaningEventCreateRequestValidator(IRepository<CleaningEvent> cleaningEventRepository)
    {
        _cleaningEventRepository = cleaningEventRepository;

        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
        RuleFor(l => l.PollutedLocationId).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.PollutedLocationId).MustAsync(async (req, _, context, _) =>
            {
                bool cleaningEventExists;
                try
                {
                    cleaningEventExists = await _cleaningEventRepository.ExistsByPropertyAsync(x => x.PollutedLocationId == req.PollutedLocationId && x.IsFinalized != true);
                }
                catch (Exception)
                {
                    return true;
                }

                if (cleaningEventExists is false)
                    return true;

                context.AddFailure("One active cleaning event for this polluted location already exists.");
                return true;
            });
        });
    }
}
