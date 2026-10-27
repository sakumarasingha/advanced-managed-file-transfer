using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Mft.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Mft.Infrastructure.Storage;

/// <summary>
/// Creates the real storage account through Azure Resource Manager, authenticating as the app's managed identity
/// (or the developer's `az login` locally). The identity needs rights to create storage accounts in the target
/// resource group - infra/main.bicep grants Storage Account Contributor on the app's own resource group.
/// Registered when Storage:AccountProvisioning is Azure.
/// </summary>
public class AzureStorageAccountProvisioner : IStorageAccountProvisioner
{
    private readonly ILogger<AzureStorageAccountProvisioner> _logger;

    public AzureStorageAccountProvisioner(ILogger<AzureStorageAccountProvisioner> logger)
    {
        _logger = logger;
    }

    public async Task<StorageAccountProvisionResult> ProvisionAsync(StorageAccountProvisionRequest request, CancellationToken ct = default)
    {
        try
        {
            var arm = new ArmClient(new DefaultAzureCredential(), request.SubscriptionId);
            var subscription = arm.GetSubscriptionResource(SubscriptionResource.CreateResourceIdentifier(request.SubscriptionId));
            var resourceGroup = (await subscription.GetResourceGroupAsync(request.ResourceGroup, ct)).Value;
            var accounts = resourceGroup.GetStorageAccounts();

            if (await accounts.ExistsAsync(request.StorageAccountName, cancellationToken: ct))
            {
                // Only the SFTP switch is ours to change; hierarchical namespace and SKU are fixed at creation.
                var existing = (await accounts.GetAsync(request.StorageAccountName, cancellationToken: ct)).Value;
                await existing.UpdateAsync(new StorageAccountPatch { IsSftpEnabled = request.SftpEnabled }, ct);

                _logger.LogInformation("Updated storage account {Account} (SFTP enabled: {Sftp})", request.StorageAccountName, request.SftpEnabled);
                return new StorageAccountProvisionResult(true, false,
                    $"Storage account '{request.StorageAccountName}' already exists - SFTP is now {(request.SftpEnabled ? "enabled" : "disabled")}.");
            }

            // No up-front name-availability check: that is a subscription-level action, and this identity only
            // has rights on the resource group. A taken name surfaces from the create call instead.
            var content = new StorageAccountCreateOrUpdateContent(
                new StorageSku(StorageSkuName.StandardLrs), StorageKind.StorageV2, resourceGroup.Data.Location)
            {
                IsHnsEnabled = true, // required for SFTP
                IsSftpEnabled = request.SftpEnabled,
                MinimumTlsVersion = StorageMinimumTlsVersion.Tls1_2,
                AllowBlobPublicAccess = false,
                EnableHttpsTrafficOnly = true,
            };
            await accounts.CreateOrUpdateAsync(WaitUntil.Completed, request.StorageAccountName, content, ct);

            _logger.LogInformation("Created storage account {Account} in {ResourceGroup}", request.StorageAccountName, request.ResourceGroup);
            return new StorageAccountProvisionResult(true, true,
                $"Created storage account '{request.StorageAccountName}' in '{request.ResourceGroup}' (SFTP {(request.SftpEnabled ? "enabled" : "disabled")}).");
        }
        catch (RequestFailedException ex)
        {
            _logger.LogWarning(ex, "Provisioning storage account {Account} failed: {Code}", request.StorageAccountName, ex.ErrorCode);
            return new StorageAccountProvisionResult(false, false, Describe(ex, request));
        }
        catch (AuthenticationFailedException ex)
        {
            _logger.LogWarning(ex, "Azure authentication failed while provisioning {Account}", request.StorageAccountName);
            return new StorageAccountProvisionResult(false, false,
                "The server couldn't authenticate to Azure. Check that the app's managed identity is enabled.");
        }
    }

    private static string Describe(RequestFailedException ex, StorageAccountProvisionRequest request) => (ex.ErrorCode, ex.Status) switch
    {
        ("StorageAccountAlreadyTaken", _) =>
            $"The storage account name '{request.StorageAccountName}' is already taken by another Azure customer. Choose a different name.",
        ("AccountNameInvalid", _) =>
            $"'{request.StorageAccountName}' isn't a valid storage account name (3-24 characters, lowercase letters and numbers).",
        (_, 403) => $"The server's identity isn't allowed to create storage accounts in resource group '{request.ResourceGroup}'. " +
               "Grant it the Storage Account Contributor role there.",
        (_, 404) => $"Resource group '{request.ResourceGroup}' (or the subscription) wasn't found. Create the resource group first.",
        _ => $"Azure rejected the request ({ex.ErrorCode ?? ex.Status.ToString()}): {ex.Message}",
    };
}
