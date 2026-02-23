using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a verification session aggregate root for identity verification.
/// </summary>
internal class VerificationSession : AggregateRoot<VerificationSessionId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private VerificationSession()
    {
        VerificationSteps = new List<VerificationStep>();
        Documents = new List<VerificationDocument>();
        BiometricData = new List<BiometricSample>();
    }

    /// <summary>
    /// Initializes a new instance of the VerificationSession aggregate root.
    /// </summary>
    /// <param name="id">The verification session identifier.</param>
    /// <param name="identityId">The identity being verified.</param>
    /// <param name="verificationType">The type of verification (document, biometric, etc.).</param>
    /// <param name="requestedBy">The entity requesting the verification.</param>
    public VerificationSession(
        VerificationSessionId id,
        IdentityId identityId,
        VerificationType verificationType,
        string requestedBy)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        VerificationType = verificationType;
        RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy));
        Status = VerificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        VerificationSteps = new List<VerificationStep>();
        Documents = new List<VerificationDocument>();
        BiometricData = new List<BiometricSample>();
        Version = 1;

        AddEvent(new VerificationSessionStarted(Id, IdentityId, VerificationType, RequestedBy, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity being verified.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The type of verification.
    /// </summary>
    public VerificationType VerificationType { get; private set; }

    /// <summary>
    /// The entity that requested the verification.
    /// </summary>
    public string RequestedBy { get; private set; }

    /// <summary>
    /// The current status of the verification session.
    /// </summary>
    public VerificationStatus Status { get; private set; }

    /// <summary>
    /// When the verification session was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the verification session was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// The overall verification result.
    /// </summary>
    public VerificationResult? Result { get; private set; }

    /// <summary>
    /// The confidence score (0-100) of the verification.
    /// </summary>
    public int? ConfidenceScore { get; private set; }

    /// <summary>
    /// The verification steps performed.
    /// </summary>
    public List<VerificationStep> VerificationSteps { get; private set; }

    /// <summary>
    /// The documents submitted for verification.
    /// </summary>
    public List<VerificationDocument> Documents { get; private set; }

    /// <summary>
    /// The biometric samples collected.
    /// </summary>
    public List<BiometricSample> BiometricData { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Adds a document to the verification session.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="documentType">The type of document.</param>
    /// <param name="fileName">The original file name.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="fileSize">The file size in bytes.</param>
    /// <param name="storagePath">The storage path.</param>
    public void AddDocument(
        string documentId,
        DocumentType documentType,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath)
    {
        if (Status != VerificationStatus.Pending && Status != VerificationStatus.InProgress)
            throw new InvalidOperationException("Cannot add documents to completed verification session");

        var document = new VerificationDocument(
            documentId,
            documentType,
            fileName,
            contentType,
            fileSize,
            storagePath,
            DateTime.UtcNow);

        Documents.Add(document);
        Status = VerificationStatus.InProgress;
        IncrementVersion();

        AddEvent(new DocumentAddedToVerification(Id, documentId, documentType, fileName));
    }

    /// <summary>
    /// Adds a biometric sample to the verification session.
    /// </summary>
    /// <param name="sampleId">The sample identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="data">The biometric data.</param>
    /// <param name="qualityScore">The quality score of the sample.</param>
    public void AddBiometricSample(
        string sampleId,
        BiometricType biometricType,
        byte[] data,
        int qualityScore)
    {
        if (Status != VerificationStatus.Pending && Status != VerificationStatus.InProgress)
            throw new InvalidOperationException("Cannot add biometric samples to completed verification session");

        var sample = new BiometricSample(
            sampleId,
            biometricType,
            data,
            qualityScore,
            DateTime.UtcNow);

        BiometricData.Add(sample);
        Status = VerificationStatus.InProgress;
        IncrementVersion();

        AddEvent(new BiometricSampleAdded(Id, sampleId, biometricType));
    }

    /// <summary>
    /// Starts processing the verification.
    /// </summary>
    public void StartProcessing()
    {
        if (Status != VerificationStatus.Pending)
            return;

        Status = VerificationStatus.InProgress;
        IncrementVersion();

        AddEvent(new VerificationProcessingStarted(Id));
    }

    /// <summary>
    /// Adds a verification step result.
    /// </summary>
    /// <param name="stepName">The name of the verification step.</param>
    /// <param name="result">The result of the step.</param>
    /// <param name="confidenceScore">The confidence score.</param>
    /// <param name="details">Additional details.</param>
    public void AddVerificationStep(
        string stepName,
        VerificationStepResult result,
        int confidenceScore,
        string? details = null)
    {
        var step = new VerificationStep(
            stepName,
            result,
            confidenceScore,
            details,
            DateTime.UtcNow);

        VerificationSteps.Add(step);
        IncrementVersion();

        AddEvent(new VerificationStepCompleted(Id, stepName, result, confidenceScore));
    }

    /// <summary>
    /// Completes the verification session.
    /// </summary>
    /// <param name="result">The overall verification result.</param>
    /// <param name="confidenceScore">The overall confidence score.</param>
    /// <param name="reason">The reason for the result.</param>
    public void Complete(VerificationResult result, int confidenceScore, string? reason = null)
    {
        if (Status == VerificationStatus.Completed)
            return;

        Status = VerificationStatus.Completed;
        Result = result;
        ConfidenceScore = confidenceScore;
        CompletedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new VerificationSessionCompleted(Id, result, confidenceScore, reason, CompletedAt.Value));
    }

    /// <summary>
    /// Fails the verification session.
    /// </summary>
    /// <param name="reason">The reason for failure.</param>
    public void Fail(string reason)
    {
        if (Status == VerificationStatus.Failed)
            return;

        Status = VerificationStatus.Failed;
        Result = VerificationResult.Failed;
        CompletedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new VerificationSessionFailed(Id, reason, CompletedAt.Value));
    }

    #endregion
}

/// <summary>
/// Represents the type of verification.
/// </summary>
internal enum VerificationType
{
    Document,
    Biometric,
    DocumentAndBiometric,
    Liveness,
    Address
}

/// <summary>
/// Represents the status of a verification session.
/// </summary>
internal enum VerificationStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Expired
}

/// <summary>
/// Represents the result of a verification step.
/// </summary>
internal enum VerificationStepResult
{
    Passed,
    Failed,
    Warning,
    Skipped
}

/// <summary>
/// Represents a verification step.
/// </summary>
internal class VerificationStep
{
    public string StepName { get; set; }
    public VerificationStepResult Result { get; set; }
    public int ConfidenceScore { get; set; }
    public string? Details { get; set; }
    public DateTime ExecutedAt { get; set; }

    public VerificationStep(
        string stepName,
        VerificationStepResult result,
        int confidenceScore,
        string? details,
        DateTime executedAt)
    {
        StepName = stepName;
        Result = result;
        ConfidenceScore = confidenceScore;
        Details = details;
        ExecutedAt = executedAt;
    }
}

/// <summary>
/// Represents a document submitted for verification.
/// </summary>
internal class VerificationDocument
{
    public string DocumentId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string StoragePath { get; set; }
    public DateTime UploadedAt { get; set; }

    public VerificationDocument(
        string documentId,
        DocumentType documentType,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath,
        DateTime uploadedAt)
    {
        DocumentId = documentId;
        DocumentType = documentType;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        StoragePath = storagePath;
        UploadedAt = uploadedAt;
    }
}

/// <summary>
/// Represents a biometric sample.
/// </summary>
internal class BiometricSample
{
    public string SampleId { get; set; }
    public BiometricType BiometricType { get; set; }
    public byte[] Data { get; set; }
    public int QualityScore { get; set; }
    public DateTime CapturedAt { get; set; }

    public BiometricSample(
        string sampleId,
        BiometricType biometricType,
        byte[] data,
        int qualityScore,
        DateTime capturedAt)
    {
        SampleId = sampleId;
        BiometricType = biometricType;
        Data = data;
        QualityScore = qualityScore;
        CapturedAt = capturedAt;
    }
}

/// <summary>
/// Represents the type of document.
/// </summary>
internal enum DocumentType
{
    Passport,
    DriversLicense,
    NationalId,
    BirthCertificate,
    UtilityBill,
    BankStatement
}
