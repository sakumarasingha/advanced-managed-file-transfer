using Mft.Application.Abstractions;
using Mft.Infrastructure;
using Mft.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mft.Api.IntegrationTests;

public class StorageAccountProvisioningTests
{
    private static ServiceProvider BuildProvider(string? provisioningMode)
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = "Server=localhost;Database=unused;",
            ["Storage:AccountProvisioning"] = provisioningMode,
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMftInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    [Theory]
    [InlineData(null, typeof(MockStorageAccountProvisioner))]
    [InlineData("Mock", typeof(MockStorageAccountProvisioner))]
    [InlineData("Azure", typeof(AzureStorageAccountProvisioner))]
    public void ProvisioningMode_SelectsImplementation(string? mode, Type expected)
    {
        using var provider = BuildProvider(mode);
        using var scope = provider.CreateScope();

        Assert.IsType(expected, scope.ServiceProvider.GetRequiredService<IStorageAccountProvisioner>());
    }

    [Fact]
    public void ProvisioningMode_RejectsUnknownValue()
    {
        Assert.Throws<NotSupportedException>(() => BuildProvider("Azur"));
    }

    [Fact]
    public async Task MockProvisioner_ReportsSuccessWithoutTouchingAzure()
    {
        var provisioner = new MockStorageAccountProvisioner(NullLogger<MockStorageAccountProvisioner>.Instance);

        var result = await provisioner.ProvisionAsync(
            new StorageAccountProvisionRequest(Guid.NewGuid().ToString(), "rg-test", "mftteststorage", SftpEnabled: true));

        Assert.True(result.Succeeded);
        Assert.Contains("Mock", result.Message);
    }
}
