using Apsitvarkom.ModelActions.Validation;
using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.ModelActions.Validation;

public class ObjectIdentifyRequestValidationTests
{
    private static readonly IValidator<ObjectIdentifyRequest> Validator = new ObjectIdentifyRequestValidator();

    private static readonly ObjectIdentifyRequest[] ValidObjectIdentifyRequest =
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

    private static readonly ObjectIdentifyRequest[] InvalidObjectIdentifyRequest =
    {
        new()
        {
            Id = null
        }
    };

    [Test]
    [TestCaseSource(nameof(ValidObjectIdentifyRequest))]
    public async Task ValidInputShouldSucceedValidation(ObjectIdentifyRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidObjectIdentifyRequest))]
    public async Task InvalidInputShouldFailValidation(ObjectIdentifyRequest input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.IsValid, Is.False);
    }
}