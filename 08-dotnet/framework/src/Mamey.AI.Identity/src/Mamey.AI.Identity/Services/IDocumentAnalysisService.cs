using Mamey.AI.Identity.Models;

namespace Mamey.AI.Identity.Services;

/// <summary>
/// Service for analyzing identity documents (ID cards, passports, etc.).
/// </summary>
public interface IDocumentAnalysisService
{
    /// <summary>
    /// Analyzes an identity document image and extracts information.
    /// </summary>
    Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        Stream documentImage,
        string documentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the authenticity of an identity document.
    /// </summary>
    Task<DocumentVerificationResult> VerifyDocumentAsync(
        Stream documentImage,
        string documentType,
        CancellationToken cancellationToken = default);
}
