using Apsitvarkom.Models;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation
{
    internal class PollutedLocationValidationTests
    {
        private static readonly IValidator<PollutedLocation> Validator = new PollutedLocationValidator();
        private static readonly Guid dummyGuid = Guid.NewGuid();

        private static readonly PollutedLocation[] ValidPollutionLocations =
        {
            new()
            {
                Id = dummyGuid,
                Location = new Location
                {
                    Latitude = 35.929673,
                    Longitude = -78.948237,
                },
                Notes = "I love writting tests",
                Progress = 11,
                Radius = 5,
                Severity = Enumerations.LocationSeverityLevel.High,
                Spotted = new DateTime(2002, 5, 23)
            },
            new()
            {
                Id = dummyGuid,
                Location = new Location
                {
                    Latitude = -90,
                    Longitude = 180,
                },
                Progress = 0,
                Radius = 1,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = new DateTime(1410, 7, 15)
            },
        };

            private static readonly PollutedLocation[] InvalidPollutedLocations =
            {
            new()
            {
                // Invalid Location coordinates
                Id = dummyGuid,
                Location = new Location
                {
                    Latitude = 210,
                    Longitude = -720,
                },
                Notes = "Hey Ho",
                Progress = 0,
                Radius = 0,
                Severity = Enumerations.LocationSeverityLevel.High,
                Spotted = new DateTime(1990, 3, 11)
            },
            new()
            {
                // Location (special case - not a primitive) is missing
                Id = dummyGuid,
                Notes = "",
                Progress = 0,
                Radius = 0,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = new DateTime(2022, 9, 24)
            },
            new()
            { 
                // Invalid Progress and Radius
                Id = dummyGuid,
                Location = new Location
                {
                    Latitude = 89.9,
                    Longitude = 89.9,
                },
                Progress = 101,
                Radius = 0,
                Severity = Enumerations.LocationSeverityLevel.Low,
                Spotted = new DateTime(1995, 8, 24)
            },
        };

        [Test]
        [TestCaseSource(nameof(ValidPollutionLocations))]
        public async Task ValidInputShouldSucceedValidation(PollutedLocation pollutedLocations)
        {
            var result = await Validator.ValidateAsync(pollutedLocations);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(InvalidPollutedLocations))]
        public async Task InvalidInputShouldFailValidation(PollutedLocation pollutedLocations)
        {
            var result = await Validator.ValidateAsync(pollutedLocations);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Errors, Is.Not.Empty);
            Assert.That(result.IsValid, Is.False);
        }

    }
}
