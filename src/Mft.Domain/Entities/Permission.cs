namespace Mft.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<RolePermission> RolePermissions { get; set; } = [];
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
