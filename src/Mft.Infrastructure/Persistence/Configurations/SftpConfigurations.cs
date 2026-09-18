using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mft.Infrastructure.Persistence.Configurations;

public class SftpLocalUserConfiguration : IEntityTypeConfiguration<SftpLocalUser>
{
    public void Configure(EntityTypeBuilder<SftpLocalUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).HasMaxLength(64).IsRequired();
        builder.HasIndex(u => new { u.OrganizationId, u.Username }).IsUnique();
        builder.Property(u => u.HomeContainer).HasMaxLength(63).IsRequired();
        builder.Property(u => u.HomeDirectory).HasMaxLength(1024);
        builder.Property(u => u.ExternalLocalUserId).HasMaxLength(200);
        builder.Property(u => u.LastProvisioningError).HasMaxLength(2000);

        builder.HasOne(u => u.Organization).WithMany().HasForeignKey(u => u.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.SshKeys).WithOne(k => k.SftpLocalUser).HasForeignKey(k => k.SftpLocalUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Permissions).WithOne(p => p.SftpLocalUser).HasForeignKey(p => p.SftpLocalUserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SftpUserSshKeyConfiguration : IEntityTypeConfiguration<SftpUserSshKey>
{
    public void Configure(EntityTypeBuilder<SftpUserSshKey> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.PublicKeyOpenSsh).IsRequired();
        builder.Property(k => k.Fingerprint).HasMaxLength(200).IsRequired();
        builder.Property(k => k.Label).HasMaxLength(200);

        builder.HasOne(k => k.Organization).WithMany().HasForeignKey(k => k.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SftpUserPermissionConfiguration : IEntityTypeConfiguration<SftpUserPermission>
{
    public void Configure(EntityTypeBuilder<SftpUserPermission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ContainerName).HasMaxLength(63).IsRequired();
        builder.Property(p => p.PathPrefix).HasMaxLength(1024);

        builder.HasOne(p => p.Organization).WithMany().HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
