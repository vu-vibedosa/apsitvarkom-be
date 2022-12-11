using Apsitvarkom.DataAccess;
using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;
using Moq;
using System.Linq.Expressions;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class PollutedLocationUpdateRequestValidatorTests
{
    private IValidator<PollutedLocationUpdateRequest> _validator;
    private Mock<IPollutedLocationRepository> _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IPollutedLocationRepository>();
        _validator = new PollutedLocationUpdateRequestValidator(_repository.Object);
    }

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
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidPollutedLocationUpdateRequests))]
    public async Task InvalidInputShouldFailValidation(PollutedLocationUpdateRequest input)
    {
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateAvailable_ShouldSucceedValidation()
    {
        var input = ValidPollutedLocationUpdateRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = 12
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>())).ReturnsAsync(pollutedLocation);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateUnavailable_ShouldFailValidation()
    {
        var input = ValidPollutedLocationUpdateRequests.First();
        var pollutedLocation = new PollutedLocation
        {
            Progress = 100
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

        var result = await _validator.ValidateAsync(ValidPollutedLocationUpdateRequests.First());

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<PollutedLocation, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }
}