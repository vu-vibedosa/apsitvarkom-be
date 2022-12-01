using Apsitvarkom.Models;

namespace Apsitvarkom.UnitTests.Models;

public class TranslatedTests
{
    [Test]
    public void Update_LanguageSupported_IsSuccess()
    {
        var translatedString = new Translated<string>();
        var strEng1 = "value1";
        var strEng2 = "value2";
        var strLit = "reiksme";

        Assert.DoesNotThrow(() => translatedString.Update(SupportedLanguages.English, strEng1));
        Assert.DoesNotThrow(() => translatedString.Update(SupportedLanguages.Lithuanian, strLit));

        Assert.That(translatedString.English, Is.EqualTo(strEng1));
        Assert.That(translatedString.Lithuanian, Is.EqualTo(strLit));

        Assert.DoesNotThrow(() => translatedString.Update(SupportedLanguages.English, strEng2));

        Assert.That(translatedString.English, Is.EqualTo(strEng2));
        Assert.That(translatedString.Lithuanian, Is.EqualTo(strLit));
    }

    [Test]
    public void Update_LanguageNotSupported_Throws()
    {
        var translatedString = new Translated<string>();

        Assert.Throws<InvalidOperationException>(() => translatedString.Update((SupportedLanguages)3, "value"));
    }

    [Test]
    [TestCase(SupportedLanguages.Lithuanian, "lt")]
    [TestCase(SupportedLanguages.English, "en")]
    [TestCase((SupportedLanguages)15, "en")]
    public void GetSupportedLocale_ReturnsCorrectValue(SupportedLanguages language, string localeValue)
    {
        Assert.That(Translated<string>.GetSupportedLocale(language), Is.EqualTo(localeValue));
    }
}