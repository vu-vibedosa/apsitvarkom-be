using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class PollutedLocationIdentifyRequestValidationTests
{
    private static readonly IValidator<PollutedLocationIdentifyRequest> Validator = new PollutedLocationIdentifyRequestValidator();

    private static readonly PollutedLocationIdentifyRequest[] ValidPollutedLocationIdentifyRequest =
    {
        new()
        {
            Id = new Guid()
        },
        new()
        {
            Id = new Guid()
        },
    };

    private static readonly PollutedLocationIdentifyRequest[] InvalidPollutedLocationIdentifyRequest =
    {
        new()
        {
            Id = null
        }
    };

    [Test]
    [TestCaseSource(nameof(ValidPollutedLocationIdentifyRequest))]
    public async Task ValidInputShouldSucceedValidation(PollutedLocationIdentifyRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidPollutedLocationIdentifyRequest))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationIdentifyRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}