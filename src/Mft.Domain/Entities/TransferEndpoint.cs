using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

/// <summary>
/// A named, reusable source/target/archive/error location a TransferRoute reads from or writes
/// to. Shape depends on Type: BlobStorage/AzureSftp use Container+FolderPath (AzureSftp
/// optionally tying back to the SftpLocalUser whose home directory it is); ExternalSftp uses
/// ExternalSftpConnection+RemotePath.
/// </summary>
public class TransferEndpoint : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public TransferEndpointType Type { get; set; } = TransferEndpointType.BlobStorage;

    // BlobStorage / AzureSftp
    public string? Container { get; set; }
    public string? FolderPath { get; set; }
    public Guid? SftpLocalUserId { get; set; }
    public SftpLocalUser? SftpLocalUser { get; set; }

    // ExternalSftp
    public Guid? ExternalSftpConnectionId { get; set; }
    public ExternalSftpConnection? ExternalSftpConnection { get; set; }
    public string? RemotePath { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
