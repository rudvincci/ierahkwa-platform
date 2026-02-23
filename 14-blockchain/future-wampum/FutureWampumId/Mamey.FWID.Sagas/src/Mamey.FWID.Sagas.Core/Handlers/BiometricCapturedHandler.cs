using Mamey.CQRS.Commands;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Sagas.Core.Handlers;

/// <summary>
/// Handles biometric submission during identity registration.
/// </summary>
public class BiometricCapturedHandler : ICommandHandler<SubmitBiometrics>
{
    private readonly ILogger<BiometricCapturedHandler> _logger;
    private readonly IBusPublisher _publisher;
    
    public BiometricCapturedHandler(
        ILogger<BiometricCapturedHandler> logger,
        IBusPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }
    
    public async Task HandleAsync(SubmitBiometrics command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing biometric submission for identity {IdentityId}", command.IdentityId);
        
        // Validate biometric quality
        var hasValidFingerprint = command.FingerprintTemplate?.Length > 0;
        var hasValidFace = command.FaceTemplate?.Length > 0;
        
        if (!hasValidFingerprint && !hasValidFace)
        {
            _logger.LogWarning("No valid biometric data provided for identity {IdentityId}", command.IdentityId);
            
            // Publish biometric capture failed event
            await _publisher.PublishAsync(new BiometricCaptureFailed
            {
                IdentityId = command.IdentityId,
                Reason = "No valid biometric data provided"
            });
            return;
        }
        
        if (command.QualityScore < 0.7)
        {
            _logger.LogWarning("Biometric quality score too low ({Score}) for identity {IdentityId}",
                command.QualityScore, command.IdentityId);
            
            await _publisher.PublishAsync(new BiometricCaptureFailed
            {
                IdentityId = command.IdentityId,
                Reason = $"Quality score {command.QualityScore:F2} below threshold 0.70"
            });
            return;
        }
        
        // Generate biometric template ID
        var templateId = Guid.NewGuid();
        
        _logger.LogInformation("Biometrics captured successfully for identity {IdentityId}. TemplateId: {TemplateId}, Score: {Score}",
            command.IdentityId, templateId, command.QualityScore);
        
        // Publish biometric captured event (saga will react)
        await _publisher.PublishAsync(new BiometricsCaptured
        {
            IdentityId = command.IdentityId,
            BiometricTemplateId = templateId,
            HasFingerprint = hasValidFingerprint,
            HasFaceRecognition = hasValidFace,
            QualityScore = command.QualityScore,
            CapturedAt = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Event published when biometrics are successfully captured.
/// </summary>
public record BiometricsCaptured
{
    public Guid IdentityId { get; init; }
    public Guid BiometricTemplateId { get; init; }
    public bool HasFingerprint { get; init; }
    public bool HasFaceRecognition { get; init; }
    public double QualityScore { get; init; }
    public DateTime CapturedAt { get; init; }
}

/// <summary>
/// Event published when biometric capture fails.
/// </summary>
public record BiometricCaptureFailed
{
    public Guid IdentityId { get; init; }
    public string Reason { get; init; } = null!;
}
