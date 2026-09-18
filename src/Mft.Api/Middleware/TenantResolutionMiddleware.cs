using Mft.Infrastructure.Identity;
using Mft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

namespace Mft.Api.Middleware;

/// <summary>
/// Runs after JWT authentication. Resolves the caller's Entra identity (tid/oid) to an
/// internal AppUser (which determines their Organization) and loads their effective
/// roles/permissions into the request-scoped CurrentContextAccessor. Handles the
/// admin-invite "claim on first login" flow: if no user is claimed yet for this identity,
/// tries to match an invited-but-unclaimed row by email and claims it.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, MftDbContext db, CurrentContextAccessor accessor)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var objectId = context.User.GetObjectId();
            var tenantId = context.User.GetTenantId();

            if (!string.IsNullOrEmpty(objectId) && !string.IsNullOrEmpty(tenantId))
            {
                var user = await db.AppUsers.IgnoreQueryFilters()
                    .Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role).ThenInclude(r => r!.RolePermissions).ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.EntraTenantId == tenantId && u.EntraObjectId == objectId, context.RequestAborted);

                if (user is null)
                {
                    var email = GetEmailClaim(context.User);
                    if (!string.IsNullOrEmpty(email))
                    {
                        user = await db.AppUsers.IgnoreQueryFilters()
                            .Include(u => u.RoleAssignments).ThenInclude(ra => ra.Role).ThenInclude(r => r!.RolePermissions).ThenInclude(rp => rp.Permission)
                            .FirstOrDefaultAsync(u => u.EntraObjectId == null && u.Email == email, context.RequestAborted);

                        if (user is not null)
                        {
                            user.EntraObjectId = objectId;
                            user.EntraTenantId = tenantId;
                            await db.SaveChangesAsync(context.RequestAborted);
                        }
                    }
                }

                if (user is not null && user.IsActive)
                {
                    if (user.LastLoginAtUtc is null || DateTime.UtcNow - user.LastLoginAtUtc.Value > TimeSpan.FromMinutes(30))
                    {
                        user.LastLoginAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync(context.RequestAborted);
                    }

                    accessor.OrganizationId = user.OrganizationId;
                    accessor.UserId = user.Id;
                    accessor.Email = user.Email;
                    accessor.DisplayName = user.DisplayName;
                    accessor.RoleSet = user.RoleAssignments.Select(ra => ra.Role!.Name).ToHashSet();
                    accessor.PermissionSet = user.RoleAssignments
                        .SelectMany(ra => ra.Role!.RolePermissions.Select(rp => rp.Permission!.Key))
                        .ToHashSet();
                    accessor.IsAuthenticatedAndProvisioned = true;
                }
            }
        }

        await _next(context);
    }

    private static string? GetEmailClaim(System.Security.Claims.ClaimsPrincipal principal)
    {
        return principal.FindFirst("preferred_username")?.Value
            ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Upn)?.Value
            ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? principal.FindFirst("emails")?.Value;
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
        => app.UseMiddleware<TenantResolutionMiddleware>();
}
