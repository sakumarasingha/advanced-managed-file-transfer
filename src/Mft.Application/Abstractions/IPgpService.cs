using Mft.Domain.Enums;

namespace Mft.Application.Abstractions;

public record PgpKeyGenerationResult(
    string PublicKeyArmored,
    byte[] PrivateKeyMaterial,
    string Fingerprint);

public interface IPgpService
{
    Task<PgpKeyGenerationResult> GenerateKeyPairAsync(
        PgpKeyType keyType,
        int? rsaKeySizeBits,
        PgpMode mode,
        string? passphrase,
        CancellationToken ct = default);

    Task EncryptAsync(Stream plaintext, Stream output, string publicKeyArmored, bool useAead, CancellationToken ct = default);

    Task DecryptAsync(Stream ciphertext, Stream output, byte[] privateKeyMaterial, string? passphrase, CancellationToken ct = default);
}
