using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

public class TransferHistory : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid TransferRouteId { get; set; }
    public TransferRoute? TransferRoute { get; set; }

    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public TransferTrigger TriggeredBy { get; set; } = TransferTrigger.Scheduled;

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }

    public string SourcePath { get; set; } = string.Empty;
    public string? DestinationPath { get; set; }
    public string? ArchivePath { get; set; }
    public string? ErrorFolderPath { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }

    public Guid? RetryOfTransferHistoryId { get; set; }
    public TransferHistory? RetryOfTransferHistory { get; set; }
    public int RetryCount { get; set; }

    public bool PgpApplied { get; set; }
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
}
