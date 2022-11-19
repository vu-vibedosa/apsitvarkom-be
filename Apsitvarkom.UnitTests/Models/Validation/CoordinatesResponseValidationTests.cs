using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class CoordinatesResponseValidationTests
{
    private static readonly IValidator<CoordinatesResponse> Validator = new CoordinatesResponseValidator();

    private static readonly CoordinatesResponse[] ValidCoordinateResponses =
    {
        new()
        {
            Latitude = 35.929673,
            Longitude = -78.948237,
        },
        new()
        {
            Latitude = 0,
            Longitude = 0,
        },
    };

    private static readonly CoordinatesResponse[] InvalidCoordinateResponses =
    {
        new()
        {
            Latitude = -91,
            Longitude = 0,
        },
        new()
        {
            Latitude = 91,
            Longitude = 0,
        },
        new()
        {
            Latitude = 0,
            Longitude = -181,
        },
        new()
        {
            Latitude = 0,
            Longitude = 181,
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidCoordinateResponses))]
    public async Task ValidInputShouldSucceedValidation(CoordinatesResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCoordinateResponses))]
    public async Task InvalidInputShouldFailValidation(CoordinatesResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}
