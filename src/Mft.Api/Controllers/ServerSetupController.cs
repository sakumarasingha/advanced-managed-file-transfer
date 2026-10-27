using System.Text.RegularExpressions;
using Mft.Application.Abstractions;
using Mft.Application.Common;
using Mft.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mft.Api.Controllers;

public record StorageAccountSettingsResponse(
    Guid Id,
    string? StorageAccountName,
    string? ResourceGroup,
    string? SubscriptionId,
    bool SftpEnabled,
    DateTime? LastTestedAtUtc,
    bool? LastTestSucceeded,
    string? LastTestMessage);

public record UpdateStorageAccountSettingsRequest(
    string? StorageAccountName,
    string? ResourceGroup,
    string? SubscriptionId,
    bool SftpEnabled);

public record TestConnectionResponse(bool Succeeded, string Message);

public record ProvisionStorageAccountResponse(bool Succeeded, bool Created, string Message);

[ApiController]
[Route("api/serversetup")]
[Authorize]
public partial class ServerSetupController : ControllerBase
{
    private readonly MftDbContext _db;
    private readonly IStorageSftpProvider _storageSftpProvider;
    private readonly IStorageAccountProvisioner _provisioner;
    private readonly ICurrentOrganizationContext _orgContext;

    public ServerSetupController(
        MftDbContext db,
        IStorageSftpProvider storageSftpProvider,
        IStorageAccountProvisioner provisioner,
        ICurrentOrganizationContext orgContext)
    {
        _db = db;
        _storageSftpProvider = storageSftpProvider;
        _provisioner = provisioner;
        _orgContext = orgContext;
    }

    [HttpGet("storage-account")]
    public async Task<ActionResult<StorageAccountSettingsResponse>> Get(CancellationToken ct)
    {
        var settings = await GetOrCreateSettingsAsync(ct);
        return Map(settings);
    }

    [HttpPut("storage-account")]
    [Authorize(Policy = PermissionKeys.ServerSetupManage)]
    public async Task<ActionResult<StorageAccountSettingsResponse>> Update(UpdateStorageAccountSettingsRequest request, CancellationToken ct)
    {
        var settings = await GetOrCreateSettingsAsync(ct);

        settings.StorageAccountName = request.StorageAccountName;
        settings.ResourceGroup = request.ResourceGroup;
        settings.SubscriptionId = request.SubscriptionId;
        settings.SftpEnabled = request.SftpEnabled;
        settings.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(settings);
    }

    /// <summary>
    /// Creates (or updates) the real Azure Storage account described by the saved settings.
    /// </summary>
    [HttpPost("provision")]
    [Authorize(Policy = PermissionKeys.ServerSetupManage)]
    public async Task<ActionResult<ProvisionStorageAccountResponse>> Provision(CancellationToken ct)
    {
        var settings = await GetOrCreateSettingsAsync(ct);

        var problem = ValidateForProvisioning(settings);
        if (problem is not null)
        {
            return BadRequest(new ProvisionStorageAccountResponse(false, false, problem));
        }

        var result = await _provisioner.ProvisionAsync(
            new StorageAccountProvisionRequest(
                settings.SubscriptionId!, settings.ResourceGroup!, settings.StorageAccountName!, settings.SftpEnabled),
            ct);

        return new ProvisionStorageAccountResponse(result.Succeeded, result.Created, result.Message);
    }

    [HttpPost("test-connection")]
    [Authorize(Policy = PermissionKeys.ServerSetupManage)]
    public async Task<ActionResult<TestConnectionResponse>> TestConnection(CancellationToken ct)
    {
        var result = await _storageSftpProvider.TestConnectionAsync(ct);

        var settings = await GetOrCreateSettingsAsync(ct);
        settings.LastTestedAtUtc = DateTime.UtcNow;
        settings.LastTestSucceeded = result.Succeeded;
        settings.LastTestMessage = result.Message;
        await _db.SaveChangesAsync(ct);

        return new TestConnectionResponse(result.Succeeded, result.Message);
    }

    [HttpGet("containers")]
    public async Task<ActionResult<IReadOnlyList<string>>> ListContainers(CancellationToken ct)
    {
        var containers = await _storageSftpProvider.ListContainersAsync(ct);
        return Ok(containers);
    }

    private async Task<Domain.Entities.StorageAccountSettings> GetOrCreateSettingsAsync(CancellationToken ct)
    {
        var settings = await _db.StorageAccountSettings.FirstOrDefaultAsync(ct);
        if (settings is not null)
        {
            return settings;
        }

        settings = new Domain.Entities.StorageAccountSettings
        {
            Id = Guid.NewGuid(),
            OrganizationId = _orgContext.OrganizationId!.Value
        };
        _db.StorageAccountSettings.Add(settings);
        await _db.SaveChangesAsync(ct);
        return settings;
    }

    private static string? ValidateForProvisioning(Domain.Entities.StorageAccountSettings s)
    {
        if (string.IsNullOrWhiteSpace(s.StorageAccountName) || !StorageAccountNameRegex().IsMatch(s.StorageAccountName))
        {
            return "Storage account name must be 3-24 characters, lowercase letters and numbers only.";
        }

        if (!Guid.TryParse(s.SubscriptionId, out _))
        {
            return "Subscription ID must be a valid GUID.";
        }

        if (string.IsNullOrWhiteSpace(s.ResourceGroup) || !ResourceGroupRegex().IsMatch(s.ResourceGroup))
        {
            return "Resource group is required (letters, numbers, '-', '_', '.', '(' and ')', up to 90 characters).";
        }

        return null;
    }

    [GeneratedRegex("^[a-z0-9]{3,24}$")]
    private static partial Regex StorageAccountNameRegex();

    [GeneratedRegex(@"^[\w\-\.\(\)]{1,90}$")]
    private static partial Regex ResourceGroupRegex();

    private static StorageAccountSettingsResponse Map(Domain.Entities.StorageAccountSettings s) => new(
        s.Id, s.StorageAccountName, s.ResourceGroup, s.SubscriptionId, s.SftpEnabled,
        s.LastTestedAtUtc, s.LastTestSucceeded, s.LastTestMessage);
}
