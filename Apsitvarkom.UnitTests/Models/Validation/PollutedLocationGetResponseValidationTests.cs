﻿using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class PollutedLocationGetResponseValidationTests
{
    private static readonly IValidator<PollutedLocationGetResponse> Validator =
        new PollutedLocationGetResponseValidator(new LocationGetResponseValidator(new CoordinatesGetResponseValidator()));

    private static readonly Guid DummyGuid = Guid.NewGuid();

    private static readonly PollutedLocationGetResponse[] ValidPollutedLocationGetResponses =
    {
        new()
        {
            Id = DummyGuid,
            Location =
            {
                Coordinates =
                {
                    Latitude = 54.691452,
                    Longitude = 25.266276
                }
            },
            Radius = 5,
            Severity = PollutedLocation.SeverityLevel.Moderate,
            Spotted = DateTime.Parse("2019-08-23T14:05:43Z").ToUniversalTime(),
            Progress = 41,
            Notes = "Prisoners broke a window."
        },
        new()
        {
            Id = DummyGuid,
            Location =
            {
                Coordinates =
                {
                    Latitude = 54.675369,
                    Longitude = 25.273316
                }
            },
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = DateTime.Parse("2023-04-13T07:16:55Z").ToUniversalTime(),
            Progress = 13,
        },
    };

    private static readonly PollutedLocationGetResponse[] InvalidPollutedLocationGetResponses =
    {
        new()
        {
            // Invalid Location coordinates
            Id = DummyGuid,
            Location = 
            {
                Coordinates =
                {
                    Latitude = 85,
                    Longitude = -720
                }
            },
            Notes = "gtfo with these cringy notes",
            Progress = 1,
            Radius = 1,
            Severity = PollutedLocation.SeverityLevel.High,
            Spotted = new DateTime(1990, 3, 11)
        },
        new()
        { 
            // Invalid Progress
            Id = DummyGuid,
            Location =
            {
                Coordinates =
                {
                    Latitude = 89.9,
                    Longitude = 89.9
                },
            },
            Progress = 101,
            Radius = 2,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = new DateTime(1995, 8, 24)
        },
        new()
        { 
            // Invalid Radius
            Id = DummyGuid,
            Location =
            {
                Coordinates =
                {
                    Latitude = 89.9,
                    Longitude = 89.9
                },
            },
            Progress = 11,
            Radius = 0,
            Severity = PollutedLocation.SeverityLevel.Low,
            Spotted = new DateTime(1995, 8, 24)
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidPollutedLocationGetResponses))]
    public async Task ValidInputShouldSucceedValidation(PollutedLocationGetResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidPollutedLocationGetResponses))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationGetResponse input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        // Only 1 thing must error in the invalid input
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}