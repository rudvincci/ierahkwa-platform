using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Infrastructure.Metrics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.Biometric;

/// <summary>
/// Service for biometric authentication with blockchain verification.
/// Stores biometric template hashes on MameyNode blockchain and verifies against them.
/// 
/// TDD Reference: Lines 1594-1703 (Identity), Lines 1822-1895 (Biometric Verification)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
internal sealed class BiometricBlockchainService : IBiometricBlockchainService
{
    private readonly IGovernmentIdentityClient _governmentClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BiometricBlockchainService> _logger;
    private readonly BiometricBlockchainOptions _options;

    // Prometheus metrics
    private static readonly Counter BiometricEnrollments = PrometheusMetrics
        .CreateCounter(
            "blockchain_biometric_enrollments_total",
            "Total biometric enrollments",
            new CounterConfiguration
            {
                LabelNames = new[] { "modality", "status" }
            });

    private static readonly Counter BiometricVerifications = PrometheusMetrics
        .CreateCounter(
            "blockchain_biometric_verifications_total",
            "Total biometric verifications",
            new CounterConfiguration
            {
                LabelNames = new[] { "modality", "result" }
            });

    private static readonly Histogram BiometricVerificationDuration = PrometheusMetrics
        .CreateHistogram(
            "blockchain_biometric_verification_duration_seconds",
            "Biometric verification duration",
            new HistogramConfiguration
            {
                Buckets = new[] { 0.1, 0.25, 0.5, 1.0, 2.0, 5.0 }
            });

    public BiometricBlockchainService(
        IGovernmentIdentityClient governmentClient,
        IMemoryCache cache,
        ILogger<BiometricBlockchainService> logger,
        IConfiguration configuration)
    {
        _governmentClient = governmentClient ?? throw new ArgumentNullException(nameof(governmentClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options = new BiometricBlockchainOptions();
        configuration.GetSection("biometricBlockchain").Bind(_options);
    }

    /// <inheritdoc />
    public async Task<BiometricEnrollmentResult> EnrollBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string templateHash,
        BiometricMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Enrolling biometric on blockchain: IdentityId={IdentityId}, Modality={Modality}",
            identityId, modality);

        try
        {
            if (!_options.Enabled)
            {
                _logger.LogDebug("Biometric blockchain service is disabled");
                return new BiometricEnrollmentResult
                {
                    Success = false,
                    ErrorMessage = "Biometric blockchain service is disabled"
                };
            }

            // Create enrollment event data
            var enrollmentData = new Dictionary<string, string>
            {
                { "IdentityId", identityId.ToString() },
                { "Modality", modality.ToString() },
                { "TemplateHash", templateHash },
                { "EnrolledAt", DateTime.UtcNow.ToString("O") }
            };

            if (metadata != null)
            {
                if (!string.IsNullOrEmpty(metadata.DeviceId))
                    enrollmentData["DeviceId"] = metadata.DeviceId;
                if (metadata.QualityScore.HasValue)
                    enrollmentData["QualityScore"] = metadata.QualityScore.Value.ToString("F4");
                if (!string.IsNullOrEmpty(metadata.Algorithm))
                    enrollmentData["Algorithm"] = metadata.Algorithm;
            }

            // Log to blockchain via Government Identity service
            // Note: We're creating a biometric enrollment record as a government identity
            var response = await _governmentClient.CreateIdentityAsync(
                new Application.Clients.CreateIdentityRequest(
                    CitizenId: identityId.ToString(),
                    FirstName: $"BiometricEnrollment_{modality}",
                    LastName: templateHash,
                    DateOfBirth: DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    Nationality: "Biometric",
                    Metadata: enrollmentData
                ), cancellationToken);

            if (response.Success)
            {
                BiometricEnrollments.WithLabels(modality.ToString(), "success").Inc();

                // Cache the enrollment
                var cacheKey = GetEnrollmentCacheKey(identityId, modality);
                var enrollmentInfo = new EnrolledBiometricInfo
                {
                    Modality = modality,
                    EnrollmentId = response.BlockchainAccount,
                    TemplateHash = templateHash,
                    EnrolledAt = DateTime.UtcNow,
                    IsActive = true
                };
                _cache.Set(cacheKey, enrollmentInfo, TimeSpan.FromMinutes(_options.CacheDurationMinutes));

                _logger.LogInformation(
                    "Biometric enrolled successfully: IdentityId={IdentityId}, Modality={Modality}, EnrollmentId={EnrollmentId}",
                    identityId, modality, response.BlockchainAccount);

                return new BiometricEnrollmentResult
                {
                    Success = true,
                    EnrollmentId = response.BlockchainAccount,
                    TransactionHash = response.IdentityId, // Use IdentityId as transaction reference
                    EnrolledAt = DateTime.UtcNow
                };
            }

            BiometricEnrollments.WithLabels(modality.ToString(), "failure").Inc();
            _logger.LogWarning(
                "Failed to enroll biometric: IdentityId={IdentityId}, Error={Error}",
                identityId, response.ErrorMessage);

            return new BiometricEnrollmentResult
            {
                Success = false,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            BiometricEnrollments.WithLabels(modality.ToString(), "error").Inc();
            _logger.LogError(ex, "Error enrolling biometric: IdentityId={IdentityId}", identityId);

            return new BiometricEnrollmentResult
            {
                Success = false,
                ErrorMessage = $"Enrollment error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<BiometricVerificationResult> VerifyBiometricOnBlockchainAsync(
        Guid identityId,
        BiometricModality modality,
        string templateHash,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        _logger.LogDebug(
            "Verifying biometric against blockchain: IdentityId={IdentityId}, Modality={Modality}",
            identityId, modality);

        try
        {
            if (!_options.Enabled)
            {
                return new BiometricVerificationResult
                {
                    IsVerified = false,
                    ErrorMessage = "Biometric blockchain service is disabled"
                };
            }

            // Try cache first
            var cacheKey = GetEnrollmentCacheKey(identityId, modality);
            EnrolledBiometricInfo? enrollment = null;

            if (_cache.TryGetValue(cacheKey, out EnrolledBiometricInfo? cachedEnrollment))
            {
                enrollment = cachedEnrollment;
            }
            else
            {
                // Fetch from blockchain
                var response = await _governmentClient.GetIdentityAsync(
                    identityId.ToString(),
                    cancellationToken);

                if (response?.Success == true)
                {
                    // GetIdentityResult contains FirstName which stores the enrollment type
                    // and LastName which stores the template hash
                    var storedHash = response.LastName;
                    if (!string.IsNullOrEmpty(storedHash))
                    {
                        enrollment = new EnrolledBiometricInfo
                        {
                            Modality = modality,
                            EnrollmentId = response.IdentityId,
                            TemplateHash = storedHash,
                            EnrolledAt = DateTime.TryParse(response.DateOfBirth, out var dt) ? dt : DateTime.UtcNow,
                            IsActive = response.Status == "Active"
                        };

                        // Cache it
                        _cache.Set(cacheKey, enrollment, TimeSpan.FromMinutes(_options.CacheDurationMinutes));
                    }
                }
            }

            if (enrollment == null)
            {
                BiometricVerifications.WithLabels(modality.ToString(), "not_enrolled").Inc();
                BiometricVerificationDuration.Observe((DateTime.UtcNow - startTime).TotalSeconds);

                return new BiometricVerificationResult
                {
                    IsVerified = false,
                    IsEnrolled = false,
                    ErrorMessage = "Biometric not enrolled on blockchain"
                };
            }

            if (!enrollment.IsActive)
            {
                BiometricVerifications.WithLabels(modality.ToString(), "revoked").Inc();
                BiometricVerificationDuration.Observe((DateTime.UtcNow - startTime).TotalSeconds);

                return new BiometricVerificationResult
                {
                    IsVerified = false,
                    IsEnrolled = true,
                    IsRevoked = true,
                    EnrollmentId = enrollment.EnrollmentId,
                    FailureReason = "Biometric enrollment has been revoked"
                };
            }

            // Compare hashes
            var hashMatches = string.Equals(templateHash, enrollment.TemplateHash, StringComparison.Ordinal);
            var tamperDetected = !hashMatches && _options.TamperDetectionEnabled;

            if (tamperDetected)
            {
                _logger.LogWarning(
                    "Biometric tamper detected: IdentityId={IdentityId}, Modality={Modality}",
                    identityId, modality);
            }

            var result = new BiometricVerificationResult
            {
                IsVerified = hashMatches,
                HashMatches = hashMatches,
                IsEnrolled = true,
                IsRevoked = false,
                TamperDetected = tamperDetected,
                EnrollmentId = enrollment.EnrollmentId,
                EnrolledAt = enrollment.EnrolledAt
            };

            var metricLabel = hashMatches ? "success" : (tamperDetected ? "tamper" : "mismatch");
            BiometricVerifications.WithLabels(modality.ToString(), metricLabel).Inc();
            BiometricVerificationDuration.Observe((DateTime.UtcNow - startTime).TotalSeconds);

            _logger.LogInformation(
                "Biometric verification completed: IdentityId={IdentityId}, Modality={Modality}, Verified={Verified}",
                identityId, modality, hashMatches);

            return result;
        }
        catch (Exception ex)
        {
            BiometricVerifications.WithLabels(modality.ToString(), "error").Inc();
            BiometricVerificationDuration.Observe((DateTime.UtcNow - startTime).TotalSeconds);

            _logger.LogError(ex, "Error verifying biometric: IdentityId={IdentityId}", identityId);

            return new BiometricVerificationResult
            {
                IsVerified = false,
                ErrorMessage = $"Verification error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<BiometricBlockchainStatus> GetBiometricStatusAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting biometric status: IdentityId={IdentityId}", identityId);

        var status = new BiometricBlockchainStatus
        {
            IdentityId = identityId
        };

        try
        {
            // Check for each modality
            foreach (BiometricModality modality in Enum.GetValues<BiometricModality>())
            {
                var cacheKey = GetEnrollmentCacheKey(identityId, modality);
                if (_cache.TryGetValue(cacheKey, out EnrolledBiometricInfo? enrollment) && enrollment != null)
                {
                    status.EnrolledBiometrics.Add(enrollment);
                }
            }

            status.BiometricVerificationEnabled = _options.Enabled && status.EnrolledBiometrics.Any(e => e.IsActive);

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting biometric status: IdentityId={IdentityId}", identityId);
            return status;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RevokeBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Revoking biometric: IdentityId={IdentityId}, Modality={Modality}, Reason={Reason}",
            identityId, modality, reason ?? "Not specified");

        try
        {
            // Update on blockchain - UpdateIdentityAsync takes identityId and a dictionary of updates
            var updates = new Dictionary<string, string>
            {
                { "Status", "Revoked" },
                { "RevokedAt", DateTime.UtcNow.ToString("O") },
                { "RevocationReason", reason ?? "Not specified" }
            };
            var response = await _governmentClient.UpdateIdentityAsync(
                identityId.ToString(),
                updates,
                cancellationToken);

            if (response.Success)
            {
                // Invalidate cache
                var cacheKey = GetEnrollmentCacheKey(identityId, modality);
                _cache.Remove(cacheKey);

                _logger.LogInformation(
                    "Biometric revoked successfully: IdentityId={IdentityId}, Modality={Modality}",
                    identityId, modality);

                return true;
            }

            _logger.LogWarning(
                "Failed to revoke biometric: IdentityId={IdentityId}, Error={Error}",
                identityId, response.ErrorMessage);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking biometric: IdentityId={IdentityId}", identityId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<BiometricEnrollmentResult> UpdateBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string newTemplateHash,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating biometric: IdentityId={IdentityId}, Modality={Modality}",
            identityId, modality);

        // Get existing enrollment
        var cacheKey = GetEnrollmentCacheKey(identityId, modality);
        string? previousEnrollmentId = null;

        if (_cache.TryGetValue(cacheKey, out EnrolledBiometricInfo? existing) && existing != null)
        {
            previousEnrollmentId = existing.EnrollmentId;
        }

        // Enroll with new hash (includes re-enrollment metadata)
        var metadata = new BiometricMetadata
        {
            IsReEnrollment = true,
            PreviousEnrollmentId = previousEnrollmentId,
            CapturedAt = DateTime.UtcNow
        };

        return await EnrollBiometricAsync(identityId, modality, newTemplateHash, metadata, cancellationToken);
    }

    private static string GetEnrollmentCacheKey(Guid identityId, BiometricModality modality)
    {
        return $"biometric_enrollment:{identityId}:{modality}";
    }
}

/// <summary>
/// Configuration options for biometric blockchain service.
/// </summary>
public class BiometricBlockchainOptions
{
    /// <summary>Whether the service is enabled.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Cache duration for enrollment data in minutes.</summary>
    public int CacheDurationMinutes { get; set; } = 15;

    /// <summary>Whether to enable tamper detection.</summary>
    public bool TamperDetectionEnabled { get; set; } = true;

    /// <summary>Supported biometric modalities.</summary>
    public string[] SupportedModalities { get; set; } = new[] { "Face", "Fingerprint", "Voice" };

    /// <summary>Minimum quality score for enrollment.</summary>
    public double MinQualityScore { get; set; } = 0.7;
}

/// <summary>
/// Request to create a biometric enrollment on blockchain.
/// </summary>
internal class BiometricEnrollmentRequest
{
    public string IdentityId { get; set; } = string.Empty;
    public string IdentityType { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Request to update a biometric enrollment on blockchain.
/// </summary>
internal class BiometricUpdateRequest
{
    public string IdentityId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}
