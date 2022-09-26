using Apsitvarkom.Models.DTO;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class PollutedLocationDTOValidationTests
{
    private static readonly IValidator<PollutedLocationDTO> Validator = new PollutedLocationDTOValidator();

    private static readonly PollutedLocationDTO[] ValidInputDTOs =
    {
        new()
        {
            Id = "5be2354e-2500-4289-bbe2-66210592e17f",
            Location = new LocationDTO
            {
                Latitude = 35.929673,
                Longitude = -78.948237,
            },
            Notes = "Lorem ipsum",
            Progress = 42,
            Radius = 15,
            Severity = "Moderate",
            Spotted = "2022-09-14T17:35:23Z"
        },
        new()
        {
            // The DTO does not have to comply to business requirements
            // They should be handled when validating PollutedLocation (not the DTO)
            Id = "378a4760-6fb9-42a9-87b2-1cece5913ffd",
            Location = new LocationDTO
            {
                Latitude = 0,
                Longitude = 0,
            },
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Low",
            Spotted = "1889-04-20"
        },
    };

    private static readonly PollutedLocationDTO[] InvalidInputDTOs =
    {
        new(), // All fields null
        new()
        {
            // All fields initialized, but fields have empty strings where content is required
            Id = "",
            Location = new LocationDTO
            {
                Latitude = 0,
                Longitude = 0,
            },
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "",
            Spotted = ""
        },
        new()
        {
            // Location (special case - not a primitive) is missing
            Id = "hey",
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Whatever",
            Spotted = "timestamp"
        },
        new()
        {
            // Location (special case - not a primitive) fields are not initialized
            Id = "hey",
            Location = new LocationDTO(),
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Whatever",
            Spotted = "timestamp"
        },
        new()
        { 
            // Invalid Guid, DTOValidator should throw errors
            Id = "Invalid Guid 45621e-9898-sd-565",
            Location = new LocationDTO
            {
                Latitude = 1,
                Longitude = 2,
            },
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Moderate",
            Spotted = "2000-02-12"
        },
        new()
        {
            // Invalid Severity, DTOValidator should throw
            Id = "771973aa-470f-4996-8b54-d4c0bcfff94b",
            Location = new LocationDTO
            {
                Latitude = 1,
                Longitude = 2,
            },
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Whatever",
            Spotted = "1111-02-11"
        },
         new()
        {
            // Invalid Spotted date format, DTOValidator should throw
            Id = "771973aa-470f-4996-8b54-d4c0bcfff94b",
            Location = new LocationDTO
            {
                Latitude = 1,
                Longitude = 2,
            },
            Notes = "",
            Progress = 0,
            Radius = 0,
            Severity = "Low",
            Spotted = "1nv4l1d d4t3"
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidInputDTOs))]
    public async Task ValidInputShouldSucceedValidation(PollutedLocationDTO inputDTO)
    {
        var result = await Validator.ValidateAsync(inputDTO);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidInputDTOs))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationDTO inputDTO)
    {
        var result = await Validator.ValidateAsync(inputDTO);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}
