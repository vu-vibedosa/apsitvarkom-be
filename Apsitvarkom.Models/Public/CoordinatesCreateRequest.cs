﻿using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CoordinatesCreateRequest
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class CoordinatesCreateRequestValidator : AbstractValidator<CoordinatesCreateRequest>
{
    public CoordinatesCreateRequestValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).NotNull().InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).NotNull().InclusiveBetween(-180.0, 180.0);
    }
}