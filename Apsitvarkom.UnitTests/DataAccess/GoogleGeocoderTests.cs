using System.Net;
using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Moq;
using Moq.Protected;

namespace Apsitvarkom.UnitTests.DataAccess;

public class GoogleGeocoderTests
{
    private Mock<IApiKeyProvider> _apiKeyProvider;
    private Mock<HttpMessageHandler> _handlerMock;
    private IGeocoder _geocoder;

    [SetUp]
    public void SetUp()
    {
        _apiKeyProvider = new Mock<IApiKeyProvider>();
        _handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("https://mockreversegeocoder.com")
        };
        _geocoder = new GoogleGeocoder(httpClient, _apiKeyProvider.Object);
    }

    [Test]
    public void OkStatusCodeReturned_SingleAddressRetrieved_TitleSuccessfullyReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}], \"status\": \"OK\"}";

        TestReverseGeocodeRequest(responseJsonString, title);
    }

    [Test]
    public void OkStatusCodeReturned_SeveralAddressesRetrieved_FirstTitleReturned()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}, {\"formatted_address\": \"second address\"}], \"status\": \"OK\"}";

        TestReverseGeocodeRequest(responseJsonString, title);
    }

    [Test]
    public void OkStatusCodeReturned_NullAddressRetrieved_NullReturned()
    {
        const string responseJsonString = "{\"results\":[{\"formatted_address\":null}], \"status\": \"OK\"}";

        TestReverseGeocodeRequest(responseJsonString);
    }

    [Test]
    public void ZeroResultsStatusCodeRetrieved_NullReturned() =>
        TestReverseGeocodeRequest("{\"results\":[], \"status\": \"ZERO_RESULTS\"}");

    [Test]
    public void EmptyResultsResponseRetrieved_NullReturned() =>
        TestReverseGeocodeRequest("{\"results\":null}");

    [Test]
    [TestCase("")]
    [TestCase("null")]
    [TestCase("{}")]
    public void EmptyResponseRetrieved_NullReturned(string response) =>
        TestReverseGeocodeRequest(response);

    private async void TestReverseGeocodeRequest(string responseJsonString, string? expectedResult = null)
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