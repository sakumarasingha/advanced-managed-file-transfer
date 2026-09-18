using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

/// <summary>
/// A reusable connection profile for a third-party SFTP server we connect out to (push or
/// pull), so credentials are managed once and referenced by any number of TransferEndpoints.
/// </summary>
public class ExternalSftpConnection : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    public string Username { get; set; } = string.Empty;
    public SftpAuthMethods AuthMethod { get; set; } = SftpAuthMethods.Password;

    public byte[]? PasswordEncrypted { get; set; }
    public byte[]? PrivateKeyEncrypted { get; set; }
    public byte[]? PassphraseEncrypted { get; set; }

    public string? HostKeyFingerprint { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }
}
