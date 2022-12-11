using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class PollutedLocationUpdateRequestValidator : AbstractValidator<PollutedLocationUpdateRequest>
{
    private readonly IPollutedLocationRepository _pollutedLocationRepository;

    public PollutedLocationUpdateRequestValidator(IPollutedLocationRepository pollutedLocationRepository)
    {
        _pollutedLocationRepository = pollutedLocationRepository;

        RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(l => l.Severity).NotNull();
        RuleFor(l => l.Id).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.Id).MustAsync(async (req, _, context, _) =>
            {
                PollutedLocation? pollutedLocation;
                try
                {
                    pollutedLocation = await _pollutedLocationRepository.GetByPropertyAsync(x => x.Id == req.Id);
                }
                catch (Exception)
                {
                    return true;
                }

                if (pollutedLocation is null)
                    return true;

                if (pollutedLocation.Progress == 100)
                    context.AddFailure("The polluted location is already cleaned up.");

                return true;
            });
        });
    }
}