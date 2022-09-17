using Apsitvarkom.DataAccess;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.UnitTests.DataAccess;

public class PollutedLocationsFileStoreTests
{
    [Test]
    [TestCase("9719d4ef-5cde-4370-a510-53af84bdede2", -181.12311, LocationSeverityLevel.High, "2015-05-16T05:50:06", 100)]
    public void GetAllPollutedLocationsDTO_SomePropertiesMissing_DeserializingSuccessful_MissingPropertiesSetToNull(string id, double longitude, LocationSeverityLevel severity, string creationTime, int progress)
    {
        // No radius, latitude and notes passed.
        var jsonString =
            "[" +
            "{" +
            $"\"id\":\"{id}\"," +
            "\"location\":" +
            "{" +
            $"\"longitude\":{longitude}," +
            "}," +
            $"\"severity\":\"{severity}\"," +
            $"\"spotted\":\"{creationTime}\"," +
            $"\"progress\":{progress}," +
            "}" +
            "]";
        var dataManager = PollutedLocationsDTOFileStore.FromContent(jsonString);

        var instances = dataManager.GetAllPollutedLocations().ToArray();

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
    public void GetAllPollutedLocationsDTO_DeserializingOfAllPropertiesSuccessful_OneInstanceReturned(string id, double longitude, double latitude, int radius, LocationSeverityLevel severity, string creationTime, int progress, string notes)
    {
        var jsonString =
            "[" +
            "{" +
            $"\"id\":\"{id}\"," +
            "\"location\":" +
            "{" +
            $"\"longitude\":{longitude}," +
            $"\"latitude\":{latitude}" +
            "}," +
            $"\"radius\":{radius}," +
            $"\"severity\":\"{severity}\"," +
            $"\"spotted\":\"{creationTime}\"," +
            $"\"progress\":{progress}," +
            $"\"notes\":\"{notes}\"" +
            "}" +
            "]";
        var dataManager = PollutedLocationsDTOFileStore.FromContent(jsonString);

        var instances = dataManager.GetAllPollutedLocations().ToArray();

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
    public void GetAllPollutedLocationsDTO_JsonIncludesMultipleInstances_AllInstancesReturnedWithCorrectProperties(string id1, string id2, string id3, string id4)
    {
        var jsonString =
            "[" +
            $"{{\"id\":\"{id1}\"}}," +
            $"{{\"id\":\"{id2}\"}}," +
            $"{{\"id\":\"{id3}\"}}," +
            $"{{\"id\":\"{id4}\"}}" +
            "]";
        var dataManager = PollutedLocationsDTOFileStore.FromContent(jsonString);

        var instances = dataManager.GetAllPollutedLocations().ToArray();

        Assert.That(instances, Has.Length.EqualTo(4));
        Assert.That(instances.Select(instance => instance.Id?.ToString()), Is.EqualTo(new[] { id1, id2, id3, id4 }));
    }
}