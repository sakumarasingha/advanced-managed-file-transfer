using Mft.Domain.Entities;

namespace Mft.Domain.Tests;

public class TransferRouteDefaultsTests
{
    [Fact]
    public void MaxFileSize_DefaultsTo1MB_AndIsEnabled()
    {
        var route = new TransferRoute();

        Assert.Equal(1_048_576, route.MaxFileSizeBytes);
        Assert.True(route.MaxFileSizeLimitEnabled);
    }

    [Fact]
    public void ArchiveEnabled_DefaultsToTrue()
    {
        var route = new TransferRoute();

        Assert.True(route.ArchiveEnabled);
    }
}
