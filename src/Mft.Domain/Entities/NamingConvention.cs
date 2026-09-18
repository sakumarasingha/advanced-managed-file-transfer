using Mft.Domain.Common;

namespace Mft.Domain.Entities;

public class NamingConvention : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PathTemplate { get; set; } = "{Container}/{DestinationFolder}/{FileType}";
    public bool IsDefault { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
