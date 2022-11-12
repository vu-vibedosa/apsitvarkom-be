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
    public Task OkStatusCodeReturned_SingleAddressRetrieved_TitleSuccessfullyReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}]}";

        return TestReverseGeocodeRequest(responseJsonString, title);
    }

    [Test]
    public Task OkStatusCodeReturned_SeveralAddressesRetrieved_FirstTitleReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}, {\"formatted_address\": \"second address\"}]}";

        return TestReverseGeocodeRequest(responseJsonString, title);
    }

    [Test]
    public Task OkStatusCodeReturned_NullAddressRetrieved_NullReturned()
    {
        const string responseJsonString = "{\"results\":[{\"formatted_address\":null}]}";

        return TestReverseGeocodeRequest(responseJsonString);
    }

    [Test]
    public Task ZeroResultsStatusCodeRetrieved_NullReturned() =>
        TestReverseGeocodeRequest("{\"results\":[]}");

    [Test]
    public Task EmptyResultsResponseRetrieved_NullReturned() =>
        TestReverseGeocodeRequest("{\"results\":null}");

    [Test]
    [TestCase("null")]
    [TestCase("{}")]
    public Task EmptyResponseRetrieved_NullReturned(string response) =>
        TestReverseGeocodeRequest(response);

    private async Task TestReverseGeocodeRequest(string responseJsonString, string? expectedResult = null)
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

        var result = await _geocoder.ReverseGeocodeAsync(new Coordinates());

        Assert.That(result, Is.EqualTo(expectedResult));
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}