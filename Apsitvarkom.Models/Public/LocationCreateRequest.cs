using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class LocationCreateRequest
{
    public CoordinatesCreateRequest? Coordinates { get; set; }
}

public class LocationCreateRequestValidator : AbstractValidator<LocationCreateRequest>
{
    public LocationCreateRequestValidator(IValidator<CoordinatesCreateRequest> coordinatesCreateRequestValidator)
    {
        RuleFor(l => l.Coordinates).NotNull();
        RuleFor(l => l.Coordinates!).SetValidator(coordinatesCreateRequestValidator).When(l => l.Coordinates is not null);
    }
}