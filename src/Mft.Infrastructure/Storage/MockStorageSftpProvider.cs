using Mft.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Mft.Infrastructure.Storage;

/// <summary>
/// Simulates Azure Storage SFTP local-user provisioning for local dev / M1, before a real
/// Azure subscription is wired up. Registered when Storage:Mode=Mock (the default).
/// A username containing "failtest" deliberately fails, so the UI's error path is exercisable.
/// </summary>
public class MockStorageSftpProvider : IStorageSftpProvider
{
    private readonly ILogger<MockStorageSftpProvider> _logger;

    public MockStorageSftpProvider(ILogger<MockStorageSftpProvider> logger)
    {
        _logger = logger;
    }

    public async Task<ContainerProvisionResult> EnsureContainerSftpEnabledAsync(string containerName, CancellationToken ct = default)
    {
        await SimulateLatency(ct);
        _logger.LogInformation("[Mock] Ensured container {Container} exists with SFTP enabled", containerName);
        return new ContainerProvisionResult(true, null);
    }

    public async Task<LocalUserProvisionResult> CreateOrUpdateLocalUserAsync(
        string username,
        IEnumerable<SshPublicKeyDto> sshKeys,
        bool passwordAuthEnabled,
        IEnumerable<PermissionScopeDto> permissionScopes,
        CancellationToken ct = default)
    {
        await SimulateLatency(ct);

        if (username.Contains("failtest", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("[Mock] Forced failure provisioning local user {Username}", username);
            return new LocalUserProvisionResult(false, null, "Simulated provisioning failure (username contained 'failtest').");
        }

        var externalId = $"mock-{Guid.NewGuid():N}";
        _logger.LogInformation("[Mock] Provisioned SFTP local user {Username} as {ExternalId}", username, externalId);
        return new LocalUserProvisionResult(true, externalId, null);
    }

    public async Task DeleteLocalUserAsync(string username, CancellationToken ct = default)
    {
        await SimulateLatency(ct);
        _logger.LogInformation("[Mock] Deleted SFTP local user {Username}", username);
    }

    public async Task<IReadOnlyList<string>> ListContainersAsync(CancellationToken ct = default)
    {
        await SimulateLatency(ct);
        return ["incoming", "outgoing", "archive", "errors"];
    }

    public async Task SetContainerPermissionsAsync(string username, IEnumerable<PermissionScopeDto> scopes, CancellationToken ct = default)
    {
        await SimulateLatency(ct);
        _logger.LogInformation("[Mock] Updated container permissions for {Username}", username);
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(CancellationToken ct = default)
    {
        await SimulateLatency(ct);
        return new ConnectionTestResult(true, "Mock connection succeeded (no real Azure Storage account configured yet).");
    }

    private static Task SimulateLatency(CancellationToken ct) => Task.Delay(150, ct);
}
