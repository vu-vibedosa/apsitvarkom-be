using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventCreateRequestValidator : AbstractValidator<CleaningEventCreateRequest>
{
    private readonly IPollutedLocationRepository _pollutedLocationRepository;

    public CleaningEventCreateRequestValidator(IPollutedLocationRepository pollutedLocationRepository)
    {
        _pollutedLocationRepository = pollutedLocationRepository;

        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
        RuleFor(l => l.PollutedLocationId).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.PollutedLocationId).MustAsync(async (req, _, context, _) =>
            {
                PollutedLocation? pollutedLocation;
                try
                {
                    pollutedLocation = await _pollutedLocationRepository.GetByPropertyAsync(x => x.Id == req.PollutedLocationId);
                }
                catch (Exception)
                {
                    return true;
                }

                if (pollutedLocation is null)
                    return true;

                if (pollutedLocation.Progress == 100)
                    context.AddFailure("No more events for this polluted location can be created, as it is already cleaned up.");

                if (pollutedLocation.Events.Any(x => !x.IsFinalized))
                    context.AddFailure("One active cleaning event for this polluted location already exists.");
                
                return true;
            });
        });
    }
}
