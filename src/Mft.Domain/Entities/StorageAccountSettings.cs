using Mft.Domain.Common;

namespace Mft.Domain.Entities;

public class StorageAccountSettings : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string? StorageAccountName { get; set; }
    public string? ResourceGroup { get; set; }
    public string? SubscriptionId { get; set; }
    public bool SftpEnabled { get; set; }
    public DateTime? LastTestedAtUtc { get; set; }
    public bool? LastTestSucceeded { get; set; }
    public string? LastTestMessage { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
