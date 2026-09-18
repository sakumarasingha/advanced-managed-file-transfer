using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mft.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).HasMaxLength(200).IsRequired();
        builder.Property(o => o.Slug).HasMaxLength(100).IsRequired();
        builder.HasIndex(o => o.Slug).IsUnique();
        builder.Property(o => o.TimeZoneId).HasMaxLength(100);
        builder.Property(o => o.PlanTier).HasMaxLength(50);
    }
}
