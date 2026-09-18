using Mft.Api.Authorization;
using Mft.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Mft.Api.IntegrationTests;

public class PermissionAuthorizationHandlerTests
{
    private class FakeCurrentUserContext : ICurrentUserContext
    {
        public Guid? UserId => Guid.NewGuid();
        public string? Email => "test@example.com";
        public string? DisplayName => "Test User";
        public IReadOnlySet<string> Permissions { get; init; } = new HashSet<string>();
        public IReadOnlySet<string> Roles { get; init; } = new HashSet<string>();
        public bool IsAuthenticatedAndProvisioned { get; init; } = true;
    }

    [Fact]
    public async Task Succeeds_WhenUserHasPermission()
    {
        var currentUser = new FakeCurrentUserContext { Permissions = new HashSet<string> { "sftpusers.manage" } };
        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement("sftpusers.manage");
        var context = new AuthorizationHandlerContext([requirement], new System.Security.Claims.ClaimsPrincipal(), null);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Fails_WhenUserLacksPermission()
    {
        var currentUser = new FakeCurrentUserContext { Permissions = new HashSet<string>() };
        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement("sftpusers.manage");
        var context = new AuthorizationHandlerContext([requirement], new System.Security.Claims.ClaimsPrincipal(), null);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Fails_WhenNotProvisioned_EvenWithPermission()
    {
        var currentUser = new FakeCurrentUserContext
        {
            Permissions = new HashSet<string> { "sftpusers.manage" },
            IsAuthenticatedAndProvisioned = false
        };
        var handler = new PermissionAuthorizationHandler(currentUser);
        var requirement = new PermissionRequirement("sftpusers.manage");
        var context = new AuthorizationHandlerContext([requirement], new System.Security.Claims.ClaimsPrincipal(), null);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }
}
