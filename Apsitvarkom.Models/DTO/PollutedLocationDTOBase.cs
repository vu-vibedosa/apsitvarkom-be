namespace Apsitvarkom.Models.DTO
{
    public class PollutedLocationDTOBase
    {
        public LocationDTO? Location { get; set; }

        public int? Radius { get; set; }

        public string? Severity { get; set; }

        public int? Progress { get; set; }

        public string? Notes { get; set; }
    }
}
