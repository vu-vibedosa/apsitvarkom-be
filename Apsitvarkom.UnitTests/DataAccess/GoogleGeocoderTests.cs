using System.Net;
using System.Text;
using Apsitvarkom.Configuration;
using Apsitvarkom.DataAccess;
using Moq;
using Moq.Protected;

namespace Apsitvarkom.UnitTests.DataAccess;

public class GoogleGeocoderTests
{
    private static readonly IApiKeyProvider _apiKeyProvider = new Mock<IApiKeyProvider>().Object;

    [Test]
    public async Task SuccessfullyFindsFirstAddress()
    {
        const string title = "Hello World";
        const string responseJsonString = "{\"results\":[{\"formatted_address\":\"" + title + "\"}, {\"formatted_address\": \"second address\"}]}";

        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseJsonString),
        };

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);
        httpClient.BaseAddress = new Uri("https://anything.com");
        IGeocoder geocoder = new GoogleGeocoder(httpClient, _apiKeyProvider);

        var result = await geocoder.ReverseGeocodeAsync(new()
        {
            Latitude = 0,
            Longitude = 0
        });

        Assert.That(result, Is.EqualTo(title));
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}