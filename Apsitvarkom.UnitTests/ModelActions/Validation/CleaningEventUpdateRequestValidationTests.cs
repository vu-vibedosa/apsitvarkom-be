using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CleaningEventUpdateRequestValidationTests
{
    private static readonly IValidator<CleaningEventUpdateRequest> Validator = new CleaningEventUpdateRequestValidator();

    private static readonly CleaningEventUpdateRequest[] ValidCleaningEventUpdateRequests =
    {
        new()
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z"),
            Notes = "boop"
        },
        new()
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z")
        },
    };

    private static readonly CleaningEventUpdateRequest[] InvalidCleaningEventUpdateRequests =
   {
        new()
        {
            // Missing id
            StartTime = DateTime.Parse("2077-03-12T10:11:12Z"),
        },
        new()
        { 
            // Missing StartTime
            Id = Guid.NewGuid(),
            Notes = "boop"
        },
        new()
        { 
            // Invalid StartTime
            Id = Guid.NewGuid(),
            StartTime = DateTime.Parse("2007-03-12T10:11:12Z"),
            Notes = "boop"
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidCleaningEventUpdateRequests))]
    public async Task ValidInputShouldSucceedValidation(CleaningEventUpdateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCleaningEventUpdateRequests))]
    public async Task InvalidInputShouldFailValidation(CleaningEventUpdateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}