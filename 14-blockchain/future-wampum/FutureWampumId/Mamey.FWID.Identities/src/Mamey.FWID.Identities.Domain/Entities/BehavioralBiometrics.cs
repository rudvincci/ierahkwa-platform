using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a behavioral biometrics aggregate root for keystroke and motion signature analysis.
/// </summary>
internal class BehavioralBiometrics : AggregateRoot<BehavioralBiometricsId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private BehavioralBiometrics()
    {
        KeystrokePatterns = new List<KeystrokePattern>();
        MotionPatterns = new List<MotionPattern>();
        BehavioralSessions = new List<BehavioralSession>();
        AnomalyDetections = new List<AnomalyDetection>();
        BaselineMetrics = new Dictionary<string, double>();
    }

    /// <summary>
    /// Initializes a new instance of the BehavioralBiometrics aggregate root.
    /// </summary>
    /// <param name="id">The behavioral biometrics identifier.</param>
    /// <param name="identityId">The identity this biometrics profile belongs to.</param>
    /// <param name="deviceFingerprint">The device fingerprint for this profile.</param>
    public BehavioralBiometrics(
        BehavioralBiometricsId id,
        IdentityId identityId,
        string deviceFingerprint)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        DeviceFingerprint = deviceFingerprint ?? throw new ArgumentNullException(nameof(deviceFingerprint));
        Status = BiometricStatus.Enrollment;
        CreatedAt = DateTime.UtcNow;
        LastActivityAt = CreatedAt;
        ConfidenceScore = 0;
        KeystrokePatterns = new List<KeystrokePattern>();
        MotionPatterns = new List<MotionPattern>();
        BehavioralSessions = new List<BehavioralSession>();
        AnomalyDetections = new List<AnomalyDetection>();
        BaselineMetrics = new Dictionary<string, double>();
        Version = 1;

        AddEvent(new BehavioralBiometricsProfileCreated(Id, IdentityId, DeviceFingerprint, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity this biometrics profile belongs to.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The device fingerprint for this profile.
    /// </summary>
    public string DeviceFingerprint { get; private set; }

    /// <summary>
    /// The current status of the biometrics profile.
    /// </summary>
    public BiometricStatus Status { get; private set; }

    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the profile was last active.
    /// </summary>
    public DateTime LastActivityAt { get; private set; }

    /// <summary>
    /// The overall confidence score (0-100).
    /// </summary>
    public int ConfidenceScore { get; private set; }

    /// <summary>
    /// The number of enrollment sessions completed.
    /// </summary>
    public int EnrollmentSessionsCompleted { get; private set; }

    /// <summary>
    /// The keystroke patterns collected.
    /// </summary>
    public List<KeystrokePattern> KeystrokePatterns { get; private set; }

    /// <summary>
    /// The motion patterns collected.
    /// </summary>
    public List<MotionPattern> MotionPatterns { get; private set; }

    /// <summary>
    /// The behavioral sessions recorded.
    /// </summary>
    public List<BehavioralSession> BehavioralSessions { get; private set; }

    /// <summary>
    /// The anomaly detections recorded.
    /// </summary>
    public List<AnomalyDetection> AnomalyDetections { get; private set; }

    /// <summary>
    /// The baseline metrics for comparison.
    /// </summary>
    public Dictionary<string, double> BaselineMetrics { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Starts an enrollment session.
    /// </summary>
    public void StartEnrollmentSession()
    {
        if (Status != BiometricStatus.Enrollment && Status != BiometricStatus.Active)
            throw new InvalidOperationException("Enrollment can only be started from enrollment or active status");

        Status = BiometricStatus.Enrolling;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new EnrollmentSessionStarted(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Records keystroke data during enrollment or verification.
    /// </summary>
    /// <param name="keyEvents">The key events captured.</param>
    /// <param name="sessionType">The type of session (enrollment/verification).</param>
    public void RecordKeystrokeData(List<KeyEvent> keyEvents, SessionType sessionType)
    {
        if (Status != BiometricStatus.Enrolling && Status != BiometricStatus.Active)
            throw new InvalidOperationException("Keystroke data can only be recorded during enrollment or active sessions");

        var pattern = KeystrokePattern.FromKeyEvents(keyEvents, sessionType, DateTime.UtcNow);
        KeystrokePatterns.Add(pattern);
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new KeystrokeDataRecorded(Id, keyEvents.Count, sessionType, DateTime.UtcNow));
    }

    /// <summary>
    /// Records motion data during enrollment or verification.
    /// </summary>
    /// <param name="motionEvents">The motion events captured.</param>
    /// <param name="sessionType">The type of session (enrollment/verification).</param>
    public void RecordMotionData(List<MotionEvent> motionEvents, SessionType sessionType)
    {
        if (Status != BiometricStatus.Enrolling && Status != BiometricStatus.Active)
            throw new InvalidOperationException("Motion data can only be recorded during enrollment or active sessions");

        var pattern = MotionPattern.FromMotionEvents(motionEvents, sessionType, DateTime.UtcNow);
        MotionPatterns.Add(pattern);
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new MotionDataRecorded(Id, motionEvents.Count, sessionType, DateTime.UtcNow));
    }

    /// <summary>
    /// Completes an enrollment session.
    /// </summary>
    public void CompleteEnrollmentSession()
    {
        if (Status != BiometricStatus.Enrolling)
            throw new InvalidOperationException("Enrollment session must be in progress to complete");

        EnrollmentSessionsCompleted++;
        Status = BiometricStatus.Active;
        UpdateBaselineMetrics();
        UpdateConfidenceScore();
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new EnrollmentSessionCompleted(Id, EnrollmentSessionsCompleted, ConfidenceScore, DateTime.UtcNow));
    }

    /// <summary>
    /// Performs behavioral verification.
    /// </summary>
    /// <param name="keystrokeData">The keystroke data to verify.</param>
    /// <param name="motionData">The motion data to verify.</param>
    /// <returns>The verification result.</returns>
    public BehavioralVerificationResult VerifyBehavior(
        List<KeyEvent>? keystrokeData = null,
        List<MotionEvent>? motionData = null)
    {
        if (Status != BiometricStatus.Active)
            throw new InvalidOperationException("Behavioral verification can only be performed on active profiles");

        var keystrokeScore = keystrokeData != null ? CalculateKeystrokeScore(keystrokeData) : 50;
        var motionScore = motionData != null ? CalculateMotionScore(motionData) : 50;
        var overallScore = (keystrokeScore + motionScore) / 2;

        var result = new BehavioralVerificationResult(
            overallScore >= 70, // Threshold for acceptance
            overallScore,
            keystrokeScore,
            motionScore,
            DateTime.UtcNow);

        // Record the session
        var session = new BehavioralSession(
            Guid.NewGuid().ToString(),
            SessionType.Verification,
            result.IsMatch,
            overallScore,
            DateTime.UtcNow);

        BehavioralSessions.Add(session);

        // Check for anomalies
        if (overallScore < 50)
        {
            var anomaly = new AnomalyDetection(
                Guid.NewGuid().ToString(),
                AnomalyType.BehavioralDeviation,
                $"Low confidence score: {overallScore}",
                overallScore,
                DateTime.UtcNow);

            AnomalyDetections.Add(anomaly);

            AddEvent(new BehavioralAnomalyDetected(Id, overallScore, "Low confidence behavioral match", DateTime.UtcNow));
        }

        UpdateConfidenceScore();
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new BehavioralVerificationPerformed(Id, result.IsMatch, overallScore, DateTime.UtcNow));

        return result;
    }

    /// <summary>
    /// Updates the baseline metrics after enrollment.
    /// </summary>
    private void UpdateBaselineMetrics()
    {
        // Calculate average metrics from enrollment sessions
        var enrollmentKeystrokes = KeystrokePatterns.Where(k => k.SessionType == SessionType.Enrollment).ToList();
        var enrollmentMotions = MotionPatterns.Where(m => m.SessionType == SessionType.Enrollment).ToList();

        if (enrollmentKeystrokes.Any())
        {
            BaselineMetrics["avg_keystroke_latency"] = enrollmentKeystrokes.Average(k => k.AverageLatency);
            BaselineMetrics["avg_keystroke_pressure"] = enrollmentKeystrokes.Average(k => k.AveragePressure);
        }

        if (enrollmentMotions.Any())
        {
            BaselineMetrics["avg_motion_speed"] = enrollmentMotions.Average(m => m.AverageSpeed);
            BaselineMetrics["avg_motion_acceleration"] = enrollmentMotions.Average(m => m.AverageAcceleration);
        }
    }

    /// <summary>
    /// Updates the overall confidence score.
    /// </summary>
    private void UpdateConfidenceScore()
    {
        // Base score from enrollment completion
        var baseScore = Math.Min(EnrollmentSessionsCompleted * 20, 60);

        // Bonus for recent successful verifications
        var recentVerifications = BehavioralSessions
            .Where(s => s.SessionType == SessionType.Verification &&
                       s.IsSuccessful &&
                       (DateTime.UtcNow - s.SessionTime) < TimeSpan.FromDays(30))
            .Count();

        var verificationBonus = Math.Min(recentVerifications * 5, 30);

        // Penalty for anomalies
        var recentAnomalies = AnomalyDetections
            .Count(a => (DateTime.UtcNow - a.DetectedAt) < TimeSpan.FromDays(7));

        var anomalyPenalty = recentAnomalies * 10;

        ConfidenceScore = Math.Clamp(baseScore + verificationBonus - anomalyPenalty, 0, 100);
    }

    /// <summary>
    /// Calculates the keystroke verification score.
    /// </summary>
    private int CalculateKeystrokeScore(List<KeyEvent> keystrokeData)
    {
        if (!BaselineMetrics.ContainsKey("avg_keystroke_latency"))
            return 50; // No baseline available

        var pattern = KeystrokePattern.FromKeyEvents(keystrokeData, SessionType.Verification, DateTime.UtcNow);
        var latencyDiff = Math.Abs(pattern.AverageLatency - BaselineMetrics["avg_keystroke_latency"]);
        var pressureDiff = Math.Abs(pattern.AveragePressure - BaselineMetrics["avg_keystroke_pressure"]);

        // Calculate similarity score (lower difference = higher score)
        var latencyScore = Math.Max(0, 100 - (latencyDiff * 10));
        var pressureScore = Math.Max(0, 100 - (pressureDiff * 10));

        return (int)((latencyScore + pressureScore) / 2);
    }

    /// <summary>
    /// Calculates the motion verification score.
    /// </summary>
    private int CalculateMotionScore(List<MotionEvent> motionData)
    {
        if (!BaselineMetrics.ContainsKey("avg_motion_speed"))
            return 50; // No baseline available

        var pattern = MotionPattern.FromMotionEvents(motionData, SessionType.Verification, DateTime.UtcNow);
        var speedDiff = Math.Abs(pattern.AverageSpeed - BaselineMetrics["avg_motion_speed"]);
        var accelerationDiff = Math.Abs(pattern.AverageAcceleration - BaselineMetrics["avg_motion_acceleration"]);

        // Calculate similarity score (lower difference = higher score)
        var speedScore = Math.Max(0, 100 - (speedDiff * 10));
        var accelerationScore = Math.Max(0, 100 - (accelerationDiff * 10));

        return (int)((speedScore + accelerationScore) / 2);
    }

    /// <summary>
    /// Checks if the profile is ready for verification.
    /// </summary>
    /// <returns>True if the profile has sufficient enrollment data.</returns>
    public bool IsReadyForVerification()
    {
        return Status == BiometricStatus.Active &&
               EnrollmentSessionsCompleted >= 3 &&
               KeystrokePatterns.Count >= 10 &&
               MotionPatterns.Count >= 5;
    }

    #endregion
}

/// <summary>
/// Represents the status of a biometric profile.
/// </summary>
internal enum BiometricStatus
{
    Enrollment,
    Enrolling,
    Active,
    Suspended,
    Disabled
}

/// <summary>
/// Represents the type of session.
/// </summary>
internal enum SessionType
{
    Enrollment,
    Verification
}

/// <summary>
/// Represents the type of anomaly.
/// </summary>
internal enum AnomalyType
{
    BehavioralDeviation,
    TimingAnomaly,
    PatternMismatch,
    DeviceChange
}

/// <summary>
/// Represents a keystroke pattern.
/// </summary>
internal class KeystrokePattern
{
    public string PatternId { get; set; }
    public SessionType SessionType { get; set; }
    public List<KeyEvent> KeyEvents { get; set; } = new();
    public double AverageLatency { get; set; }
    public double AveragePressure { get; set; }
    public DateTime RecordedAt { get; set; }

    public KeystrokePattern(
        string patternId,
        SessionType sessionType,
        List<KeyEvent> keyEvents,
        DateTime recordedAt)
    {
        PatternId = patternId;
        SessionType = sessionType;
        KeyEvents = keyEvents;
        RecordedAt = recordedAt;

        // Calculate metrics
        if (keyEvents.Any())
        {
            AverageLatency = keyEvents.Average(e => e.KeyDownTime - (keyEvents.FirstOrDefault()?.KeyDownTime ?? 0));
            AveragePressure = keyEvents.Average(e => e.Pressure);
        }
    }

    public static KeystrokePattern FromKeyEvents(List<KeyEvent> keyEvents, SessionType sessionType, DateTime recordedAt)
    {
        return new KeystrokePattern(
            Guid.NewGuid().ToString(),
            sessionType,
            keyEvents,
            recordedAt);
    }
}

/// <summary>
/// Represents a motion pattern.
/// </summary>
internal class MotionPattern
{
    public string PatternId { get; set; }
    public SessionType SessionType { get; set; }
    public List<MotionEvent> MotionEvents { get; set; } = new();
    public double AverageSpeed { get; set; }
    public double AverageAcceleration { get; set; }
    public DateTime RecordedAt { get; set; }

    public MotionPattern(
        string patternId,
        SessionType sessionType,
        List<MotionEvent> motionEvents,
        DateTime recordedAt)
    {
        PatternId = patternId;
        SessionType = sessionType;
        MotionEvents = motionEvents;
        RecordedAt = recordedAt;

        // Calculate metrics
        if (motionEvents.Any())
        {
            AverageSpeed = motionEvents.Average(e => e.Speed);
            AverageAcceleration = motionEvents.Average(e => e.Acceleration);
        }
    }

    public static MotionPattern FromMotionEvents(List<MotionEvent> motionEvents, SessionType sessionType, DateTime recordedAt)
    {
        return new MotionPattern(
            Guid.NewGuid().ToString(),
            sessionType,
            motionEvents,
            recordedAt);
    }
}

/// <summary>
/// Represents a behavioral session.
/// </summary>
internal class BehavioralSession
{
    public string SessionId { get; set; }
    public SessionType SessionType { get; set; }
    public bool IsSuccessful { get; set; }
    public int Score { get; set; }
    public DateTime SessionTime { get; set; }

    public BehavioralSession(
        string sessionId,
        SessionType sessionType,
        bool isSuccessful,
        int score,
        DateTime sessionTime)
    {
        SessionId = sessionId;
        SessionType = sessionType;
        IsSuccessful = isSuccessful;
        Score = score;
        SessionTime = sessionTime;
    }
}

/// <summary>
/// Represents an anomaly detection.
/// </summary>
internal class AnomalyDetection
{
    public string DetectionId { get; set; }
    public AnomalyType AnomalyType { get; set; }
    public string Description { get; set; }
    public double SeverityScore { get; set; }
    public DateTime DetectedAt { get; set; }

    public AnomalyDetection(
        string detectionId,
        AnomalyType anomalyType,
        string description,
        double severityScore,
        DateTime detectedAt)
    {
        DetectionId = detectionId;
        AnomalyType = anomalyType;
        Description = description;
        SeverityScore = severityScore;
        DetectedAt = detectedAt;
    }
}

/// <summary>
/// Represents a key event.
/// </summary>
internal class KeyEvent
{
    public string Key { get; set; }
    public double KeyDownTime { get; set; }
    public double KeyUpTime { get; set; }
    public double Pressure { get; set; }

    public KeyEvent(string key, double keyDownTime, double keyUpTime, double pressure)
    {
        Key = key;
        KeyDownTime = keyDownTime;
        KeyUpTime = keyUpTime;
        Pressure = pressure;
    }
}

/// <summary>
/// Represents a motion event.
/// </summary>
internal class MotionEvent
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Speed { get; set; }
    public double Acceleration { get; set; }
    public double Timestamp { get; set; }

    public MotionEvent(double x, double y, double z, double speed, double acceleration, double timestamp)
    {
        X = x;
        Y = y;
        Z = z;
        Speed = speed;
        Acceleration = acceleration;
        Timestamp = timestamp;
    }
}

/// <summary>
/// Represents the result of behavioral verification.
/// </summary>
internal class BehavioralVerificationResult
{
    public bool IsMatch { get; set; }
    public int OverallScore { get; set; }
    public int KeystrokeScore { get; set; }
    public int MotionScore { get; set; }
    public DateTime VerifiedAt { get; set; }

    public BehavioralVerificationResult(
        bool isMatch,
        int overallScore,
        int keystrokeScore,
        int motionScore,
        DateTime verifiedAt)
    {
        IsMatch = isMatch;
        OverallScore = overallScore;
        KeystrokeScore = keystrokeScore;
        MotionScore = motionScore;
        VerifiedAt = verifiedAt;
    }
}
