using System.Linq.Expressions;
using Apsitvarkom.DataAccess;
using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using FluentValidation;
using Moq;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class CleaningEventUpdateRequestValidationTests
{
    private IValidator<CleaningEventUpdateRequest> _validator;
    private Mock<IRepository<CleaningEvent>> _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IRepository<CleaningEvent>>();
        _validator = new CleaningEventUpdateRequestValidator(_repository.Object);
    }

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
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidCleaningEventUpdateRequests))]
    public async Task InvalidInputShouldFailValidation(CleaningEventUpdateRequest input)
    {
        var result = await _validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateAvailable_ShouldSucceedValidation()
    {
        var input = ValidCleaningEventUpdateRequests.First();
        var cleaningEvent = new CleaningEvent
        {
            Id = (Guid)input.Id!,
            IsFinalized = false
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(cleaningEvent);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_UpdateUnavailable_ShouldFailValidation()
    {
        var input = ValidCleaningEventUpdateRequests.First();
        var cleaningEvent = new CleaningEvent
        {
            Id = (Guid)input.Id!,
            IsFinalized = true
        };

        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).ReturnsAsync(cleaningEvent);

        var result = await _validator.ValidateAsync(input);

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task ValidInputWithRepositoryCall_RepositoryThrows_NoErrorsReturned()
    {
        _repository.Setup(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>())).Throws<Exception>();

        var result = await _validator.ValidateAsync(ValidCleaningEventUpdateRequests.First());

        _repository.Verify(x => x.GetByPropertyAsync(It.IsAny<Expression<Func<CleaningEvent, bool>>>()), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }
}