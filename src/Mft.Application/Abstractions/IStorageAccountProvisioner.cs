namespace Mft.Application.Abstractions;

public record StorageAccountProvisionRequest(
    string SubscriptionId,
    string ResourceGroup,
    string StorageAccountName,
    bool SftpEnabled);

/// <param name="Created">True when a new account was created, false when an existing one was updated.</param>
public record StorageAccountProvisionResult(bool Succeeded, bool Created, string Message);

/// <summary>
/// Creates (or updates) the Azure Storage account that acts as the organization's SFTP server.
/// </summary>
public interface IStorageAccountProvisioner
{
    Task<StorageAccountProvisionResult> ProvisionAsync(StorageAccountProvisionRequest request, CancellationToken ct = default);
}
