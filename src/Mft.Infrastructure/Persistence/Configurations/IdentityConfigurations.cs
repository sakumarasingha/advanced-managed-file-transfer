using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mft.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(200);
        builder.Property(u => u.EntraObjectId).HasMaxLength(100);
        builder.Property(u => u.EntraTenantId).HasMaxLength(100);

        builder.HasIndex(u => new { u.OrganizationId, u.EntraTenantId, u.EntraObjectId }).IsUnique()
            .HasFilter("[EntraObjectId] IS NOT NULL AND [EntraTenantId] IS NOT NULL");
        builder.HasIndex(u => new { u.OrganizationId, u.Email }).IsUnique();

        builder.HasOne(u => u.Organization).WithMany().HasForeignKey(u => u.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(500);

        builder.HasOne(r => r.Organization).WithMany().HasForeignKey(r => r.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Key).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.Key).IsUnique();
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions).HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.HasKey(ra => ra.Id);
        builder.HasIndex(ra => new { ra.OrganizationId, ra.AppUserId, ra.RoleId }).IsUnique();

        builder.HasOne(ra => ra.Organization).WithMany().HasForeignKey(ra => ra.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ra => ra.AppUser).WithMany(u => u.RoleAssignments).HasForeignKey(ra => ra.AppUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ra => ra.Role).WithMany(r => r.RoleAssignments).HasForeignKey(ra => ra.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}
