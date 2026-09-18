using Mft.Application.Abstractions;

namespace Mft.Infrastructure.Identity;

/// <summary>
/// Mutable, request-scoped holder for the resolved org/user/permissions.
/// Populated by TenantResolutionMiddleware early in the pipeline (after JWT auth, before
/// controllers run) - MftDbContext reads through this same instance on every query, so
/// updating it mid-request is reflected immediately in later EF Core query filters.
/// </summary>
public class CurrentContextAccessor : ICurrentOrganizationContext, ICurrentUserContext
{
    public Guid? OrganizationId { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public HashSet<string> PermissionSet { get; set; } = [];
    public HashSet<string> RoleSet { get; set; } = [];
    public bool IsAuthenticatedAndProvisioned { get; set; }

    IReadOnlySet<string> ICurrentUserContext.Permissions => PermissionSet;
    IReadOnlySet<string> ICurrentUserContext.Roles => RoleSet;
}
