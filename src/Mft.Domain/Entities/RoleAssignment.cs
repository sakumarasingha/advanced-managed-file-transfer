using Mft.Domain.Common;

namespace Mft.Domain.Entities;

public class RoleAssignment : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? AssignedByUserId { get; set; }
}
