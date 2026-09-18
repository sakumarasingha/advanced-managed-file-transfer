using Mft.Application.Common;
using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mft.Infrastructure.Persistence;

/// <summary>
/// Seeds the permission catalog, one bootstrap Organization, an "Org Admin" role holding
/// every permission, and a bootstrap AppUser invite so the very first sign-in has something
/// to claim. Idempotent - safe to run on every startup.
/// </summary>
public static class DataSeeder
{
    public const string BootstrapAdminEmail = "sakumarasingha@gmail.com";
    public const string BootstrapOrgSlug = "default-org";

    public static async Task SeedAsync(MftDbContext db, CancellationToken ct = default)
    {
        await SeedPermissionsAsync(db, ct);
        var org = await SeedOrganizationAsync(db, ct);
        var adminRole = await SeedOrgAdminRoleAsync(db, org.Id, ct);
        await SeedBootstrapAdminAsync(db, org.Id, adminRole.Id, ct);
        await SeedStorageAccountSettingsAsync(db, org.Id, ct);
    }

    private static async Task SeedPermissionsAsync(MftDbContext db, CancellationToken ct)
    {
        var existingKeys = await db.Permissions.Select(p => p.Key).ToListAsync(ct);
        foreach (var (key, category, description) in PermissionKeys.Catalog)
        {
            if (existingKeys.Contains(key))
            {
                continue;
            }

            db.Permissions.Add(new Permission
            {
                Id = Guid.NewGuid(),
                Key = key,
                Category = category,
                Description = description
            });
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task<Organization> SeedOrganizationAsync(MftDbContext db, CancellationToken ct)
    {
        var org = await db.Organizations.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Slug == BootstrapOrgSlug, ct);
        if (org is not null)
        {
            return org;
        }

        org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "Default Organization",
            Slug = BootstrapOrgSlug,
            IsActive = true,
            TimeZoneId = "UTC"
        };
        db.Organizations.Add(org);
        await db.SaveChangesAsync(ct);
        return org;
    }

    private static async Task<Role> SeedOrgAdminRoleAsync(MftDbContext db, Guid organizationId, CancellationToken ct)
    {
        var role = await db.Roles.IgnoreQueryFilters()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.Name == "Org Admin", ct);

        var allPermissions = await db.Permissions.ToListAsync(ct);

        if (role is null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Name = "Org Admin",
                Description = "Full access to everything within the organization.",
                IsSystemDefined = true
            };
            db.Roles.Add(role);
        }

        var assignedPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
        foreach (var permission in allPermissions)
        {
            if (!assignedPermissionIds.Contains(permission.Id))
            {
                db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
            }
        }

        await db.SaveChangesAsync(ct);
        return role;
    }

    private static async Task SeedBootstrapAdminAsync(MftDbContext db, Guid organizationId, Guid adminRoleId, CancellationToken ct)
    {
        var user = await db.AppUsers.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.OrganizationId == organizationId && u.Email == BootstrapAdminEmail, ct);

        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Email = BootstrapAdminEmail,
                DisplayName = "Bootstrap Admin",
                IsActive = true,
                InvitedAtUtc = DateTime.UtcNow
            };
            db.AppUsers.Add(user);
            await db.SaveChangesAsync(ct);
        }

        var hasAssignment = await db.RoleAssignments.IgnoreQueryFilters()
            .AnyAsync(ra => ra.AppUserId == user.Id && ra.RoleId == adminRoleId, ct);

        if (!hasAssignment)
        {
            db.RoleAssignments.Add(new RoleAssignment
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                AppUserId = user.Id,
                RoleId = adminRoleId
            });
            await db.SaveChangesAsync(ct);
        }
    }

    private static async Task SeedStorageAccountSettingsAsync(MftDbContext db, Guid organizationId, CancellationToken ct)
    {
        var exists = await db.StorageAccountSettings.IgnoreQueryFilters().AnyAsync(s => s.OrganizationId == organizationId, ct);
        if (exists)
        {
            return;
        }

        db.StorageAccountSettings.Add(new StorageAccountSettings
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId
        });
        await db.SaveChangesAsync(ct);
    }
}
