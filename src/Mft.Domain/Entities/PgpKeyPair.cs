using Mft.Domain.Common;
using Mft.Domain.Enums;

namespace Mft.Domain.Entities;

public class PgpKeyPair : IHasOrganization
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string Name { get; set; } = string.Empty;
    public PgpKeyType KeyType { get; set; } = PgpKeyType.Rsa;
    public int? KeySizeBits { get; set; } = 4096;
    public PgpMode PgpMode { get; set; } = PgpMode.Classic;

    public string PublicKeyArmored { get; set; } = string.Empty;
    public byte[] PrivateKeyEncrypted { get; set; } = [];
    public byte[]? PassphraseEncrypted { get; set; }

    public string Fingerprint { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? RevokedAtUtc { get; set; }
}
