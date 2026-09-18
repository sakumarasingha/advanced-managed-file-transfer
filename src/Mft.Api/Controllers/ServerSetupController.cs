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

[ApiController]
[Route("api/serversetup")]
[Authorize]
public class ServerSetupController : ControllerBase
{
    private readonly MftDbContext _db;
    private readonly IStorageSftpProvider _storageSftpProvider;
    private readonly ICurrentOrganizationContext _orgContext;

    public ServerSetupController(MftDbContext db, IStorageSftpProvider storageSftpProvider, ICurrentOrganizationContext orgContext)
    {
        _db = db;
        _storageSftpProvider = storageSftpProvider;
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

    private static StorageAccountSettingsResponse Map(Domain.Entities.StorageAccountSettings s) => new(
        s.Id, s.StorageAccountName, s.ResourceGroup, s.SubscriptionId, s.SftpEnabled,
        s.LastTestedAtUtc, s.LastTestSucceeded, s.LastTestMessage);
}
