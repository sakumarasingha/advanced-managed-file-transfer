using Mft.Application.Abstractions;
using Mft.Infrastructure.Identity;
using Mft.Infrastructure.Persistence;
using Mft.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mft.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMftInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MftDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<CurrentContextAccessor>();
        services.AddScoped<ICurrentOrganizationContext>(sp => sp.GetRequiredService<CurrentContextAccessor>());
        services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<CurrentContextAccessor>());

        var storageMode = configuration["Storage:Mode"] ?? "Mock";
        if (storageMode == "Mock")
        {
            services.AddScoped<IStorageSftpProvider, MockStorageSftpProvider>();
        }
        else
        {
            throw new NotSupportedException(
                $"Storage:Mode '{storageMode}' is not yet supported - only 'Mock' is implemented in this milestone.");
        }

        // Separate from Storage:Mode so the real account provisioning can be switched on while the
        // SFTP user/container operations are still mocked.
        var provisioningMode = configuration["Storage:AccountProvisioning"] ?? "Mock";
        switch (provisioningMode)
        {
            case "Mock":
                services.AddScoped<IStorageAccountProvisioner, MockStorageAccountProvisioner>();
                break;
            case "Azure":
                services.AddScoped<IStorageAccountProvisioner, AzureStorageAccountProvisioner>();
                break;
            default:
                throw new NotSupportedException(
                    $"Storage:AccountProvisioning '{provisioningMode}' is not supported - use 'Mock' or 'Azure'.");
        }

        return services;
    }
}
