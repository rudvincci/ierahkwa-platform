using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a behavioral biometrics profile is created.
/// </summary>
internal record BehavioralBiometricsProfileCreated(
    BehavioralBiometricsId BehavioralBiometricsId,
    IdentityId IdentityId,
    string DeviceFingerprint,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when an enrollment session is started.
/// </summary>
internal record EnrollmentSessionStarted(
    BehavioralBiometricsId BehavioralBiometricsId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when keystroke data is recorded.
/// </summary>
internal record KeystrokeDataRecorded(
    BehavioralBiometricsId BehavioralBiometricsId,
    int KeyEventCount,
    SessionType SessionType,
    DateTime RecordedAt) : IDomainEvent;

/// <summary>
/// Event raised when motion data is recorded.
/// </summary>
internal record MotionDataRecorded(
    BehavioralBiometricsId BehavioralBiometricsId,
    int MotionEventCount,
    SessionType SessionType,
    DateTime RecordedAt) : IDomainEvent;

/// <summary>
/// Event raised when an enrollment session is completed.
/// </summary>
internal record EnrollmentSessionCompleted(
    BehavioralBiometricsId BehavioralBiometricsId,
    int TotalSessionsCompleted,
    int ConfidenceScore,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when behavioral verification is performed.
/// </summary>
internal record BehavioralVerificationPerformed(
    BehavioralBiometricsId BehavioralBiometricsId,
    bool IsMatch,
    int ConfidenceScore,
    DateTime VerifiedAt) : IDomainEvent;

/// <summary>
/// Event raised when a behavioral anomaly is detected.
/// </summary>
internal record BehavioralAnomalyDetected(
    BehavioralBiometricsId BehavioralBiometricsId,
    int ConfidenceScore,
    string Reason,
    DateTime DetectedAt) : IDomainEvent;
