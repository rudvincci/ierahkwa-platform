using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered document analysis service.
/// </summary>
public interface IDocumentAnalysisService
{
    /// <summary>
    /// Analyzes a document for authenticity and extracts data.
    /// </summary>
    Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        DocumentAnalysisRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Classifies document type.
    /// </summary>
    Task<DocumentClassification> ClassifyDocumentAsync(
        byte[] documentImage,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs OCR and data extraction.
    /// </summary>
    Task<ExtractedDocumentData> ExtractDataAsync(
        byte[] documentImage,
        DocumentClassification documentType,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detects tampering in document.
    /// </summary>
    Task<List<TamperingIndicator>> DetectTamperingAsync(
        byte[] documentImage,
        CancellationToken cancellationToken = default);
}

public class DocumentAnalysisRequest
{
    public Guid IdentityId { get; set; }
    public byte[] DocumentImage { get; set; } = null!;
    public byte[]? DocumentBackImage { get; set; }
    public DocumentClassification? ExpectedType { get; set; }
    public string? IssuingCountry { get; set; }
}
