using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

/// <summary>
/// The core MFT pipeline: Source -> (optional, ordered) Transformation steps -> Target, plus
/// where successfully/unsuccessfully processed files go (Archive/Error) and when it runs.
/// </summary>
public class TransferRoute : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; } = true;

    public Guid SourceEndpointId { get; set; }
    public TransferEndpoint? SourceEndpoint { get; set; }

    public Guid TargetEndpointId { get; set; }
    public TransferEndpoint? TargetEndpoint { get; set; }

    public List<TransformationStep> TransformationSteps { get; set; } = [];

    public FileTypeResolutionMode FileTypeResolutionMode { get; set; } = FileTypeResolutionMode.Static;
    public string? FileTypeSubfolder { get; set; }

    public Guid NamingConventionId { get; set; }
    public NamingConvention? NamingConvention { get; set; }

    public bool ArchiveEnabled { get; set; } = true;
    public Guid? ArchiveEndpointId { get; set; }
    public TransferEndpoint? ArchiveEndpoint { get; set; }

    public Guid ErrorEndpointId { get; set; }
    public TransferEndpoint? ErrorEndpoint { get; set; }

    public ScheduleType ScheduleType { get; set; } = ScheduleType.IntervalMinutes;
    public int? PickupIntervalMinutes { get; set; } = 15;
    public string? CronExpression { get; set; }
    public string TimeZoneId { get; set; } = "UTC";

    public long MaxFileSizeBytes { get; set; } = 1_048_576;
    public bool MaxFileSizeLimitEnabled { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>
/// One step in a route's ordered transformation pipeline between Source and Target. Only
/// PGP encrypt/decrypt exist today; Type is designed to grow (e.g. format conversion) without
/// changing the route shape.
/// </summary>
public class TransformationStep : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid TransferRouteId { get; set; }
    public TransferRoute? TransferRoute { get; set; }

    public int Order { get; set; }
    public TransformationType Type { get; set; }

    public Guid? PgpKeyPairId { get; set; }
    public PgpKeyPair? PgpKeyPair { get; set; }
}
