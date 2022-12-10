using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventFinalizeRequestValidator : AbstractValidator<CleaningEventFinalizeRequest>
{
    private readonly IPollutedLocationRepository _pollutedLocationRepository;

    public CleaningEventFinalizeRequestValidator(IPollutedLocationRepository pollutedLocationRepository)
    {
        _pollutedLocationRepository = pollutedLocationRepository;

        RuleFor(l => l.NewProgress).NotNull().InclusiveBetween(0, 100);
        RuleFor(l => l.Id).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.Id).MustAsync(async (req, _, context, _) =>
            {
                PollutedLocation? pollutedLocation;
                try
                {
                    pollutedLocation = await _pollutedLocationRepository.GetByPropertyAsync(x => x.Events.Any(e => e.Id == req.Id));
                }
                catch (Exception)
                {
                    return true;
                }

                if (pollutedLocation is null)
                    return true;

                var cleaningEvent = pollutedLocation.Events.Single(x => x.Id == req.Id);

                if (cleaningEvent.IsFinalized)
                {
                    context.AddFailure("The cleaning event is already finalized.");
                    return true;
                }

                if (cleaningEvent.StartTime > DateTime.UtcNow)
                    context.AddFailure("The requested cleaning event hasn't yet happened.");

                if (req.NewProgress is not null && pollutedLocation.Progress > req.NewProgress)
                    context.AddFailure($"New progress value cannot be lower than the current ({pollutedLocation.Progress}).");

                return true;
            });
        });
    }
}
