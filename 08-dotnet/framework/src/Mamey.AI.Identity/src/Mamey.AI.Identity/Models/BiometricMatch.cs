namespace Mamey.AI.Identity.Models;

/// <summary>
/// Represents the result of a biometric matching operation.
/// </summary>
public class BiometricMatch
{
    /// <summary>
    /// Similarity score (0-1.0, where 1.0 is a perfect match).
    /// </summary>
    public double SimilarityScore { get; set; }

    /// <summary>
    /// Whether the match is considered a match based on threshold.
    /// </summary>
    public bool IsMatch { get; set; }

    /// <summary>
    /// Type of biometric: Face, Fingerprint, Voice.
    /// </summary>
    public string BiometricType { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level in the match (0-1.0).
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Timestamp when the match was performed.
    /// </summary>
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
}
