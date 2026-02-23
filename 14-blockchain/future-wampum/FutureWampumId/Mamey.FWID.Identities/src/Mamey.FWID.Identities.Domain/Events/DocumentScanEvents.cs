using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a document scan is created.
/// </summary>
internal record DocumentScanCreated(
    DocumentScanId DocumentScanId,
    VerificationSessionId VerificationSessionId,
    DocumentType DocumentType,
    string FileName,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when OCR processing starts.
/// </summary>
internal record OcrProcessingStarted(
    DocumentScanId DocumentScanId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when OCR processing completes.
/// </summary>
internal record OcrProcessingCompleted(
    DocumentScanId DocumentScanId,
    int ResultCount,
    int ConfidenceScore,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when fraud detection starts.
/// </summary>
internal record FraudDetectionStarted(
    DocumentScanId DocumentScanId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when fraud detection completes.
/// </summary>
internal record FraudDetectionCompleted(
    DocumentScanId DocumentScanId,
    int IndicatorCount,
    int RiskScore,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when validation starts.
/// </summary>
internal record ValidationStarted(
    DocumentScanId DocumentScanId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when validation completes.
/// </summary>
internal record ValidationCompleted(
    DocumentScanId DocumentScanId,
    int ResultCount,
    int OverallScore,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when a document scan fails.
/// </summary>
internal record DocumentScanFailed(
    DocumentScanId DocumentScanId,
    string Reason,
    DateTime FailedAt) : IDomainEvent;
