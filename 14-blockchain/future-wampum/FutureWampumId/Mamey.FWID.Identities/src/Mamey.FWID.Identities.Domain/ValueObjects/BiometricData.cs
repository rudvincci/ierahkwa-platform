using System.Text.Json.Serialization;
using Mamey.Security;

namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents encrypted biometric data for identity verification.
/// Compliant with Biometric Verification Microservice spec (§2.2, §3).
/// </summary>
public class BiometricData : IEquatable<BiometricData>
{
    /// <summary>
    /// Initializes a new instance for Entity Framework.
    /// </summary>
    private BiometricData()
    {
        EncryptedTemplate = Array.Empty<byte>();
        Hash = string.Empty;
        TemplateId = string.Empty;
        AlgoVersion = string.Empty;
        Format = string.Empty;
        Quality = BiometricQuality.Unknown;
    }

    /// <summary>
    /// Initializes a new instance with the specified biometric data.
    /// </summary>
    /// <param name="type">The type of biometric data.</param>
    /// <param name="encryptedTemplate">The encrypted biometric template (vector/embedding).</param>
    /// <param name="hash">The hash of the biometric data for verification.</param>
    /// <param name="templateId">The template identifier (from Biometric Verification Microservice).</param>
    /// <param name="algoVersion">The algorithm version used (e.g., "face-3.2.0").</param>
    /// <param name="format">The template format (e.g., "ISO39794-5:Face").</param>
    /// <param name="quality">The quality assessment (GOOD | FAIR | POOR).</param>
    /// <param name="evidenceJws">The signed evidence JWS from Biometric Verification Microservice.</param>
    /// <param name="livenessScore">The PAD (Presentation Attack Detection) score (0..1).</param>
    /// <param name="livenessDecision">The PAD decision (PASS | FAIL | INCONCLUSIVE).</param>
    [JsonConstructor]
    public BiometricData(
        BiometricType type,
        byte[] encryptedTemplate,
        string hash,
        string? templateId = null,
        string? algoVersion = null,
        string? format = null,
        BiometricQuality quality = BiometricQuality.Unknown,
        string? evidenceJws = null,
        double? livenessScore = null,
        string? livenessDecision = null)
    {
        Type = type;
        EncryptedTemplate = encryptedTemplate ?? throw new ArgumentNullException(nameof(encryptedTemplate));
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        TemplateId = templateId ?? string.Empty;
        AlgoVersion = algoVersion ?? string.Empty;
        Format = format ?? string.Empty;
        Quality = quality;
        EvidenceJws = evidenceJws;
        LivenessScore = livenessScore;
        LivenessDecision = livenessDecision;
        
        if (encryptedTemplate.Length == 0)
            throw new ArgumentException("Encrypted template cannot be empty.", nameof(encryptedTemplate));
        
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be null or whitespace.", nameof(hash));
        
        CapturedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The type of biometric data.
    /// </summary>
    public BiometricType Type { get; private set; }

    /// <summary>
    /// The encrypted biometric template (vector/embedding).
    /// </summary>
    public byte[] EncryptedTemplate { get; private set; }

    /// <summary>
    /// The hash of the biometric data for verification.
    /// </summary>
    [Hashed]
    public string Hash { get; private set; }

    /// <summary>
    /// The template identifier from Biometric Verification Microservice.
    /// </summary>
    public string TemplateId { get; private set; }

    /// <summary>
    /// The algorithm version used (e.g., "face-3.2.0").
    /// </summary>
    public string AlgoVersion { get; private set; }

    /// <summary>
    /// The template format (e.g., "ISO39794-5:Face").
    /// </summary>
    public string Format { get; private set; }

    /// <summary>
    /// The quality assessment (GOOD | FAIR | POOR | UNKNOWN).
    /// </summary>
    public BiometricQuality Quality { get; private set; }

    /// <summary>
    /// The signed evidence JWS from Biometric Verification Microservice (§2.4).
    /// Contains PAD scores, match scores, quality metrics, policy, and provenance.
    /// </summary>
    public string? EvidenceJws { get; private set; }

    /// <summary>
    /// The PAD (Presentation Attack Detection) score (0..1).
    /// </summary>
    public double? LivenessScore { get; private set; }

    /// <summary>
    /// The PAD decision (PASS | FAIL | INCONCLUSIVE).
    /// </summary>
    public string? LivenessDecision { get; private set; }

    /// <summary>
    /// The date and time the biometric data was captured.
    /// </summary>
    public DateTime CapturedAt { get; private set; }

    /// <summary>
    /// Calculates the match score between this biometric data and another.
    /// </summary>
    /// <param name="other">The other biometric data to compare.</param>
    /// <returns>A match score between 0.0 and 1.0, where 1.0 is a perfect match.</returns>
    public double Match(BiometricData other)
    {
        if (other is null)
            return 0.0;
        
        if (Type != other.Type)
            return 0.0;
        
        // Simplified matching - in production, use actual biometric matching library
        // This is a placeholder that compares hashes for exact match
        if (Hash == other.Hash)
            return 1.0;
        
        // For now, return a placeholder score
        // In production, this would use actual biometric matching algorithms
        return CalculateMatchScore(EncryptedTemplate, other.EncryptedTemplate);
    }

    /// <summary>
    /// Calculates the match score between two encrypted templates.
    /// </summary>
    /// <param name="template1">The first template.</param>
    /// <param name="template2">The second template.</param>
    /// <returns>A similarity score between 0.0 and 1.0.</returns>
    private static double CalculateMatchScore(byte[] template1, byte[] template2)
    {
        if (template1.Length != template2.Length)
            return 0.0;
        
        // Simplified byte-level similarity calculation
        // In production, use actual biometric matching library
        int matchingBytes = 0;
        for (int i = 0; i < template1.Length; i++)
        {
            if (template1[i] == template2[i])
                matchingBytes++;
        }
        
        return (double)matchingBytes / template1.Length;
    }

    public bool Equals(BiometricData? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        // Compare by template ID if available, otherwise fall back to hash
        if (!string.IsNullOrEmpty(TemplateId) && !string.IsNullOrEmpty(other.TemplateId))
            return TemplateId == other.TemplateId;
        
        return Type == other.Type && Hash == other.Hash;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BiometricData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Hash);
    }

    public static bool operator ==(BiometricData? left, BiometricData? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BiometricData? left, BiometricData? right)
    {
        return !Equals(left, right);
    }
}

