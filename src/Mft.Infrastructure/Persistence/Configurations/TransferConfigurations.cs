using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mft.Infrastructure.Persistence.Configurations;

public class NamingConventionConfiguration : IEntityTypeConfiguration<NamingConvention>
{
    public void Configure(EntityTypeBuilder<NamingConvention> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Name).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Description).HasMaxLength(1000);
        builder.Property(n => n.PathTemplate).HasMaxLength(1000).IsRequired();

        builder.HasOne(n => n.Organization).WithMany().HasForeignKey(n => n.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExternalSftpConnectionConfiguration : IEntityTypeConfiguration<ExternalSftpConnection>
{
    public void Configure(EntityTypeBuilder<ExternalSftpConnection> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Host).HasMaxLength(255).IsRequired();
        builder.Property(c => c.Username).HasMaxLength(200).IsRequired();
        builder.Property(c => c.HostKeyFingerprint).HasMaxLength(200);

        builder.HasOne(c => c.Organization).WithMany().HasForeignKey(c => c.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TransferEndpointConfiguration : IEntityTypeConfiguration<TransferEndpoint>
{
    public void Configure(EntityTypeBuilder<TransferEndpoint> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Container).HasMaxLength(63);
        builder.Property(e => e.FolderPath).HasMaxLength(1024);
        builder.Property(e => e.RemotePath).HasMaxLength(1024);

        builder.HasOne(e => e.Organization).WithMany().HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.SftpLocalUser).WithMany().HasForeignKey(e => e.SftpLocalUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ExternalSftpConnection).WithMany().HasForeignKey(e => e.ExternalSftpConnectionId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TransferRouteConfiguration : IEntityTypeConfiguration<TransferRoute>
{
    public void Configure(EntityTypeBuilder<TransferRoute> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.FileTypeSubfolder).HasMaxLength(200);
        builder.Property(t => t.CronExpression).HasMaxLength(200);
        builder.Property(t => t.TimeZoneId).HasMaxLength(100);

        builder.HasOne(t => t.Organization).WithMany().HasForeignKey(t => t.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.NamingConvention).WithMany().HasForeignKey(t => t.NamingConventionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.SourceEndpoint).WithMany().HasForeignKey(t => t.SourceEndpointId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.TargetEndpoint).WithMany().HasForeignKey(t => t.TargetEndpointId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.ArchiveEndpoint).WithMany().HasForeignKey(t => t.ArchiveEndpointId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.ErrorEndpoint).WithMany().HasForeignKey(t => t.ErrorEndpointId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.TransformationSteps).WithOne(s => s.TransferRoute).HasForeignKey(s => s.TransferRouteId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TransformationStepConfiguration : IEntityTypeConfiguration<TransformationStep>
{
    public void Configure(EntityTypeBuilder<TransformationStep> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasOne(s => s.Organization).WithMany().HasForeignKey(s => s.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.PgpKeyPair).WithMany().HasForeignKey(s => s.PgpKeyPairId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.TransferRouteId, s.Order });
    }
}

public class PgpKeyPairConfiguration : IEntityTypeConfiguration<PgpKeyPair>
{
    public void Configure(EntityTypeBuilder<PgpKeyPair> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Name).HasMaxLength(200).IsRequired();
        builder.Property(k => k.PublicKeyArmored).IsRequired();
        builder.Property(k => k.PrivateKeyEncrypted).IsRequired();
        builder.Property(k => k.Fingerprint).HasMaxLength(200).IsRequired();

        builder.HasOne(k => k.Organization).WithMany().HasForeignKey(k => k.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TransferHistoryConfiguration : IEntityTypeConfiguration<TransferHistory>
{
    public void Configure(EntityTypeBuilder<TransferHistory> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.FileName).HasMaxLength(500).IsRequired();
        builder.Property(t => t.SourcePath).HasMaxLength(1024).IsRequired();
        builder.Property(t => t.DestinationPath).HasMaxLength(1024);
        builder.Property(t => t.ArchivePath).HasMaxLength(1024);
        builder.Property(t => t.ErrorFolderPath).HasMaxLength(1024);
        builder.Property(t => t.ErrorMessage).HasMaxLength(2000);
        builder.Property(t => t.ErrorCode).HasMaxLength(100);

        builder.HasOne(t => t.Organization).WithMany().HasForeignKey(t => t.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.TransferRoute).WithMany().HasForeignKey(t => t.TransferRouteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.RetryOfTransferHistory).WithMany().HasForeignKey(t => t.RetryOfTransferHistoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => new { t.OrganizationId, t.TransferRouteId, t.StartedAtUtc });
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).HasMaxLength(200).IsRequired();
        builder.Property(a => a.EntityType).HasMaxLength(200).IsRequired();
        builder.Property(a => a.EntityId).HasMaxLength(200).IsRequired();
        builder.Property(a => a.IpAddress).HasMaxLength(64);
        builder.HasIndex(a => new { a.OrganizationId, a.TimestampUtc });
    }
}

public class StorageAccountSettingsConfiguration : IEntityTypeConfiguration<StorageAccountSettings>
{
    public void Configure(EntityTypeBuilder<StorageAccountSettings> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StorageAccountName).HasMaxLength(24);
        builder.Property(s => s.ResourceGroup).HasMaxLength(200);
        builder.Property(s => s.SubscriptionId).HasMaxLength(100);
        builder.Property(s => s.LastTestMessage).HasMaxLength(2000);
        builder.HasIndex(s => s.OrganizationId).IsUnique();

        builder.HasOne(s => s.Organization).WithMany().HasForeignKey(s => s.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
