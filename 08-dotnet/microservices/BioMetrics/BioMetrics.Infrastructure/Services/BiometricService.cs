using BioMetrics.Core.Interfaces;
using BioMetrics.Core.Models;

namespace BioMetrics.Infrastructure.Services;

public class BiometricService : IBiometricService
{
    private static readonly Dictionary<string, BiometricProfile> _profiles = new()
    {
        ["CIT-001"] = new BiometricProfile
        {
            UserId = "CIT-001",
            UserName = "Ruddie Salomon",
            TotalAuthentications = 156,
            SuccessRate = 99.8m,
            LastAuthentication = DateTime.UtcNow.AddMinutes(-5),
            Enrollments = new List<BiometricEnrollment>
            {
                new() { Type = BiometricType.Fingerprint, Status = EnrollmentStatus.Active, UseCount = 45 },
                new() { Type = BiometricType.FaceId, Status = EnrollmentStatus.Active, UseCount = 89 },
                new() { Type = BiometricType.IrisScan, Status = EnrollmentStatus.Active, UseCount = 12 },
                new() { Type = BiometricType.VoicePrint, Status = EnrollmentStatus.Active, UseCount = 10 }
            },
            Devices = new List<BiometricDevice>
            {
                new() { DeviceName = "iPhone 15 Pro", DeviceType = "iPhone", IsActive = true, 
                        SupportedTypes = new() { BiometricType.FaceId, BiometricType.Fingerprint } },
                new() { DeviceName = "MacBook Pro", DeviceType = "MacBook", IsActive = true,
                        SupportedTypes = new() { BiometricType.Fingerprint } },
                new() { DeviceName = "Apple Watch", DeviceType = "Watch", IsActive = false,
                        SupportedTypes = new() { BiometricType.Fingerprint } }
            }
        }
    };

    private static readonly List<AuthenticationLog> _logs = new()
    {
        new() { UserId = "CIT-001", Type = BiometricType.FaceId, DeviceName = "iPhone 15 Pro", Success = true },
        new() { UserId = "CIT-001", Type = BiometricType.Fingerprint, DeviceName = "MacBook Pro", Success = true, 
                Timestamp = DateTime.UtcNow.AddHours(-2) },
        new() { UserId = "CIT-001", Type = BiometricType.FaceId, DeviceName = "iPhone 15 Pro", Success = true,
                Timestamp = DateTime.UtcNow.AddDays(-1) },
        new() { UserId = "CIT-001", Type = BiometricType.VoicePrint, DeviceName = "iPhone 15 Pro", Success = false,
                FailureReason = "Voice mismatch", Timestamp = DateTime.UtcNow.AddDays(-2) }
    };

    public Task<BiometricProfile?> GetProfileAsync(string userId) =>
        Task.FromResult(_profiles.GetValueOrDefault(userId));

    public Task<BiometricEnrollment> EnrollAsync(string userId, BiometricType type, string data)
    {
        var enrollment = new BiometricEnrollment
        {
            UserId = userId,
            Type = type,
            TemplateHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(data)))
        };

        if (_profiles.TryGetValue(userId, out var profile))
        {
            profile.Enrollments.Add(enrollment);
        }

        return Task.FromResult(enrollment);
    }

    public Task<VerificationResult> VerifyAsync(string userId, BiometricType type, string data)
    {
        var result = new VerificationResult
        {
            Success = true,
            Message = "Authentication successful",
            Type = type,
            Confidence = 99.5m,
            TransactionId = Guid.NewGuid().ToString()
        };

        _logs.Add(new AuthenticationLog
        {
            UserId = userId,
            Type = type,
            Success = true,
            DeviceName = "Current Device"
        });

        return Task.FromResult(result);
    }

    public Task<IEnumerable<BiometricDevice>> GetDevicesAsync(string userId)
    {
        if (_profiles.TryGetValue(userId, out var profile))
            return Task.FromResult(profile.Devices.AsEnumerable());
        return Task.FromResult(Enumerable.Empty<BiometricDevice>());
    }

    public Task<BiometricDevice> RegisterDeviceAsync(DeviceRegistration registration)
    {
        var device = new BiometricDevice
        {
            UserId = registration.UserId,
            DeviceName = registration.DeviceName,
            DeviceType = registration.DeviceType,
            SupportedTypes = registration.SupportedTypes
        };

        if (_profiles.TryGetValue(registration.UserId, out var profile))
        {
            profile.Devices.Add(device);
        }

        return Task.FromResult(device);
    }

    public Task<IEnumerable<AuthenticationLog>> GetAuthenticationLogsAsync(string userId) =>
        Task.FromResult(_logs.Where(l => l.UserId == userId).OrderByDescending(l => l.Timestamp));

    public Task<BiometricStats> GetStatsAsync()
    {
        return Task.FromResult(new BiometricStats
        {
            EnrolledMethods = 4,
            AuthenticationsToday = 156,
            SuccessRate = 99.8m,
            RegisteredDevices = 3
        });
    }
}
