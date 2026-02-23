using Mamey.FWID.Identities.BlazorWasm.Services;
using ContractsBiometricType = Mamey.FWID.Identities.Contracts.BiometricType;

namespace Mamey.FWID.Identities.BlazorWasm.Models;

// ==================== Face Capture Models ====================

public class FaceCaptureResult
{
    public string ImageData { get; set; } = string.Empty;
    public DateTime CapturedAt { get; set; }
    public bool LivenessVerified { get; set; }
    public double LivenessConfidence { get; set; }
    public FaceQualityResult? Quality { get; set; }
}

// ==================== Fingerprint Capture Models ====================

public enum FingerprintMode
{
    Register,
    Verify
}

public class FingerprintCaptureResult
{
    public bool Success { get; set; }
    public FingerprintMode Mode { get; set; }
    public WebAuthnCredential? Credential { get; set; }
    public WebAuthnAssertion? Assertion { get; set; }
    public DateTime CapturedAt { get; set; }
}

// ==================== Voice Capture Models ====================

public class VoiceCaptureResult
{
    public bool Success { get; set; }
    public string AudioData { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public VoiceQualityResult? Quality { get; set; }
    public DateTime CapturedAt { get; set; }
}

// ==================== Biometric Progress Models ====================

/// <summary>
/// Biometric step types for UI workflow (extends contract BiometricType with UI-specific values).
/// </summary>
public enum BiometricStepType
{
    Face,
    Fingerprint,
    Voice,
    Iris
}

public class BiometricStep
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public BiometricStepType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; } = true;
    public BiometricStepStatus Status { get; set; } = BiometricStepStatus.Pending;
    public string? Data { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Converts to the contracts BiometricType for API calls.
    /// </summary>
    public ContractsBiometricType ToContractType() => Type switch
    {
        BiometricStepType.Face => ContractsBiometricType.FaceRecognition,
        BiometricStepType.Fingerprint => ContractsBiometricType.Fingerprint,
        BiometricStepType.Voice => ContractsBiometricType.VoiceRecognition,
        BiometricStepType.Iris => ContractsBiometricType.IrisRecognition,
        _ => ContractsBiometricType.Fingerprint
    };
}

public enum BiometricStepStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Skipped
}
