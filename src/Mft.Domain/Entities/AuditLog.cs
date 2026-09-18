namespace Mft.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? ActorUserId { get; set; }

    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string? DetailsJson { get; set; }
    public string? IpAddress { get; set; }
}
