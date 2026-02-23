using Microsoft.AspNetCore.Mvc;
using BioMetrics.Core.Interfaces;
using BioMetrics.Core.Models;

namespace BioMetrics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BiometricsController : ControllerBase
{
    private readonly IBiometricService _biometricService;

    public BiometricsController(IBiometricService biometricService)
    {
        _biometricService = biometricService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<BiometricProfile>> GetProfile(string userId)
    {
        var profile = await _biometricService.GetProfileAsync(userId);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPost("enroll")]
    public async Task<ActionResult<BiometricEnrollment>> Enroll([FromBody] EnrollmentRequest request)
    {
        var enrollment = await _biometricService.EnrollAsync(request.UserId, request.Type, request.Data);
        return Ok(enrollment);
    }

    [HttpPost("verify")]
    public async Task<ActionResult<VerificationResult>> Verify([FromBody] VerificationRequest request)
    {
        var result = await _biometricService.VerifyAsync(request.UserId, request.Type, request.Data);
        return Ok(result);
    }

    [HttpGet("devices/{userId}")]
    public async Task<ActionResult<IEnumerable<BiometricDevice>>> GetDevices(string userId)
    {
        var devices = await _biometricService.GetDevicesAsync(userId);
        return Ok(devices);
    }

    [HttpPost("devices")]
    public async Task<ActionResult<BiometricDevice>> RegisterDevice([FromBody] DeviceRegistration request)
    {
        var device = await _biometricService.RegisterDeviceAsync(request);
        return Ok(device);
    }

    [HttpGet("activity/{userId}")]
    public async Task<ActionResult<IEnumerable<AuthenticationLog>>> GetActivity(string userId)
    {
        var activity = await _biometricService.GetAuthenticationLogsAsync(userId);
        return Ok(activity);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<BiometricStats>> GetStats()
    {
        var stats = await _biometricService.GetStatsAsync();
        return Ok(stats);
    }
}

public class EnrollmentRequest
{
    public string UserId { get; set; } = string.Empty;
    public BiometricType Type { get; set; }
    public string Data { get; set; } = string.Empty;
}

public class VerificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public BiometricType Type { get; set; }
    public string Data { get; set; } = string.Empty;
}
