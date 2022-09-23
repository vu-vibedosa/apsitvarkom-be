﻿using System.Globalization;
using System.Text.Json;
using Apsitvarkom.DataAccess;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.DataAccess;

public class PollutedLocationDTOFileRepositoryTests
{
    // Existing mock data file, containing invalid json data.
    private static readonly string InvalidDataSourcePath = Path.Combine("DataAccess", "PollutedLocationDTOMockInvalid.json");

    // Existing mock data file, containing three valid instances with unique property values.
    private static readonly string ValidDataSourcePath = Path.Combine("DataAccess", "PollutedLocationDTOMockValid.json");

    #region GetAllAsync tests
    [Test]
    [TestCase("9719d4ef-5cde-4370-a510-53af84bdede2", -181.12311, LocationSeverityLevel.High, "2015-05-16T05:50:06", 100)]
    public async Task GetAllAsync_SomePropertiesMissing_DeserializingSuccessful_MissingPropertiesSetToNull(string id, double longitude, LocationSeverityLevel severity, string creationTime, int progress)
    {
        // No radius, latitude and notes passed.
        var jsonString =
            "[" +
            "{" +
            $"\"id\":\"{id}\"," +
            "\"location\":" +
            "{" +
            $"\"longitude\":{longitude.ToString(CultureInfo.InvariantCulture)}," +
            "}," +
            $"\"severity\":\"{severity}\"," +
            $"\"spotted\":\"{creationTime}\"," +
            $"\"progress\":{progress}," +
            "}" +
            "]";
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Has.Length.EqualTo(1));

        var instance = instances.Single();
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id, Is.EqualTo(id));
            Assert.That(instance.Location?.Longitude, Is.EqualTo(longitude));
            Assert.That(instance.Location?.Latitude, Is.Null);
            Assert.That(instance.Radius, Is.Null);
            Assert.That(instance.Severity, Is.EqualTo(severity.ToString()));
            Assert.That(instance.Spotted, Is.EqualTo(creationTime));
            Assert.That(instance.Progress, Is.EqualTo(progress));
            Assert.That(instance.Notes, Is.Null);
        });
    }

    [Test]
    [TestCase("5be2354e-2500-4289-bbe2-66210592e17f", -78.948237, 35.929673, 10, LocationSeverityLevel.Moderate, "2022-09-14T17:35:23Z", 16, "Lorem ipsum")]
    [TestCase("c3aca40c-0ed7-4b78-82b1-496d76b5e61f", 87.499999, -159.412971, 1, LocationSeverityLevel.Low, "2015-05-16T05:50:06.7199222-04:00", 0, "")]
    public async Task GetAllAsync_DeserializingOfAllPropertiesSuccessful_OneInstanceReturned(string id, double longitude, double latitude, int radius, LocationSeverityLevel severity, string creationTime, int progress, string notes)
    {
        var jsonString =
            "[" +
            "{" +
            $"\"id\":\"{id}\"," +
            "\"location\":" +
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
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Has.Length.EqualTo(1));

        var instance = instances.Single();
        Assert.Multiple(() =>
        {
            Assert.That(instance.Id, Is.EqualTo(id));
            Assert.That(instance.Location?.Longitude, Is.EqualTo(longitude));
            Assert.That(instance.Location?.Latitude, Is.EqualTo(latitude));
            Assert.That(instance.Radius, Is.EqualTo(radius));
            Assert.That(instance.Severity, Is.EqualTo(severity.ToString()));
            Assert.That(instance.Spotted, Is.EqualTo(creationTime));
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
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Has.Length.EqualTo(4));
        Assert.That(instances.Select(instance => instance.Id?.ToString()), Is.EqualTo(new[] { id1, id2, id3, id4 }));
    }

    [Test]
    public async Task GetAllAsync_JsonIncludesNoInstances_EmptyListReturned()
    {
        var jsonString = "[]";
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var instances = (await dataManager.GetAllAsync()).ToArray();

        Assert.That(instances, Is.Empty);
    }

    [Test]
    public void GetAllAsync_ReadFromFile_JsonIncludesValidData_DoesNotThrow()
    {
        using var dataManager = PollutedLocationDTOFileRepository.FromFile(ValidDataSourcePath);
        Assert.DoesNotThrowAsync(async () => await dataManager.GetAllAsync());
    }

    [Test]
    public void GetAllAsync_ReadFromFile_JsonIncludesInvalidData_Throws()
    {
        using var dataManager = PollutedLocationDTOFileRepository.FromFile(InvalidDataSourcePath);
        Assert.ThrowsAsync<JsonException>(async () => await dataManager.GetAllAsync());
    }

    [Test]
    public void GetAllAsync_ReadFromFile_CouldNotFindSourceFile_Throws()
    {
        var notExistingSourcePath = Guid.NewGuid() + ".json";
        Assert.Throws<FileNotFoundException>(() => PollutedLocationDTOFileRepository.FromFile(notExistingSourcePath));
    }
    #endregion

    #region GetByIdAsync tests

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb", "71009336-9133-47c8-b577-d755c8c371ee")]
    public async Task GetByIdAsync_JsonIncludesTheRequiredInstance_ReturnsInstance(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var requestId = id2;

        var instance = await dataManager.GetByIdAsync(requestId);

        Assert.That(instance, Is.Not.Null);
        Assert.That(instance!.Id, Is.EqualTo(requestId));
    }

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb", "71009336-9133-47c8-b577-d755c8c371ee")]
    public async Task GetByIdAsync_NoInstanceWithRequestedIdFound_ReturnsNull(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var requestId = Guid.NewGuid().ToString();

        var instance = await dataManager.GetByIdAsync(requestId);

        Assert.That(instance, Is.Null);
    }

    [Test]
    [TestCase("b38b9bf6-74f6-4325-8ddf-9defe9bc2994", "3fd9bd2a-90ac-4ae1-baee-3c31b91e91f6", "6ad05412-a6c5-436d-9795-8581b27bfadb")]
    public void GetByIdAsync_SeveralInstanceWithRequestedIdFound_Throws(string id1, string id2, string id3)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id2}\"}}" +
            "]";
        using var dataManager = PollutedLocationDTOFileRepository.FromContent(jsonString);

        var requestId = id2;

       Assert.ThrowsAsync<InvalidOperationException>(async () => await dataManager.GetByIdAsync(requestId));
    }
    #endregion
}