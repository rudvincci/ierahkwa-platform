// ============================================================================
// IERAHKWA SOVEREIGN PLATFORM - BIOMETRICS SERVICE
// Complete biometric authentication implementation
// ============================================================================

using System.Security.Cryptography;
using System.Text;

namespace BioMetrics.Core;

public enum BiometricType
{
    Fingerprint,
    FaceId,
    Voice,
    Retina,
    Palm
}

public class BiometricTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public BiometricType Type { get; set; }
    public string TemplateHash { get; set; } = "";
    public string DeviceId { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
    public int FailedAttempts { get; set; }
}

public class BiometricVerificationResult
{
    public bool Success { get; set; }
    public double MatchScore { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
}

public class BiometricEnrollmentRequest
{
    public string UserId { get; set; } = "";
    public BiometricType Type { get; set; }
    public byte[] BiometricData { get; set; } = Array.Empty<byte>();
    public string DeviceId { get; set; } = "";
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class BiometricVerificationRequest
{
    public string UserId { get; set; } = "";
    public BiometricType Type { get; set; }
    public byte[] BiometricData { get; set; } = Array.Empty<byte>();
    public string DeviceId { get; set; } = "";
    public double MinMatchScore { get; set; } = 0.85;
}

public interface IBiometricService
{
    Task<BiometricTemplate> EnrollAsync(BiometricEnrollmentRequest request);
    Task<BiometricVerificationResult> VerifyAsync(BiometricVerificationRequest request);
    Task<bool> DeleteTemplateAsync(string userId, BiometricType type);
    Task<IEnumerable<BiometricTemplate>> GetUserTemplatesAsync(string userId);
    Task<bool> IsEnrolledAsync(string userId, BiometricType type);
}

public interface IBiometricRepository
{
    Task<BiometricTemplate?> GetAsync(string userId, BiometricType type);
    Task<IEnumerable<BiometricTemplate>> GetByUserAsync(string userId);
    Task SaveAsync(BiometricTemplate template);
    Task DeleteAsync(string userId, BiometricType type);
    Task UpdateLastUsedAsync(string templateId);
    Task IncrementFailedAttemptsAsync(string templateId);
    Task ResetFailedAttemptsAsync(string templateId);
}

public class BiometricService : IBiometricService
{
    private readonly IBiometricRepository _repository;
    private readonly IBiometricProcessor _processor;
    private readonly IAuditLogger _auditLogger;
    private const int MaxFailedAttempts = 5;

    public BiometricService(
        IBiometricRepository repository, 
        IBiometricProcessor processor,
        IAuditLogger auditLogger)
    {
        _repository = repository;
        _processor = processor;
        _auditLogger = auditLogger;
    }

    public async Task<BiometricTemplate> EnrollAsync(BiometricEnrollmentRequest request)
    {
        // Check if already enrolled
        var existing = await _repository.GetAsync(request.UserId, request.Type);
        if (existing != null && existing.IsActive)
        {
            throw new InvalidOperationException($"User already enrolled for {request.Type}");
        }

        // Process biometric data to create template
        var processedTemplate = await _processor.CreateTemplateAsync(request.BiometricData, request.Type);
        
        // Hash the template for secure storage
        var templateHash = HashTemplate(processedTemplate);

        var template = new BiometricTemplate
        {
            UserId = request.UserId,
            Type = request.Type,
            TemplateHash = templateHash,
            DeviceId = request.DeviceId,
            IsActive = true
        };

        await _repository.SaveAsync(template);
        
        await _auditLogger.LogAsync("BIOMETRIC_ENROLL", request.UserId, new
        {
            Type = request.Type.ToString(),
            DeviceId = request.DeviceId
        });

        return template;
    }

    public async Task<BiometricVerificationResult> VerifyAsync(BiometricVerificationRequest request)
    {
        var template = await _repository.GetAsync(request.UserId, request.Type);
        
        if (template == null || !template.IsActive)
        {
            return new BiometricVerificationResult
            {
                Success = false,
                ErrorMessage = "No active biometric template found",
                MatchScore = 0
            };
        }

        // Check lockout
        if (template.FailedAttempts >= MaxFailedAttempts)
        {
            return new BiometricVerificationResult
            {
                Success = false,
                ErrorMessage = "Biometric locked due to too many failed attempts",
                MatchScore = 0
            };
        }

        // Process incoming biometric
        var processedInput = await _processor.CreateTemplateAsync(request.BiometricData, request.Type);
        var inputHash = HashTemplate(processedInput);

        // Compare templates
        var matchScore = await _processor.CompareTemplatesAsync(
            template.TemplateHash, 
            inputHash, 
            request.Type
        );

        var success = matchScore >= request.MinMatchScore;

        if (success)
        {
            await _repository.UpdateLastUsedAsync(template.Id);
            await _repository.ResetFailedAttemptsAsync(template.Id);
            
            await _auditLogger.LogAsync("BIOMETRIC_VERIFY_SUCCESS", request.UserId, new
            {
                Type = request.Type.ToString(),
                MatchScore = matchScore
            });
        }
        else
        {
            await _repository.IncrementFailedAttemptsAsync(template.Id);
            
            await _auditLogger.LogAsync("BIOMETRIC_VERIFY_FAILED", request.UserId, new
            {
                Type = request.Type.ToString(),
                MatchScore = matchScore,
                FailedAttempts = template.FailedAttempts + 1
            });
        }

        return new BiometricVerificationResult
        {
            Success = success,
            MatchScore = matchScore,
            UserId = request.UserId
        };
    }

    public async Task<bool> DeleteTemplateAsync(string userId, BiometricType type)
    {
        await _repository.DeleteAsync(userId, type);
        await _auditLogger.LogAsync("BIOMETRIC_DELETE", userId, new { Type = type.ToString() });
        return true;
    }

    public async Task<IEnumerable<BiometricTemplate>> GetUserTemplatesAsync(string userId)
    {
        return await _repository.GetByUserAsync(userId);
    }

    public async Task<bool> IsEnrolledAsync(string userId, BiometricType type)
    {
        var template = await _repository.GetAsync(userId, type);
        return template != null && template.IsActive;
    }

    private string HashTemplate(byte[] template)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(template);
        return Convert.ToBase64String(hash);
    }
}

// Biometric processor interface
public interface IBiometricProcessor
{
    Task<byte[]> CreateTemplateAsync(byte[] rawData, BiometricType type);
    Task<double> CompareTemplatesAsync(string template1Hash, string template2Hash, BiometricType type);
}

// Simulated processor (replace with actual SDK integration)
public class SimulatedBiometricProcessor : IBiometricProcessor
{
    public Task<byte[]> CreateTemplateAsync(byte[] rawData, BiometricType type)
    {
        // In production, use actual biometric SDK
        // This simulates feature extraction
        using var sha256 = SHA256.Create();
        var features = sha256.ComputeHash(rawData);
        
        // Add type-specific processing marker
        var typeMarker = Encoding.UTF8.GetBytes(type.ToString());
        var combined = new byte[features.Length + typeMarker.Length];
        Buffer.BlockCopy(features, 0, combined, 0, features.Length);
        Buffer.BlockCopy(typeMarker, 0, combined, features.Length, typeMarker.Length);
        
        return Task.FromResult(sha256.ComputeHash(combined));
    }

    public Task<double> CompareTemplatesAsync(string template1Hash, string template2Hash, BiometricType type)
    {
        // In production, use actual matching algorithm
        // This simulates a fuzzy match
        if (template1Hash == template2Hash)
            return Task.FromResult(1.0);

        // Calculate similarity based on hash
        var bytes1 = Convert.FromBase64String(template1Hash);
        var bytes2 = Convert.FromBase64String(template2Hash);

        int matching = 0;
        int total = Math.Min(bytes1.Length, bytes2.Length);
        
        for (int i = 0; i < total; i++)
        {
            if (bytes1[i] == bytes2[i]) matching++;
        }

        return Task.FromResult((double)matching / total);
    }
}

// Audit logger interface
public interface IAuditLogger
{
    Task LogAsync(string action, string userId, object data);
}

public class ConsoleAuditLogger : IAuditLogger
{
    public Task LogAsync(string action, string userId, object data)
    {
        Console.WriteLine($"[AUDIT] {DateTime.UtcNow:O} | {action} | User: {userId} | Data: {System.Text.Json.JsonSerializer.Serialize(data)}");
        return Task.CompletedTask;
    }
}

// In-memory repository for development
public class InMemoryBiometricRepository : IBiometricRepository
{
    private readonly Dictionary<string, BiometricTemplate> _templates = new();
    private readonly object _lock = new();

    private string GetKey(string userId, BiometricType type) => $"{userId}:{type}";

    public Task<BiometricTemplate?> GetAsync(string userId, BiometricType type)
    {
        lock (_lock)
        {
            _templates.TryGetValue(GetKey(userId, type), out var template);
            return Task.FromResult(template);
        }
    }

    public Task<IEnumerable<BiometricTemplate>> GetByUserAsync(string userId)
    {
        lock (_lock)
        {
            var templates = _templates.Values
                .Where(t => t.UserId == userId)
                .ToList();
            return Task.FromResult<IEnumerable<BiometricTemplate>>(templates);
        }
    }

    public Task SaveAsync(BiometricTemplate template)
    {
        lock (_lock)
        {
            _templates[GetKey(template.UserId, template.Type)] = template;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string userId, BiometricType type)
    {
        lock (_lock)
        {
            _templates.Remove(GetKey(userId, type));
        }
        return Task.CompletedTask;
    }

    public Task UpdateLastUsedAsync(string templateId)
    {
        lock (_lock)
        {
            var template = _templates.Values.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            {
                template.LastUsedAt = DateTime.UtcNow;
            }
        }
        return Task.CompletedTask;
    }

    public Task IncrementFailedAttemptsAsync(string templateId)
    {
        lock (_lock)
        {
            var template = _templates.Values.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            {
                template.FailedAttempts++;
            }
        }
        return Task.CompletedTask;
    }

    public Task ResetFailedAttemptsAsync(string templateId)
    {
        lock (_lock)
        {
            var template = _templates.Values.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            {
                template.FailedAttempts = 0;
            }
        }
        return Task.CompletedTask;
    }
}
