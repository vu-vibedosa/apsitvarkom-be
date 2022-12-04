using System.Net;
using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Moq;
using Moq.Protected;

namespace Apsitvarkom.UnitTests.DataAccess;

public class GoogleGeocoderTests
{
    private Mock<HttpMessageHandler> _handlerMock;
    private IGeocoder _geocoder;

    [SetUp]
    public void SetUp()
    {
        var apiKeyProvider = new Mock<IApiKeyProvider>();
        _handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("https://mockreversegeocoder.com")
        };
        _geocoder = new GoogleGeocoder(httpClient, apiKeyProvider.Object);
    }

    [Test]
    public Task ReverseGeocodeAsync_OkStatusCodeReturned_SingleAddressRetrieved_TitleSuccessfullyReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}]}";

        return TestReverseGeocodeAsyncRequest(responseJsonString, title);
    }

    [Test]
    public Task ReverseGeocodeAsync_OkStatusCodeReturned_SeveralAddressesRetrieved_FirstTitleReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}, {\"formatted_address\": \"second address\"}]}";

        return TestReverseGeocodeAsyncRequest(responseJsonString, title);
    }

    [Test]
    public Task ReverseGeocodeAsync_OkStatusCodeReturned_NullAddressRetrieved_NullReturned()
    {
        const string responseJsonString = "{\"results\":[{\"formatted_address\":null}]}";

        return TestReverseGeocodeAsyncRequest(responseJsonString);
    }

    [Test]
    public Task ReverseGeocodeAsync_ZeroResultsStatusCodeRetrieved_NullReturned() =>
        TestReverseGeocodeAsyncRequest("{\"results\":[]}");

    [Test]
    public Task ReverseGeocodeAsync_EmptyResultsResponseRetrieved_NullReturned() =>
        TestReverseGeocodeAsyncRequest("{\"results\":null}");

    [Test]
    [TestCase("null")]
    [TestCase("{}")]
    public Task ReverseGeocodeAsync_EmptyResponseRetrieved_NullReturned(string response) =>
        TestReverseGeocodeAsyncRequest(response);

    [Test]
    public async Task GetLocationTitlesAsync_TitlesForAllLanguagesReturned()
    {
        var title1 = "text";
        var contentString1 = "{\"results\":[{\"formatted_address\":\"" + title1 + "\"}]}";
        var response1 = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(contentString1)
        };

        var contentString2 = "{\"results\":null}";
        var response2 = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(contentString2)
        };

        _handlerMock.Protected().SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        var expectedResult = new Translated<string>(title1, string.Empty);

        var result = await _geocoder.ReverseGeocodeAsync(new Coordinates());

        Assert.That(result.English, Is.EqualTo(expectedResult.English));
        Assert.That(result.Lithuanian, Is.EqualTo(expectedResult.Lithuanian));

        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(2),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    private async Task TestReverseGeocodeAsyncRequest(string responseJsonString, string? expectedResult = null)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseJsonString),
        };

        _handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var result = await _geocoder.ReverseGeocodeAsync(new Coordinates(), SupportedLanguages.English);

        Assert.That(result, Is.EqualTo(expectedResult));
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}