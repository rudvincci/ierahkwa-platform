// ============================================================================
// IERAHKWA SOVEREIGN PLATFORM - BIOMETRICS API CONTROLLER
// REST endpoints for biometric authentication
// ============================================================================

using BioMetrics.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioMetrics.API;

[ApiController]
[Route("api/biometrics")]
[Authorize]
public class BiometricsController : ControllerBase
{
    private readonly IBiometricService _biometricService;

    public BiometricsController(IBiometricService biometricService)
    {
        _biometricService = biometricService;
    }

    /// <summary>
    /// Enroll biometric data for a user
    /// </summary>
    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var enrollmentRequest = new BiometricEnrollmentRequest
            {
                UserId = userId,
                Type = request.Type,
                BiometricData = Convert.FromBase64String(request.BiometricData),
                DeviceId = request.DeviceId
            };

            var template = await _biometricService.EnrollAsync(enrollmentRequest);

            return Ok(new
            {
                success = true,
                data = new
                {
                    templateId = template.Id,
                    type = template.Type.ToString(),
                    createdAt = template.CreatedAt
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Verify biometric data
    /// </summary>
    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
    {
        var verificationRequest = new BiometricVerificationRequest
        {
            UserId = request.UserId,
            Type = request.Type,
            BiometricData = Convert.FromBase64String(request.BiometricData),
            DeviceId = request.DeviceId,
            MinMatchScore = request.MinMatchScore ?? 0.85
        };

        var result = await _biometricService.VerifyAsync(verificationRequest);

        if (result.Success)
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    verified = true,
                    matchScore = result.MatchScore,
                    timestamp = result.Timestamp
                }
            });
        }

        return Ok(new
        {
            success = false,
            data = new
            {
                verified = false,
                matchScore = result.MatchScore,
                error = result.ErrorMessage
            }
        });
    }

    /// <summary>
    /// Get user's enrolled biometrics
    /// </summary>
    [HttpGet("enrolled")]
    public async Task<IActionResult> GetEnrolled()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var templates = await _biometricService.GetUserTemplatesAsync(userId);

        return Ok(new
        {
            success = true,
            data = templates.Select(t => new
            {
                type = t.Type.ToString(),
                isActive = t.IsActive,
                deviceId = t.DeviceId,
                createdAt = t.CreatedAt,
                lastUsedAt = t.LastUsedAt
            })
        });
    }

    /// <summary>
    /// Check if user is enrolled for specific biometric type
    /// </summary>
    [HttpGet("enrolled/{type}")]
    public async Task<IActionResult> CheckEnrollment(BiometricType type)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var isEnrolled = await _biometricService.IsEnrolledAsync(userId, type);

        return Ok(new
        {
            success = true,
            data = new { isEnrolled, type = type.ToString() }
        });
    }

    /// <summary>
    /// Delete biometric enrollment
    /// </summary>
    [HttpDelete("enrolled/{type}")]
    public async Task<IActionResult> DeleteEnrollment(BiometricType type)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _biometricService.DeleteTemplateAsync(userId, type);

        return Ok(new { success = true, message = $"Biometric {type} deleted successfully" });
    }

    /// <summary>
    /// Biometric login (verify + generate tokens)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> BiometricLogin([FromBody] BiometricLoginRequest request)
    {
        var verificationRequest = new BiometricVerificationRequest
        {
            UserId = request.UserId,
            Type = request.Type,
            BiometricData = Convert.FromBase64String(request.BiometricData),
            DeviceId = request.DeviceId,
            MinMatchScore = 0.90 // Higher threshold for login
        };

        var result = await _biometricService.VerifyAsync(verificationRequest);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                success = false,
                error = result.ErrorMessage ?? "Biometric verification failed"
            });
        }

        // In production, inject JwtService and generate tokens here
        // For now, return success with placeholder
        return Ok(new
        {
            success = true,
            data = new
            {
                verified = true,
                matchScore = result.MatchScore,
                // tokens would be generated here
                message = "Biometric login successful"
            }
        });
    }
}

// Request DTOs
public class EnrollRequest
{
    public BiometricType Type { get; set; }
    public string BiometricData { get; set; } = ""; // Base64 encoded
    public string DeviceId { get; set; } = "";
}

public class VerifyRequest
{
    public string UserId { get; set; } = "";
    public BiometricType Type { get; set; }
    public string BiometricData { get; set; } = ""; // Base64 encoded
    public string DeviceId { get; set; } = "";
    public double? MinMatchScore { get; set; }
}

public class BiometricLoginRequest
{
    public string UserId { get; set; } = "";
    public BiometricType Type { get; set; }
    public string BiometricData { get; set; } = ""; // Base64 encoded
    public string DeviceId { get; set; } = "";
}
