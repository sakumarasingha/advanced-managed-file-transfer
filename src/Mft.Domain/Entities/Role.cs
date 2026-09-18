namespace Mft.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemDefined { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<RolePermission> RolePermissions { get; set; } = [];
    public List<RoleAssignment> RoleAssignments { get; set; } = [];
}
