using Microsoft.AspNetCore.Mvc;
using OutlookExtractor.Infrastructure.Services;

namespace OutlookExtractor.API.Controllers;

/// <summary>
/// Authentication Controller
/// Handles Microsoft 365 / Office 365 authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly MicrosoftGraphService _graphService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        MicrosoftGraphService graphService,
        ILogger<AuthenticationController> logger)
    {
        _graphService = graphService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate using Client Credentials (Service Principal)
    /// </summary>
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting authentication with tenant: {TenantId}", request.TenantId);
            
            var success = await _graphService.AuthenticateAsync(
                request.TenantId,
                request.ClientId,
                request.ClientSecret
            );

            if (success)
            {
                var userEmail = await _graphService.GetUserEmailAsync();
                var displayName = await _graphService.GetUserDisplayNameAsync();

                return Ok(new
                {
                    success = true,
                    message = "Authentication successful",
                    userEmail = userEmail,
                    displayName = displayName
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Authentication failed. Please check your credentials."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return StatusCode(500, new
            {
                success = false,
                message = "Authentication error: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Authenticate using Interactive Browser (User Login)
    /// </summary>
    [HttpPost("authenticate-interactive")]
    public async Task<IActionResult> AuthenticateInteractive([FromBody] InteractiveAuthRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting interactive authentication");
            
            var success = await _graphService.AuthenticateInteractiveAsync(request.ClientId);

            if (success)
            {
                var userEmail = await _graphService.GetUserEmailAsync();
                var displayName = await _graphService.GetUserDisplayNameAsync();

                return Ok(new
                {
                    success = true,
                    message = "Authentication successful",
                    userEmail = userEmail,
                    displayName = displayName
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Interactive authentication failed."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during interactive authentication");
            return StatusCode(500, new
            {
                success = false,
                message = "Authentication error: " + ex.Message
            });
        }
    }

    /// <summary>
    /// Check authentication status
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var isAuthenticated = await _graphService.IsAuthenticatedAsync();

            if (isAuthenticated)
            {
                var userEmail = await _graphService.GetUserEmailAsync();
                var displayName = await _graphService.GetUserDisplayNameAsync();

                return Ok(new
                {
                    authenticated = true,
                    userEmail = userEmail,
                    displayName = displayName
                });
            }

            return Ok(new
            {
                authenticated = false,
                message = "Not authenticated. Please authenticate first."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return StatusCode(500, new
            {
                authenticated = false,
                message = ex.Message
            });
        }
    }
}

/// <summary>
/// Authentication request model
/// </summary>
public class AuthRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
/// Interactive authentication request model
/// </summary>
public class InteractiveAuthRequest
{
    public string ClientId { get; set; } = string.Empty;
}
