using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.Biometrics.Engine.Models;

namespace Mamey.Biometrics.Engine;

/// <summary>
/// Client interface for communicating with the Python biometric engine.
/// </summary>
public interface IBiometricEngineClient
{
    /// <summary>
    /// Extract face encoding from image data.
    /// </summary>
    /// <param name="request">Extract encoding request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Extract encoding response</returns>
    Task<ExtractEncodingResponse> ExtractEncodingAsync(ExtractEncodingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compare two face encodings.
    /// </summary>
    /// <param name="request">Compare encodings request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Compare encodings response</returns>
    Task<CompareEncodingsResponse> CompareEncodingsAsync(CompareEncodingsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detect faces in image data.
    /// </summary>
    /// <param name="request">Detect face request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detect face response</returns>
    Task<DetectFaceResponse> DetectFaceAsync(DetectFaceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the biometric engine is healthy.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get service information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Service information</returns>
    Task<ServiceInfoResponse> GetServiceInfoAsync(CancellationToken cancellationToken = default);
}
