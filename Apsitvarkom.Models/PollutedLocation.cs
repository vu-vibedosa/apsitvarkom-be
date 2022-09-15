using System.Text.Json.Serialization;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.Models
{
    /// <summary>
    /// Class for storing captured polluted location records.
    /// </summary>
    public class PollutedLocation
    {
        /// <summary>Unique identifier of the given record.</summary>
        public string Id { get; init; }

        /// <summary>Location of the given record.</summary>
        public Location Location { get; set; }

        /// <summary>Rough size estimate of the given record in meters.</summary>
        public int Radius { get; set; }

        /// <summary>Estimated current pollution level of the record.</summary>
        public LocationSeverityLevel Severity { get; set; }

        /// <summary><see cref="DateTime"/> of when the record was created.</summary>
        public DateTime Spotted { get; init; }

        /// <summary>Current progress of the location's cleaning process in percentages.</summary>
        public int Progress { get; set; }

        /// <summary>Additional information about the record.</summary>
        public string Notes { get; set; }

        /// <summary>Constructor of a new polluted location record.</summary>
        /// <param name="latitude">Location of the record.</param>
        /// <param name="longitude">Location of the record.</param>
        /// <param name="radius">Size of the recorded location.</param>
        /// <param name="severity">Current levels of trash in the record site.</param>
        /// <param name="notes">Additional information about the record.</param>
        public PollutedLocation(double latitude, double longitude, int radius, int severity, string notes)
        {
            Id = Guid.NewGuid().ToString();
            Location = new Location(latitude, longitude);
            Radius = radius;
            Severity = (LocationSeverityLevel)severity;
            Progress = 0;
            Spotted = DateTime.Now;
            Notes = notes;
        }

        /// <summary>Constructor of an existing polluted location record.</summary>
        /// <param name="id">Unique identifier of the record.</param>
        /// <param name="location">Location of the record.</param>
        /// <param name="radius">Size of the recorded location.</param>
        /// <param name="severity">Current levels of trash in the record site.</param>
        /// <param name="progress">Current progress of the cleaning process of the record site.</param>
        /// <param name="spotted"><see cref="DateTime"/> when the location was recorded.</param>
        /// <param name="notes">Additional information about the record.</param>
        [JsonConstructor]
        public PollutedLocation(string id, Location location, int radius, LocationSeverityLevel severity, int progress, DateTime spotted, string notes)
        {
            Id = id;
            Location = location;
            Radius = radius;
            Severity = severity;
            Progress = progress;
            Spotted = spotted;
            Notes = notes;
        }
    }
}