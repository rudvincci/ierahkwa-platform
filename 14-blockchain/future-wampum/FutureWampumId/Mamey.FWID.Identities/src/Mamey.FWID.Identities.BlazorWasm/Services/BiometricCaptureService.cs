using Microsoft.JSInterop;

namespace Mamey.FWID.Identities.BlazorWasm.Services;

/// <summary>
/// Service for capturing and managing biometric data in the browser.
/// Provides face, fingerprint (WebAuthn), and voice capture capabilities.
/// </summary>
public interface IBiometricCaptureService
{
    /// <summary>Gets available media devices.</summary>
    Task<MediaDevicesInfo> GetAvailableDevicesAsync();
    
    /// <summary>Checks permission status for camera or microphone.</summary>
    Task<string> CheckPermissionAsync(string type);
    
    /// <summary>Initializes the camera for face capture.</summary>
    Task<bool> InitCameraAsync(string videoElementId, CameraOptions? options = null);
    
    /// <summary>Stops the active camera stream.</summary>
    Task StopCameraAsync();
    
    /// <summary>Captures a frame from the video element.</summary>
    Task<string> CaptureFrameAsync(string videoElementId, string canvasElementId);
    
    /// <summary>Assesses the quality of a captured face image.</summary>
    Task<FaceQualityResult> AssessFaceQualityAsync(string canvasElementId);
    
    /// <summary>Detects face position in the frame.</summary>
    Task<FacePositionResult> DetectFacePositionAsync(string canvasElementId);
    
    /// <summary>Checks if WebAuthn (fingerprint) is available.</summary>
    Task<bool> IsWebAuthnAvailableAsync();
    
    /// <summary>Registers a new fingerprint credential.</summary>
    Task<WebAuthnCredential> RegisterFingerprintAsync(WebAuthnRegistrationOptions options);
    
    /// <summary>Verifies fingerprint using stored credential.</summary>
    Task<WebAuthnAssertion> VerifyFingerprintAsync(WebAuthnVerificationOptions options);
    
    /// <summary>Starts voice recording.</summary>
    Task<bool> StartVoiceRecordingAsync();
    
    /// <summary>Stops voice recording and returns audio data.</summary>
    Task<string> StopVoiceRecordingAsync();
    
    /// <summary>Gets current audio level (0-100).</summary>
    Task<int> GetAudioLevelAsync();
    
    /// <summary>Assesses voice recording quality.</summary>
    Task<VoiceQualityResult> AssessVoiceQualityAsync(string base64Audio);
    
    /// <summary>Performs a liveness detection action.</summary>
    Task<LivenessActionResult> DetectLivenessActionAsync(string action, string videoElementId, int timeout = 10000);
    
    /// <summary>Runs a full liveness check sequence.</summary>
    Task<LivenessCheckResult> RunLivenessCheckAsync(string videoElementId, string[] actions, Action<LivenessProgress>? onProgress = null);
    
    /// <summary>Releases all active media resources.</summary>
    Task ReleaseAllAsync();
}

/// <summary>
/// Implementation of the biometric capture service using JavaScript interop.
/// </summary>
public class BiometricCaptureService : IBiometricCaptureService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<BiometricCaptureService>? _dotNetRef;

    public BiometricCaptureService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<MediaDevicesInfo> GetAvailableDevicesAsync()
    {
        return await _jsRuntime.InvokeAsync<MediaDevicesInfo>("BiometricCapture.getAvailableDevices");
    }

    public async Task<string> CheckPermissionAsync(string type)
    {
        return await _jsRuntime.InvokeAsync<string>("BiometricCapture.checkPermission", type);
    }

    public async Task<bool> InitCameraAsync(string videoElementId, CameraOptions? options = null)
    {
        return await _jsRuntime.InvokeAsync<bool>("BiometricCapture.initCamera", videoElementId, options ?? new CameraOptions());
    }

    public async Task StopCameraAsync()
    {
        await _jsRuntime.InvokeVoidAsync("BiometricCapture.stopCamera");
    }

    public async Task<string> CaptureFrameAsync(string videoElementId, string canvasElementId)
    {
        return await _jsRuntime.InvokeAsync<string>("BiometricCapture.captureFrame", videoElementId, canvasElementId);
    }

    public async Task<FaceQualityResult> AssessFaceQualityAsync(string canvasElementId)
    {
        return await _jsRuntime.InvokeAsync<FaceQualityResult>("BiometricCapture.assessFaceQuality", canvasElementId);
    }

    public async Task<FacePositionResult> DetectFacePositionAsync(string canvasElementId)
    {
        return await _jsRuntime.InvokeAsync<FacePositionResult>("BiometricCapture.detectFacePosition", canvasElementId);
    }

    public async Task<bool> IsWebAuthnAvailableAsync()
    {
        return await _jsRuntime.InvokeAsync<bool>("BiometricCapture.isWebAuthnAvailable");
    }

    public async Task<WebAuthnCredential> RegisterFingerprintAsync(WebAuthnRegistrationOptions options)
    {
        return await _jsRuntime.InvokeAsync<WebAuthnCredential>("BiometricCapture.registerFingerprint", options);
    }

    public async Task<WebAuthnAssertion> VerifyFingerprintAsync(WebAuthnVerificationOptions options)
    {
        return await _jsRuntime.InvokeAsync<WebAuthnAssertion>("BiometricCapture.verifyFingerprint", options);
    }

    public async Task<bool> StartVoiceRecordingAsync()
    {
        return await _jsRuntime.InvokeAsync<bool>("BiometricCapture.startVoiceRecording");
    }

    public async Task<string> StopVoiceRecordingAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("BiometricCapture.stopVoiceRecording");
    }

    public async Task<int> GetAudioLevelAsync()
    {
        return await _jsRuntime.InvokeAsync<int>("BiometricCapture.getAudioLevel");
    }

    public async Task<VoiceQualityResult> AssessVoiceQualityAsync(string base64Audio)
    {
        return await _jsRuntime.InvokeAsync<VoiceQualityResult>("BiometricCapture.assessVoiceQuality", base64Audio);
    }

    public async Task<LivenessActionResult> DetectLivenessActionAsync(string action, string videoElementId, int timeout = 10000)
    {
        return await _jsRuntime.InvokeAsync<LivenessActionResult>("BiometricCapture.detectLivenessAction", action, videoElementId, timeout);
    }

    public async Task<LivenessCheckResult> RunLivenessCheckAsync(string videoElementId, string[] actions, Action<LivenessProgress>? onProgress = null)
    {
        if (onProgress != null)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            // Store callback for JS to invoke
            _progressCallback = onProgress;
        }

        return await _jsRuntime.InvokeAsync<LivenessCheckResult>("BiometricCapture.runLivenessCheck", videoElementId, actions, _dotNetRef);
    }

    private Action<LivenessProgress>? _progressCallback;

    [JSInvokable]
    public void OnLivenessProgress(LivenessProgress progress)
    {
        _progressCallback?.Invoke(progress);
    }

    public async Task ReleaseAllAsync()
    {
        await _jsRuntime.InvokeVoidAsync("BiometricCapture.releaseAll");
    }

    public async ValueTask DisposeAsync()
    {
        await ReleaseAllAsync();
        _dotNetRef?.Dispose();
    }
}

// DTOs for biometric capture

public class CameraOptions
{
    public string FacingMode { get; set; } = "user";
    public int Width { get; set; } = 640;
    public int Height { get; set; } = 480;
}

public class MediaDevicesInfo
{
    public bool HasCamera { get; set; }
    public bool HasMicrophone { get; set; }
    public List<DeviceInfo> Cameras { get; set; } = new();
    public List<DeviceInfo> Microphones { get; set; } = new();
    public string? Error { get; set; }
}

public class DeviceInfo
{
    public string DeviceId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class FaceQualityResult
{
    public bool IsValid { get; set; }
    public double Brightness { get; set; }
    public double Contrast { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class FacePositionResult
{
    public bool Detected { get; set; }
    public bool Centered { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public class WebAuthnRegistrationOptions
{
    public string Challenge { get; set; } = string.Empty;
    public string RpName { get; set; } = "FutureWampumID";
    public string? RpId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class WebAuthnCredential
{
    public string Id { get; set; } = string.Empty;
    public string RawId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public WebAuthnResponse Response { get; set; } = new();
}

public class WebAuthnResponse
{
    public string ClientDataJSON { get; set; } = string.Empty;
    public string? AttestationObject { get; set; }
    public string? AuthenticatorData { get; set; }
    public string? Signature { get; set; }
    public string? UserHandle { get; set; }
}

public class WebAuthnVerificationOptions
{
    public string Challenge { get; set; } = string.Empty;
    public string? RpId { get; set; }
    public List<AllowCredential>? AllowCredentials { get; set; }
}

public class AllowCredential
{
    public string Id { get; set; } = string.Empty;
}

public class WebAuthnAssertion
{
    public string Id { get; set; } = string.Empty;
    public string RawId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public WebAuthnResponse Response { get; set; } = new();
}

public class VoiceQualityResult
{
    public bool IsValid { get; set; }
    public double Duration { get; set; }
    public List<string> Issues { get; set; } = new();
}

public class LivenessActionResult
{
    public bool Detected { get; set; }
    public string Action { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public class LivenessCheckResult
{
    public bool Passed { get; set; }
    public double Confidence { get; set; }
    public List<LivenessActionResult> Results { get; set; } = new();
}

public class LivenessProgress
{
    public int Step { get; set; }
    public int Total { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
