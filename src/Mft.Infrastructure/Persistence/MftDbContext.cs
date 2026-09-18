using System.Linq.Expressions;
using Mft.Application.Abstractions;
using Mft.Domain.Common;
using Mft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mft.Infrastructure.Persistence;

public class MftDbContext : DbContext
{
    private readonly ICurrentOrganizationContext _orgContext;

    // Delegates to the scoped, mutable org context on every access. Referencing this property
    // (via `this`) in query filters lets EF Core re-evaluate it per instance/query instead of
    // baking a single value into the cached model - see EF Core's documented multi-tenancy
    // global-filter pattern. Because the underlying context is mutable, a value resolved mid
    // request (e.g. by TenantResolutionMiddleware) is honored by queries later in that request.
    public Guid? CurrentOrganizationId => _orgContext.OrganizationId;

    public MftDbContext(DbContextOptions<MftDbContext> options, ICurrentOrganizationContext orgContext)
        : base(options)
    {
        _orgContext = orgContext;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleAssignment> RoleAssignments => Set<RoleAssignment>();
    public DbSet<SftpLocalUser> SftpLocalUsers => Set<SftpLocalUser>();
    public DbSet<SftpUserSshKey> SftpUserSshKeys => Set<SftpUserSshKey>();
    public DbSet<SftpUserPermission> SftpUserPermissions => Set<SftpUserPermission>();
    public DbSet<NamingConvention> NamingConventions => Set<NamingConvention>();
    public DbSet<ExternalSftpConnection> ExternalSftpConnections => Set<ExternalSftpConnection>();
    public DbSet<TransferEndpoint> TransferEndpoints => Set<TransferEndpoint>();
    public DbSet<TransferRoute> TransferRoutes => Set<TransferRoute>();
    public DbSet<TransformationStep> TransformationSteps => Set<TransformationStep>();
    public DbSet<PgpKeyPair> PgpKeyPairs => Set<PgpKeyPair>();
    public DbSet<TransferHistory> TransferHistories => Set<TransferHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<StorageAccountSettings> StorageAccountSettings => Set<StorageAccountSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MftDbContext).Assembly);

        var contextInstance = Expression.Constant(this);
        var currentOrgIdMember = Expression.Property(contextInstance, nameof(CurrentOrganizationId));
        var hasValue = Expression.Property(currentOrgIdMember, nameof(Nullable<Guid>.HasValue));
        var currentOrgIdValue = Expression.Property(currentOrgIdMember, nameof(Nullable<Guid>.Value));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(IHasOrganization).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var orgIdProperty = Expression.Property(parameter, nameof(IHasOrganization.OrganizationId));
            var equalsExpr = Expression.Equal(orgIdProperty, currentOrgIdValue);
            var body = Expression.AndAlso(hasValue, equalsExpr);
            var lambda = Expression.Lambda(body, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampOrganizationId();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampOrganizationId();
        return base.SaveChanges();
    }

    private void StampOrganizationId()
    {
        if (CurrentOrganizationId is not { } orgId)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IHasOrganization tenantEntity && entry.State == EntityState.Added)
            {
                if (tenantEntity.OrganizationId == Guid.Empty)
                {
                    tenantEntity.OrganizationId = orgId;
                }
            }
        }
    }
}
