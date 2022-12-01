using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.UnitTests.Models.Validation;

public class TranslatedResponseStringValidationTests
{
    private static readonly IValidator<TranslatedResponse<string>> Validator = new TranslatedResponseValidator<string>();

    private static readonly TranslatedResponse<string>[] ValidTranslatedStringResponses =
    {
        new()
        {
            en = "eng",
            lt = "ltu",
        },
        new()
        {
            en = "eng",
            lt = string.Empty,
        }
    };

    private static readonly TranslatedResponse<string>[] InvalidTranslatedStringResponses =
    {
        new()
        {
            en = "eng",
            lt = null,
        },
        new()
        {
            en = "eng",
        },
        new(),
    };

    [Test]
    [TestCaseSource(nameof(ValidTranslatedStringResponses))]
    public async Task ValidInputShouldSucceedValidation(TranslatedResponse<string> input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Empty);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(InvalidTranslatedStringResponses))]
    public async Task InvalidInputShouldFailValidation(TranslatedResponse<string> input)
    {
        var result = await Validator.ValidateAsync(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Errors, Is.Not.Empty);
        Assert.That(result.IsValid, Is.False);
    }
}