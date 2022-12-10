using Apsitvarkom.DataAccess;
using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;
using Moq;
using System.Linq.Expressions;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CleaningEventCreateRequestValidationTests
{
    private IValidator<CleaningEventCreateRequest> _validator;
    private Mock<IPollutedLocationRepository> _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IPollutedLocationRepository>();
        _validator = new CleaningEventCreateRequestValidator(_repository.Object);
    }

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
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCleaningEventCreateRequests))]
    public async Task InvalidInputShouldFailValidation(CleaningEventCreateRequest input)
    {
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        // Only 1 thing must error in the invalid input
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateAvailable_ShouldSucceedValidation()
    {
        var input = ValidCleaningEventCreateRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = 98,
            Events = new List<CleaningEvent>
            {
                new()
                {
                    IsFinalized = true
                }
            }
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(pollutedLocation);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateUnavailable_NotFinalizedEventAlreadyExists_ShouldFailValidation()
    {
        var input = ValidCleaningEventCreateRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = 98,
            Events = new List<CleaningEvent>
            {
                new()
                {
                    IsFinalized = false
                }
            }
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(pollutedLocation);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateUnavailable_PollutedLocationTidiedUp_ShouldFailValidation()
    {
        var input = ValidCleaningEventCreateRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = 100,
            Events = new List<CleaningEvent>()
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(pollutedLocation);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_RepositoryThrows_NoErrorsReturned()
    {
        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).Throws<Exception>();

        var result = await _validator.ValidateAsync(ValidCleaningEventCreateRequests.First());

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }
}