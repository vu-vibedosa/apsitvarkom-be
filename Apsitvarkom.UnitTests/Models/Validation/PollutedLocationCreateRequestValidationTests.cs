using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class PollutedLocationCreateRequestValidationTests
{
    private static readonly IValidator<PollutedLocationCreateRequest> Validator =
        new PollutedLocationCreateRequestValidator(new CoordinatesCreateRequestValidator());

    private static readonly PollutedLocationCreateRequest[] ValidPollutedLocationCreateRequests =
    {
        new()
        {
            Coordinates = new()
            {
                Latitude = 54.691452,
                Longitude = 25.266276
            },
            Radius = 5,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Progress = 41,
            Notes = "Prisoners broke a window."
        },
        new()
        {
            Coordinates = new()
            {
                Latitude = 54.675369,
                Longitude = 25.273316
            },
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.Low,
            Progress = 13,
        },
    };

    private static readonly PollutedLocationCreateRequest[] InvalidPollutedLocationCreateRequests =
    {
        new()
        {
            // Invalid Location coordinates
            Coordinates = new()
            {
                Latitude = 94,
                Longitude = 150
            },
            Progress = 1,
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.High,
        },
        new()
        {
            // Invalid Location coordinates
            Coordinates = new()
            {
                Latitude = 85,
                Longitude = -720
            },
            Notes = "gtfo with these cringy notes",
            Progress = 1,
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.High,
        },
        new()
        { 
            // Invalid Progress
            Coordinates = new()
            {
                Latitude = 89.9,
                Longitude = 89.9
            },
            Progress = 101,
            Radius = 2,
            Severity = PollutedLocation.SeverityLevel.Low,
        },
        new()
        { 
            // Invalid Radius
            Coordinates = new()
            {
                Latitude = 89.9,
                Longitude = 89.9
            },
            Progress = 11,
            Radius = 0,
            Severity = PollutedLocation.SeverityLevel.Low,
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidPollutedLocationCreateRequests))]
    public async Task ValidInputShouldSucceedValidation(PollutedLocationCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidPollutedLocationCreateRequests))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        // Only 1 thing must error in the invalid input
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}
