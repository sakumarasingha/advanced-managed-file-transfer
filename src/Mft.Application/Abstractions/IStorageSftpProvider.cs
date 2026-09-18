using Mft.Domain.Enums;

namespace Mft.Application.Abstractions;

public record SshPublicKeyDto(string PublicKeyOpenSsh, string Fingerprint);

public record PermissionScopeDto(string ContainerName, string PathPrefix, SftpPermissions Permissions);

public record ContainerProvisionResult(bool Succeeded, string? ErrorMessage);

public record LocalUserProvisionResult(bool Succeeded, string? ExternalLocalUserId, string? ErrorMessage);

public record ConnectionTestResult(bool Succeeded, string Message);

public interface IStorageSftpProvider
{
    Task<ContainerProvisionResult> EnsureContainerSftpEnabledAsync(string containerName, CancellationToken ct = default);

    Task<LocalUserProvisionResult> CreateOrUpdateLocalUserAsync(
        string username,
        IEnumerable<SshPublicKeyDto> sshKeys,
        bool passwordAuthEnabled,
        IEnumerable<PermissionScopeDto> permissionScopes,
        CancellationToken ct = default);

    Task DeleteLocalUserAsync(string username, CancellationToken ct = default);

    Task<IReadOnlyList<string>> ListContainersAsync(CancellationToken ct = default);

    Task SetContainerPermissionsAsync(string username, IEnumerable<PermissionScopeDto> scopes, CancellationToken ct = default);

    Task<ConnectionTestResult> TestConnectionAsync(CancellationToken ct = default);
}
