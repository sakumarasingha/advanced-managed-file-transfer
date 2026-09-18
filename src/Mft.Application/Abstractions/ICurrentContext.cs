namespace Mft.Application.Abstractions;

public interface ICurrentOrganizationContext
{
    Guid? OrganizationId { get; }
}

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    string? DisplayName { get; }
    IReadOnlySet<string> Permissions { get; }
    IReadOnlySet<string> Roles { get; }
    bool IsAuthenticatedAndProvisioned { get; }
}
