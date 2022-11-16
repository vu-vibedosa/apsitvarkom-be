using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class CoordinatesGetResponseValidationTests
{
    private static readonly IValidator<CoordinatesGetResponse> Validator = new CoordinatesGetResponseValidator();

    private static readonly CoordinatesGetResponse[] ValidCoordinateGetRequests =
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

    private static readonly CoordinatesGetResponse[] InvalidCoordinateGetRequests =
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
    [TestCaseSource(nameof(ValidCoordinateGetRequests))]
    public async Task ValidInputShouldSucceedValidation(CoordinatesGetResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCoordinateGetRequests))]
    public async Task InvalidInputShouldFailValidation(CoordinatesGetResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}
