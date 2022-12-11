using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator, IValidator<TranslatedResponse<string>> translatedResponseStringValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
        RuleFor(location => location.Title).SetValidator(translatedResponseStringValidator);
    }
}