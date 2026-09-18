using Mft.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace Mft.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserContext _currentUser;

    public PermissionAuthorizationHandler(ICurrentUserContext currentUser)
    {
        _currentUser = currentUser;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (_currentUser.IsAuthenticatedAndProvisioned && _currentUser.Permissions.Contains(requirement.PermissionKey))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
