namespace Apsitvarkom.Models.Public;

using FluentValidation;
using static LocationTitle;

public class LocationTitleResponse
{
    public LocationCode Code { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class LocationTitleResponseValidator : AbstractValidator<LocationTitleResponse>
{
    public LocationTitleResponseValidator()
    {
        RuleFor(x => x.Name).NotNull();
    }
}