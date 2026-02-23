using BioMetrics.Core.Models;

namespace BioMetrics.Core.Interfaces;

public interface IBiometricService
{
    Task<BiometricProfile?> GetProfileAsync(string userId);
    Task<BiometricEnrollment> EnrollAsync(string userId, BiometricType type, string data);
    Task<VerificationResult> VerifyAsync(string userId, BiometricType type, string data);
    Task<IEnumerable<BiometricDevice>> GetDevicesAsync(string userId);
    Task<BiometricDevice> RegisterDeviceAsync(DeviceRegistration registration);
    Task<IEnumerable<AuthenticationLog>> GetAuthenticationLogsAsync(string userId);
    Task<BiometricStats> GetStatsAsync();
}
