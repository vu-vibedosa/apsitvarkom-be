using System.Globalization;
using System.Text.Json;
using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using AutoMapper;

namespace Apsitvarkom.UnitTests.DataAccess;

public class PollutedLocationFileRepositoryTests
{
    // Existing mock data file, containing invalid json data.
    private static readonly string InvalidDataSourcePath = Path.Combine("DataAccess", "PollutedLocationMockInvalid.json");

    // Existing mock data file, containing three valid instances with unique property values.
    private static readonly string ValidDataSourcePath = Path.Combine("DataAccess", "PollutedLocationMockValid.json");

    private IMapper m_mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PollutedLocationProfile>();
        });
        config.AssertConfigurationIsValid();
        m_mapper = config.CreateMapper();
    }

    #region Constructor tests
    [Test]
    public void PollutedLocationDTOFileRepositoryFromFileConstructor_CouldNotFindSourceFile_Throws() =>
        Assert.Throws<FileNotFoundException>(() => PollutedLocationFileRepository.FromFile(m_mapper, Guid.NewGuid().ToString()));

    [Test]
    public void PollutedLocationDTOFileRepositoryFromFileConstructor_HappyPath() =>
        Assert.DoesNotThrow(() => PollutedLocationFileRepository.FromFile(m_mapper, ValidDataSourcePath));
        
    [Test]
    public void PollutedLocationDTOFileRepositoryFromContentConstructor_HappyPath() =>
        Assert.DoesNotThrow(() => PollutedLocationFileRepository.FromContent(m_mapper));
    #endregion

    #region GetAllAsync tests
    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", -78.948237, 35.929673, 10, PollutedLocation.SeverityLevel.Moderate, "2022-09-14T17:35:23Z", 16, "Lorem ipsum")]
    [TestCase("c3aca40c-0ed7-4b78-82b1-496d76b5e61f", 87.499999, -159.412971, 1, PollutedLocation.SeverityLevel.Low, "2015-05-16T05:50:06.7199222-04:00", 0, "")]
    public async Task GetAllAsync_DeserializingOfAllPropertiesSuccessful_OneInstanceReturned(string id, double longitude, double latitude, int radius, PollutedLocation.SeverityLevel severity, string creationTime, int progress, string notes)
    {
        var jsonString =
            "[" +
            "{" +
            $"\"id\":\"{id}\"," +
            "\"coordinates\":" +
            "{" +
            $"\"longitude\":{longitude.ToString(CultureInfo.InvariantCulture)}," +
            $"\"latitude\":{latitude.ToString(CultureInfo.InvariantCulture)}" +
            "}," +
            $"\"radius\":{radius}," +
            $"\"severity\":\"{severity}\"," +
            $"\"spotted\":\"{creationTime}\"," +
            $"\"progress\":{progress}," +
            $"\"notes\":\"{notes}\"" +
            "}" +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Has.Length.EqualTo(1));

        var instance = instances.Single();
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id.ToString(), Is.EqualTo(id));
            Assert.That(instance.Location.Coordinates.Longitude, Is.EqualTo(longitude));
            Assert.That(instance.Location.Coordinates.Latitude, Is.EqualTo(latitude));
            Assert.That(instance.Radius, Is.EqualTo(radius));
            Assert.That(instance.Severity, Is.EqualTo(severity));
            Assert.That(instance.Spotted.ToString("o", CultureInfo.InvariantCulture), Is.EqualTo(creationTime));
            Assert.That(instance.Progress, Is.EqualTo(progress));
            Assert.That(instance.Notes, Is.EqualTo(notes));
        });
    }

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb", "71009336-9133-47c8-b577-d755c8c371ee")]
    public async Task GetAllAsync_JsonIncludesMultipleInstances_AllInstancesReturnedWithCorrectProperties(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Has.Length.EqualTo(4));
        Assert.That(instances.Select(instance => instance.Id.ToString()), Is.EqualTo(new[] { id1, id2, id3, id4 }));
    }

    [Test]
    public async Task GetAllAsync_JsonIncludesNoInstances_EmptyListReturned()
    {
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Is.Empty);
    }

    [Test]
    public void GetAllAsync_ReadFromFile_JsonIncludesValidData_DoesNotThrow()
    {
        using var dataManager = PollutedLocationFileRepository.FromFile(m_mapper, ValidDataSourcePath);
        Assert.DoesNotThrowAsync(async () => await dataManager.GetAllAsync());
    }

    [Test]
    public void GetAllAsync_ReadFromFile_JsonIncludesInvalidData_Throws()
    {
        using var dataManager = PollutedLocationFileRepository.FromFile(m_mapper, InvalidDataSourcePath);
        Assert.ThrowsAsync<JsonException>(async () => await dataManager.GetAllAsync());
    }

    [Test]
    [TestCase(-1.5, 1.5, 2.5, -2.5, -1.0, 1.0)]
    [TestCase(-41.21341, 44.44444, 81.49102, -89.149102, -28.97782, 29.58192)]
    public async Task GetAllAsyncOrdered_ReadFromJson_InstancesReturnedInAscendingOrderByDistance(double longitude1, double latitude1,
                                                                                                  double longitude2, double latitude2,
                                                                                                  double longitude3, double latitude3)
    {
        var id1 = Guid.NewGuid().ToString();
        var id2 = Guid.NewGuid().ToString();
        var id3 = Guid.NewGuid().ToString();

        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"," +
            "\"coordinates\":" +
            $"{{\"longitude\":{longitude1.ToString(CultureInfo.InvariantCulture)},\"latitude\":{latitude1.ToString(CultureInfo.InvariantCulture)}" +
            "}}," +
            $"{{\"id\":\"{id2}\"," +
            "\"coordinates\":" +
            $"{{\"longitude\":{longitude2.ToString(CultureInfo.InvariantCulture)},\"latitude\":{latitude2.ToString(CultureInfo.InvariantCulture)}" +
            "}}," +
            $"{{\"id\":\"{id3}\"," +
            "\"coordinates\":" +
            $"{{\"longitude\":{longitude3.ToString(CultureInfo.InvariantCulture)},\"latitude\":{latitude3.ToString(CultureInfo.InvariantCulture)}" +
            "}}," +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);

        var referenceCoordinates = new Coordinates
        {
            Latitude = 0,
            Longitude = 0
        };

        var instances = (await dataManager.GetAllAsync(referenceCoordinates)).ToArray();

        Assert.That(instances, Has.Length.EqualTo(3));
        Assert.That(instances.Select(x => x.Id.ToString()), Is.EqualTo(new[] { id3, id1, id2 }));
    }
    #endregion

    #region GetByPropertyAsync tests
    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb", "71009336-9133-47c8-b577-d755c8c371ee")]
    public async Task GetByPropertyAsync_JsonIncludesTheRequiredInstance_ReturnsInstance(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);

        var requestId = id2;

        var instance = await dataManager.GetByPropertyAsync(x => x.Id.ToString() == requestId);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.Id.ToString(), Is.EqualTo(requestId));
    }

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb", "71009336-9133-47c8-b577-d755c8c371ee")]
    public async Task GetByPropertyAsync_NoInstanceWithRequestedIdFound_ReturnsNull(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);

        var requestId = Guid.NewGuid().ToString();

        var instance = await dataManager.GetByPropertyAsync(x => x.Id.ToString() == requestId);

        Assert.That(instance, Is.Null);
    }

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb")]
    public async Task GetByPropertyAsync_SeveralInstanceWithRequestedIdFound_FirstMatchingInstanceReturned(string id1, string id2, string id3)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\", \"radius\":{1}}}," +
            $"{{\"id\":\"{id2}\", \"radius\":{2}}}," +
            $"{{\"id\":\"{id3}\", \"radius\":{3}}}," +
            $"{{\"id\":\"{id2}\", \"radius\":{4}}}" +
            "]";
        using var dataManager = PollutedLocationFileRepository.FromContent(m_mapper, jsonString);
        var requestId = id2;

        var instance = await dataManager.GetByPropertyAsync(x => x.Id.ToString() == requestId);

        Assert.That(instance!.Radius, Is.EqualTo(2));
    }
    #endregion
}