using Mft.Domain.Common;

namespace Mft.Domain.Entities;

public class AppUser : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string? EntraObjectId { get; set; }
    public string? EntraTenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTime? InvitedAtUtc { get; set; }
    public Guid? InvitedByUserId { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<RoleAssignment> RoleAssignments { get; set; } = [];
}
