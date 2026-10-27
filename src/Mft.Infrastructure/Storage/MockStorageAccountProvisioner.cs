using Mft.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Mft.Infrastructure.Storage;

/// <summary>
/// Pretends to provision a storage account. Registered when Storage:AccountProvisioning is Mock (the default),
/// so local dev never creates real Azure resources.
/// </summary>
public class MockStorageAccountProvisioner : IStorageAccountProvisioner
{
    private readonly ILogger<MockStorageAccountProvisioner> _logger;

    public MockStorageAccountProvisioner(ILogger<MockStorageAccountProvisioner> logger)
    {
        _logger = logger;
    }

    public async Task<StorageAccountProvisionResult> ProvisionAsync(StorageAccountProvisionRequest request, CancellationToken ct = default)
    {
        await Task.Delay(150, ct);
        _logger.LogInformation(
            "[Mock] Provisioned storage account {Account} in {ResourceGroup} (SFTP enabled: {Sftp})",
            request.StorageAccountName, request.ResourceGroup, request.SftpEnabled);
        return new StorageAccountProvisionResult(true, true, "Mock provisioning succeeded - no real Azure resources were created.");
    }
}
