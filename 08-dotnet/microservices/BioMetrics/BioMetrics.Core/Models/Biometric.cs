namespace BioMetrics.Core.Models;

public class BiometricProfile
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<BiometricEnrollment> Enrollments { get; set; } = new();
    public List<BiometricDevice> Devices { get; set; } = new();
    public BiometricSettings Settings { get; set; } = new();
    public int TotalAuthentications { get; set; }
    public decimal SuccessRate { get; set; }
    public DateTime LastAuthentication { get; set; }
}

public class BiometricEnrollment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public BiometricType Type { get; set; }
    public string TemplateHash { get; set; } = string.Empty;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
    public int UseCount { get; set; }
}

public enum BiometricType
{
    Fingerprint,
    FaceId,
    IrisScan,
    VoicePrint,
    Retina
}

public enum EnrollmentStatus
{
    Active,
    Disabled,
    Expired,
    Revoked
}

public class BiometricDevice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // iPhone, MacBook, Watch
    public List<BiometricType> SupportedTypes { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
}

public class DeviceRegistration
{
    public string UserId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public List<BiometricType> SupportedTypes { get; set; } = new();
}

public class VerificationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public BiometricType Type { get; set; }
    public decimal Confidence { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class AuthenticationLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public BiometricType Type { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class BiometricSettings
{
    public bool MultiFactorRequired { get; set; } = true;
    public bool QuickAuthEnabled { get; set; } = true;
    public bool LoginAlerts { get; set; } = true;
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(30);
}

public class BiometricStats
{
    public int EnrolledMethods { get; set; }
    public int AuthenticationsToday { get; set; }
    public decimal SuccessRate { get; set; }
    public int RegisteredDevices { get; set; }
}
