using Mft.Application.Common;

namespace Mft.Domain.Tests;

public class PermissionCatalogTests
{
    [Fact]
    public void Catalog_HasNoDuplicateKeys()
    {
        var keys = PermissionKeys.Catalog.Select(p => p.Key).ToList();

        Assert.Equal(keys.Distinct().Count(), keys.Count);
    }

    [Fact]
    public void Catalog_IncludesServerSetupManage()
    {
        Assert.Contains(PermissionKeys.Catalog, p => p.Key == PermissionKeys.ServerSetupManage);
    }
}
