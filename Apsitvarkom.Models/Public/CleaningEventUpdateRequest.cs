namespace Apsitvarkom.Models.Public;

public class CleaningEventUpdateRequest
{
    public Guid? Id { get; set; }

    public DateTime? StartTime { get; set; }

    public string? Notes { get; set; }
}
