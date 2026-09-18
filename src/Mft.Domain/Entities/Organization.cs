namespace Mft.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string TimeZoneId { get; set; } = "UTC";
    public string? PlanTier { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
