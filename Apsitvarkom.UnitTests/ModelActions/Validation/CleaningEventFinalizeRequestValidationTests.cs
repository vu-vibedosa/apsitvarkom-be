using System.Linq.Expressions;
using Apsitvarkom.DataAccess;
using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;
using Moq;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CleaningEventFinalizeRequestValidationTests
{
    private IValidator<CleaningEventFinalizeRequest> _validator;
    private Mock<IPollutedLocationRepository> _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IPollutedLocationRepository>();
        _validator = new CleaningEventFinalizeRequestValidator(_repository.Object);
    }

    private static readonly CleaningEventFinalizeRequest[] ValidCleaningEventFinalizeRequests =
    {
        new()
        {
            Id = Guid.NewGuid(),
            NewProgress = 50
        }
    };

    private static readonly CleaningEventFinalizeRequest[] InvalidCleaningEventFinalizeRequests =
   {
        new()
        {
            // Missing id
            NewProgress = 12,
        },
        new()
        { 
            // Missing NewProgress
            Id = Guid.NewGuid(),
        },
        new()
        { 
            // Invalid NewProgress
            Id = Guid.NewGuid(),
            NewProgress = 101
        },
    };

    [Test]
    [TestCaseSource(nameof(ValidCleaningEventFinalizeRequests))]
    public async Task ValidInputShouldSucceedValidation(CleaningEventFinalizeRequest input)
    {
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCleaningEventFinalizeRequests))]
    public async Task InvalidInputShouldFailValidation(CleaningEventFinalizeRequest input)
    {
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_FinalizeAvailable_ShouldSucceedValidation()
    {
        var input = ValidCleaningEventFinalizeRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = (int)input.NewProgress! - 5,
            Events = new List<CleaningEvent>
            {
                new() { Id = (Guid)input.Id!, IsFinalized = false, StartTime = new DateTime(2022, 01, 01) }
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
    public async Task ValidInputWithRepositoryCall_EventAlreadyFinalized_ShouldFailValidation()
    {
        var input = ValidCleaningEventFinalizeRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = (int)input.NewProgress! - 5,
            Events = new List<CleaningEvent>
            {
                new() { Id = (Guid)input.Id!, IsFinalized = true, StartTime = new DateTime(2022, 01, 01) }
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
    public async Task ValidInputWithRepositoryCall_StartTimeInTheFuture_ShouldFailValidation()
    {
        var input = ValidCleaningEventFinalizeRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = (int)input.NewProgress! - 5,
            Events = new List<CleaningEvent>
            {
                new() { Id = (Guid)input.Id!, IsFinalized = false, StartTime = DateTime.UtcNow + TimeSpan.FromDays(1) }
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
    public async Task ValidInputWithRepositoryCall_ProgressLowerThanPollutedLocations_ShouldFailValidation()
    {
        var input = ValidCleaningEventFinalizeRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = (int)input.NewProgress! + 5,
            Events = new List<CleaningEvent>
            {
                new() { Id = (Guid)input.Id!, IsFinalized = false, StartTime = new DateTime(2022, 01, 01) }
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
    public async Task ValidInputWithRepositoryCall_RepositoryThrows_NoErrorsReturned()
    {
        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).Throws<Exception>();

        var result = await _validator.ValidateAsync(ValidCleaningEventFinalizeRequests.First());

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }
}