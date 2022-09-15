namespace Apsitvarkom.Models
{
    /// <summary>
    /// Class for storing locations.
    /// </summary>
    public class Location
    {
        private double _latitude;
        private double _longitude;

        /// <summary>
        /// Latitude of the location. Cannot be less than -90.0 degrees or exceed 90.0 degrees.
        /// </summary>
        public double Latitude 
        { 
            get
            {
                return _latitude;
            } 
            set
            {
                if (value >= -90.0 && value <= 90.0)
                    _latitude = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(Latitude), "Latitude cannot be less than -90.0 or exceed 90.0 degrees.");
            }
        }

        /// <summary>
        /// Longitude of the location. Cannot be less than -180 degrees or exceed 180 degrees.
        /// </summary>
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (value >= -180.0 && value <= 180.0)
                    _longitude = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(Longitude), "Longitude cannot be less than -180.0 or exceed 180.0 degrees.");
            }
        }

        /// <summary>Constructor of location using latitude and longitude.</summary>
        /// <param name="latitude">Latitude of the location.</param>
        /// <param name="longitude">Longitude of the location./param>
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
