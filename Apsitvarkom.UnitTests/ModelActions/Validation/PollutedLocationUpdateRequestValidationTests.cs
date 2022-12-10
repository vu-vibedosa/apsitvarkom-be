﻿using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class PollutedLocationUpdateRequestValidatorTests
{
    private static readonly IValidator<PollutedLocationUpdateRequest> Validator = new PollutedLocationUpdateRequestValidator();

    private static readonly PollutedLocationUpdateRequest[] ValidPollutedLocationUpdateRequests =
    {
        new()
        {
            Id = Guid.NewGuid(),
            Radius = 5,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Notes = "Prisoners test."
        },
        new()
        {
            Id = Guid.NewGuid(),
            Radius = 12,
            Severity = PollutedLocation.SeverityLevel.Low,
        },
    };

    private static readonly PollutedLocationUpdateRequest[] InvalidPollutedLocationUpdateRequests =
   {
        new()
        {
             // Missing id
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.High,
        },
        new()
        {
            // Missing radius
            Id = Guid.NewGuid(),
            Severity = PollutedLocation.SeverityLevel.Low,
        },
        new()
        {
            // Missing severity
            Id = Guid.NewGuid(),
            Radius = 1,
        },
        new()
        { 
            // Invalid radius
            Id = Guid.NewGuid(),
            Radius = 0,
            Severity = PollutedLocation.SeverityLevel.Low,
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidPollutedLocationUpdateRequests))]
    public async Task ValidInputShouldSucceedValidation(PollutedLocationUpdateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidPollutedLocationUpdateRequests))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationUpdateRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}