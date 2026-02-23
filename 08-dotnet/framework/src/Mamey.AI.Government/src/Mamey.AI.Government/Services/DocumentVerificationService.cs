using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.AI.Government.Services;

public class DocumentVerificationService : IDocumentVerificationService
{
    private readonly DocumentClassifier _classifier;
    private readonly TamperDetector _tamperDetector;
    private readonly OcrService _ocrService;
    private readonly ILogger<DocumentVerificationService> _logger;

    public DocumentVerificationService(
        DocumentClassifier classifier,
        TamperDetector tamperDetector,
        OcrService ocrService,
        ILogger<DocumentVerificationService> logger)
    {
        _classifier = classifier;
        _tamperDetector = tamperDetector;
        _ocrService = ocrService;
        _logger = logger;
    }

    public async Task<DocumentVerificationResult> VerifyDocumentAsync(Stream documentStream, string expectedDocumentType, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting verification for document type: {ExpectedType}", expectedDocumentType);

        // 1. Reset stream position if needed
        if (documentStream.CanSeek) documentStream.Position = 0;

        // 2. Classify Document
        var detectedType = await _classifier.ClassifyAsync(documentStream, cancellationToken);
        var classificationConfidence = await _classifier.GetConfidenceAsync(documentStream, cancellationToken);

        if (!string.Equals(detectedType, expectedDocumentType, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Document type mismatch. Expected: {Expected}, Detected: {Detected}", expectedDocumentType, detectedType);
            return DocumentVerificationResult.Failed($"Document type mismatch. Expected {expectedDocumentType}, but detected {detectedType}.");
        }

        // 3. Check Authenticity
        if (documentStream.CanSeek) documentStream.Position = 0;
        var (isAuthentic, authenticityScore, issues) = await _tamperDetector.CheckAuthenticityAsync(documentStream, cancellationToken);

        if (!isAuthentic)
        {
            _logger.LogWarning("Document failed authenticity check. Issues: {Issues}", string.Join(", ", issues));
            var result = DocumentVerificationResult.Failed("Document authenticity check failed.");
            result.ValidationErrors.AddRange(issues);
            result.AuthenticityScore = authenticityScore;
            return result;
        }

        // 4. Extract Data
        if (documentStream.CanSeek) documentStream.Position = 0;
        var extractedData = await _ocrService.ExtractTextAsync(documentStream, cancellationToken);

        // 5. Construct Success Result
        var successResult = DocumentVerificationResult.Success(classificationConfidence, detectedType, extractedData);
        successResult.AuthenticityScore = authenticityScore;
        
        _logger.LogInformation("Document verified successfully.");
        return successResult;
    }

    public async Task<DocumentVerificationResult> AnalyzeDocumentAsync(Stream documentStream, CancellationToken cancellationToken = default)
    {
        // Generalized analysis without an expected type
        if (documentStream.CanSeek) documentStream.Position = 0;
        
        var detectedType = await _classifier.ClassifyAsync(documentStream, cancellationToken);
        var confidence = await _classifier.GetConfidenceAsync(documentStream, cancellationToken);
        
        if (documentStream.CanSeek) documentStream.Position = 0;
        var (isAuthentic, authenticityScore, issues) = await _tamperDetector.CheckAuthenticityAsync(documentStream, cancellationToken);
        
        if (documentStream.CanSeek) documentStream.Position = 0;
        var extractedData = await _ocrService.ExtractTextAsync(documentStream, cancellationToken);

        return new DocumentVerificationResult
        {
            IsVerified = isAuthentic,
            ConfidenceScore = confidence,
            DocumentType = detectedType,
            IsAuthentic = isAuthentic,
            AuthenticityScore = authenticityScore,
            ExtractedData = extractedData,
            ValidationErrors = issues,
            VerifiedAt = DateTime.UtcNow
        };
    }
}
