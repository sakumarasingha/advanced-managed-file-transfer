using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

public class SftpLocalUser : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Username { get; set; } = string.Empty;
    public string HomeContainer { get; set; } = string.Empty;
    public string? HomeDirectory { get; set; }
    public SftpAuthMethods AuthMethods { get; set; } = SftpAuthMethods.SshKey;
    public bool HasPasswordSet { get; set; }
    public SftpUserStatus Status { get; set; } = SftpUserStatus.Active;
    public ProvisioningStatus ProvisioningStatus { get; set; } = ProvisioningStatus.Pending;
    public string? ExternalLocalUserId { get; set; }
    public string? LastProvisioningError { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }

    public List<SftpUserSshKey> SshKeys { get; set; } = [];
    public List<SftpUserPermission> Permissions { get; set; } = [];
}

public class SftpUserSshKey : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid SftpLocalUserId { get; set; }
    public SftpLocalUser? SftpLocalUser { get; set; }

    public string PublicKeyOpenSsh { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public string? Label { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }
}

public class SftpUserPermission : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid SftpLocalUserId { get; set; }
    public SftpLocalUser? SftpLocalUser { get; set; }

    public string ContainerName { get; set; } = string.Empty;
    public string PathPrefix { get; set; } = string.Empty;
    public SftpPermissions Permissions { get; set; } = SftpPermissions.None;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
