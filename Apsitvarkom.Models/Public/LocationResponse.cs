using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    public CoordinatesResponse Coordinates { get; set; } = new();
    public TranslatedResponse<string> Title { get; set; } = new();
}

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator, IValidator<TranslatedResponse<string>> translatedResponseStringValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
        RuleFor(location => location.Title).SetValidator(translatedResponseStringValidator);
    }
}