using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CleaningEventCreateRequestValidationTests
{
    private static readonly IValidator<CleaningEventCreateRequest> Validator = new CleaningEventCreateRequestValidator();

    private static readonly CleaningEventCreateRequest[] ValidCleaningEventCreateRequests =
   {
        new()
        {
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            Notes = "Prisoners broke a window."
        },
        new()
        {
            PollutedLocationId = Guid.NewGuid(),
            StartTime = new DateTime(2023, 1, 1, 1, 43, 31).ToUniversalTime(),
        },
    };

    private static readonly CleaningEventCreateRequest[] InvalidCleaningEventCreateRequests =
    {
        new()
        {
            // Invalid missing PollutedLocationId
            StartTime = DateTime.UtcNow.AddDays(1),
            Notes = "Prisoners broke a window."
        },
        new()
        {
            // Invalid missing StartTime
            PollutedLocationId = Guid.NewGuid(),
            Notes = "Prisoners broke a window."
        },
         new()
        {
            // Invalid wrong StartTime
            PollutedLocationId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(-1),
            Notes = "Prisoners broke a window."
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidCleaningEventCreateRequests))]
    public async Task ValidInputShouldSucceedValidation(CleaningEventCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCleaningEventCreateRequests))]
    public async Task InvalidInputShouldFailValidation(CleaningEventCreateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        // Only 1 thing must error in the invalid input
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}