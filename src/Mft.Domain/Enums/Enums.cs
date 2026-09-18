namespace Mft.Domain.Enums;

public enum SftpAuthMethods
{
    Password = 1,
    SshKey = 2,
    Both = 3
}

public enum SftpUserStatus
{
    Active = 0,
    Disabled = 1
}

public enum ProvisioningStatus
{
    Pending = 0,
    Provisioned = 1,
    Failed = 2,
    PendingDelete = 3,
    Deleted = 4
}

[Flags]
public enum SftpPermissions
{
    None = 0,
    Read = 1,
    Write = 2,
    List = 4,
    Create = 8,
    Delete = 16
}

public enum ScheduleType
{
    IntervalMinutes = 0,
    CronExpression = 1
}

public enum FileTypeResolutionMode
{
    Static = 0,
    ByExtension = 1
}

public enum PgpKeyType
{
    Rsa = 0,
    Curve25519 = 1
}

public enum PgpMode
{
    Classic = 0,
    Aead = 1
}

public enum TransferStatus
{
    Pending = 0,
    InProgress = 1,
    Succeeded = 2,
    Failed = 3,
    SkippedTooLarge = 4,
    Retrying = 5
}

public enum TransferTrigger
{
    Scheduled = 0,
    Manual = 1,
    Retry = 2
}

/// <summary>
/// The kind of endpoint a TransferRoute reads from or writes to. BlobStorage is a plain
/// container/path in our own storage account, accessed directly via the Blob API. AzureSftp
/// is our own storage account's SFTP surface (an SftpLocalUser's home directory) - used when a
/// route should pick up what a client dropped via SFTP, or make a file available for a client
/// to pull via their SFTP login. ExternalSftp is a third-party partner's SFTP server we connect
/// out to (push or pull) via a stored ExternalSftpConnection.
/// </summary>
public enum TransferEndpointType
{
    BlobStorage = 0,
    AzureSftp = 1,
    ExternalSftp = 2
}

public enum TransformationType
{
    PgpEncrypt = 0,
    PgpDecrypt = 1
}
