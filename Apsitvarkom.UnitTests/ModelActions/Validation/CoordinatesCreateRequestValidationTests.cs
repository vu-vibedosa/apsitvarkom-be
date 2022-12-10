using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CoordinatesCreateRequestValidationTests
{
    private static readonly IValidator<CoordinatesCreateRequest> Validator = new CoordinatesCreateRequestValidator();

    private static readonly CoordinatesCreateRequest[] ValidCoordinatesCreateRequest =
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

    private static readonly CoordinatesCreateRequest[] InvalidCoordinatesCreateRequest =
    {
        new(),
        new()
        {
            Latitude = null,
        },
        new()
        {
            Longitude = null,
        },
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
    [TestCaseSource(nameof(ValidCoordinatesCreateRequest))]
    public async Task ValidInputShouldSucceedValidation(CoordinatesCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCoordinatesCreateRequest))]
    public async Task InvalidInputShouldFailValidation(CoordinatesCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}
