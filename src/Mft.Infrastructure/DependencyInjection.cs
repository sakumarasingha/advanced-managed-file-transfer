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

        return services;
    }
}
