using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a document scan aggregate root for AI/ML-powered document verification.
/// </summary>
internal class DocumentScan : AggregateRoot<DocumentScanId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private DocumentScan()
    {
        OcrResults = new List<OcrResult>();
        FraudIndicators = new List<FraudIndicator>();
        ValidationResults = new List<ValidationResult>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the DocumentScan aggregate root.
    /// </summary>
    /// <param name="id">The document scan identifier.</param>
    /// <param name="verificationSessionId">The verification session this scan belongs to.</param>
    /// <param name="documentType">The type of document being scanned.</param>
    /// <param name="fileName">The original file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="fileSize">The file size in bytes.</param>
    /// <param name="storagePath">The storage path.</param>
    public DocumentScan(
        DocumentScanId id,
        VerificationSessionId verificationSessionId,
        DocumentType documentType,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath)
        : base(id)
    {
        VerificationSessionId = verificationSessionId ?? throw new ArgumentNullException(nameof(verificationSessionId));
        DocumentType = documentType;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        FileSize = fileSize;
        StoragePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
        Status = ScanStatus.Uploaded;
        CreatedAt = DateTime.UtcNow;
        OcrResults = new List<OcrResult>();
        FraudIndicators = new List<FraudIndicator>();
        ValidationResults = new List<ValidationResult>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new DocumentScanCreated(Id, VerificationSessionId, DocumentType, FileName, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The verification session this scan belongs to.
    /// </summary>
    public VerificationSessionId VerificationSessionId { get; private set; }

    /// <summary>
    /// The type of document being scanned.
    /// </summary>
    public DocumentType DocumentType { get; private set; }

    /// <summary>
    /// The original file name.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// The content type of the file.
    /// </summary>
    public string ContentType { get; private set; }

    /// <summary>
    /// The file size in bytes.
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// The storage path of the file.
    /// </summary>
    public string StoragePath { get; private set; }

    /// <summary>
    /// The current status of the scan.
    /// </summary>
    public ScanStatus Status { get; private set; }

    /// <summary>
    /// When the scan was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When OCR processing started.
    /// </summary>
    public DateTime? OcrStartedAt { get; private set; }

    /// <summary>
    /// When OCR processing completed.
    /// </summary>
    public DateTime? OcrCompletedAt { get; private set; }

    /// <summary>
    /// When fraud detection started.
    /// </summary>
    public DateTime? FraudDetectionStartedAt { get; private set; }

    /// <summary>
    /// When fraud detection completed.
    /// </summary>
    public DateTime? FraudDetectionCompletedAt { get; private set; }

    /// <summary>
    /// When validation started.
    /// </summary>
    public DateTime? ValidationStartedAt { get; private set; }

    /// <summary>
    /// When validation completed.
    /// </summary>
    public DateTime? ValidationCompletedAt { get; private set; }

    /// <summary>
    /// Overall confidence score (0-100).
    /// </summary>
    public int? OverallConfidenceScore { get; private set; }

    /// <summary>
    /// OCR processing results.
    /// </summary>
    public List<OcrResult> OcrResults { get; private set; }

    /// <summary>
    /// Fraud detection indicators.
    /// </summary>
    public List<FraudIndicator> FraudIndicators { get; private set; }

    /// <summary>
    /// Validation results.
    /// </summary>
    public List<ValidationResult> ValidationResults { get; private set; }

    /// <summary>
    /// Extracted document data.
    /// </summary>
    public ExtractedDocumentData? ExtractedData { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Starts OCR processing.
    /// </summary>
    public void StartOcrProcessing()
    {
        if (Status != ScanStatus.Uploaded)
            throw new InvalidOperationException("OCR processing can only be started from uploaded status");

        Status = ScanStatus.OcrProcessing;
        OcrStartedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new OcrProcessingStarted(Id, OcrStartedAt.Value));
    }

    /// <summary>
    /// Completes OCR processing with results.
    /// </summary>
    /// <param name="results">The OCR results.</param>
    /// <param name="confidenceScore">The overall confidence score.</param>
    public void CompleteOcrProcessing(List<OcrResult> results, int confidenceScore)
    {
        if (Status != ScanStatus.OcrProcessing)
            throw new InvalidOperationException("OCR processing must be in progress");

        OcrResults = results ?? new List<OcrResult>();
        OcrCompletedAt = DateTime.UtcNow;
        Status = ScanStatus.OcrCompleted;
        IncrementVersion();

        AddEvent(new OcrProcessingCompleted(Id, results.Count, confidenceScore, OcrCompletedAt.Value));
    }

    /// <summary>
    /// Starts fraud detection.
    /// </summary>
    public void StartFraudDetection()
    {
        if (Status != ScanStatus.OcrCompleted)
            throw new InvalidOperationException("Fraud detection can only be started after OCR completion");

        Status = ScanStatus.FraudDetection;
        FraudDetectionStartedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new FraudDetectionStarted(Id, FraudDetectionStartedAt.Value));
    }

    /// <summary>
    /// Completes fraud detection with indicators.
    /// </summary>
    /// <param name="indicators">The fraud indicators found.</param>
    /// <param name="riskScore">The fraud risk score.</param>
    public void CompleteFraudDetection(List<FraudIndicator> indicators, int riskScore)
    {
        if (Status != ScanStatus.FraudDetection)
            throw new InvalidOperationException("Fraud detection must be in progress");

        FraudIndicators = indicators ?? new List<FraudIndicator>();
        FraudDetectionCompletedAt = DateTime.UtcNow;
        Status = ScanStatus.FraudCompleted;
        IncrementVersion();

        AddEvent(new FraudDetectionCompleted(Id, indicators.Count, riskScore, FraudDetectionCompletedAt.Value));
    }

    /// <summary>
    /// Starts document validation.
    /// </summary>
    public void StartValidation()
    {
        if (Status != ScanStatus.FraudCompleted)
            throw new InvalidOperationException("Validation can only be started after fraud detection");

        Status = ScanStatus.Validating;
        ValidationStartedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new ValidationStarted(Id, ValidationStartedAt.Value));
    }

    /// <summary>
    /// Completes document validation.
    /// </summary>
    /// <param name="results">The validation results.</param>
    /// <param name="extractedData">The extracted document data.</param>
    /// <param name="overallScore">The overall confidence score.</param>
    public void CompleteValidation(
        List<ValidationResult> results,
        ExtractedDocumentData extractedData,
        int overallScore)
    {
        if (Status != ScanStatus.Validating)
            throw new InvalidOperationException("Validation must be in progress");

        ValidationResults = results ?? new List<ValidationResult>();
        ExtractedData = extractedData;
        OverallConfidenceScore = overallScore;
        ValidationCompletedAt = DateTime.UtcNow;
        Status = ScanStatus.Completed;
        IncrementVersion();

        AddEvent(new ValidationCompleted(Id, results.Count, overallScore, ValidationCompletedAt.Value));
    }

    /// <summary>
    /// Marks the scan as failed.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    public void Fail(string reason)
    {
        Status = ScanStatus.Failed;
        IncrementVersion();

        AddEvent(new DocumentScanFailed(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if the document scan is valid for use.
    /// </summary>
    /// <returns>True if the scan passed all checks with sufficient confidence.</returns>
    public bool IsValid()
    {
        return Status == ScanStatus.Completed &&
               OverallConfidenceScore >= 80 && // Minimum confidence threshold
               !FraudIndicators.Any(f => f.Severity == FraudSeverity.Critical) &&
               ValidationResults.All(v => v.IsValid);
    }

    /// <summary>
    /// Gets the fraud risk level.
    /// </summary>
    /// <returns>The highest fraud severity found.</returns>
    public FraudSeverity GetFraudRiskLevel()
    {
        if (!FraudIndicators.Any())
            return FraudSeverity.None;

        return FraudIndicators.Max(f => f.Severity);
    }

    #endregion
}

/// <summary>
/// Represents the status of a document scan.
/// </summary>
internal enum ScanStatus
{
    Uploaded,
    OcrProcessing,
    OcrCompleted,
    FraudDetection,
    FraudCompleted,
    Validating,
    Completed,
    Failed
}

/// <summary>
/// Represents an OCR processing result.
/// </summary>
internal class OcrResult
{
    public string FieldName { get; set; }
    public string ExtractedText { get; set; }
    public int Confidence { get; set; }
    public BoundingBox BoundingBox { get; set; }
    public DateTime ProcessedAt { get; set; }

    public OcrResult(
        string fieldName,
        string extractedText,
        int confidence,
        BoundingBox boundingBox,
        DateTime processedAt)
    {
        FieldName = fieldName;
        ExtractedText = extractedText;
        Confidence = confidence;
        BoundingBox = boundingBox;
        ProcessedAt = processedAt;
    }
}

/// <summary>
/// Represents a bounding box for OCR results.
/// </summary>
internal class BoundingBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public BoundingBox(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

/// <summary>
/// Represents a fraud detection indicator.
/// </summary>
internal class FraudIndicator
{
    public string IndicatorType { get; set; }
    public FraudSeverity Severity { get; set; }
    public string Description { get; set; }
    public double Confidence { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
    public DateTime DetectedAt { get; set; }

    public FraudIndicator(
        string indicatorType,
        FraudSeverity severity,
        string description,
        double confidence,
        DateTime detectedAt)
    {
        IndicatorType = indicatorType;
        Severity = severity;
        Description = description;
        Confidence = confidence;
        DetectedAt = detectedAt;
    }
}

/// <summary>
/// Represents the severity of a fraud indicator.
/// </summary>
internal enum FraudSeverity
{
    None,
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Represents a validation result.
/// </summary>
internal class ValidationResult
{
    public string ValidationType { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int Score { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
    public DateTime ValidatedAt { get; set; }

    public ValidationResult(
        string validationType,
        bool isValid,
        string? errorMessage,
        int score,
        DateTime validatedAt)
    {
        ValidationType = validationType;
        IsValid = isValid;
        ErrorMessage = errorMessage;
        Score = score;
        ValidatedAt = validatedAt;
    }
}

/// <summary>
/// Represents extracted document data.
/// </summary>
internal class ExtractedDocumentData
{
    public string DocumentNumber { get; set; }
    public string IssuingCountry { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public PersonalInfo? PersonalInfo { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    public ExtractedDocumentData(
        string documentNumber,
        string issuingCountry,
        DateTime? issueDate,
        DateTime? expiryDate,
        PersonalInfo? personalInfo)
    {
        DocumentNumber = documentNumber;
        IssuingCountry = issuingCountry;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        PersonalInfo = personalInfo;
    }
}

/// <summary>
/// Represents personal information extracted from a document.
/// </summary>
internal class PersonalInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public Address? Address { get; set; }

    public PersonalInfo(
        string? firstName,
        string? lastName,
        DateTime? dateOfBirth,
        string? nationality,
        string? gender,
        Address? address)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Nationality = nationality;
        Gender = gender;
        Address = address;
    }
}
