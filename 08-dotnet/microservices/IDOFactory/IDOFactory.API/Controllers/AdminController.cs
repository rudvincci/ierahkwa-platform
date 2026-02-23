using Microsoft.AspNetCore.Mvc;
using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IPlatformConfigService _configService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IPlatformConfigService configService, ILogger<AdminController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Get platform configuration (public fields)
    /// </summary>
    [HttpGet("config")]
    public async Task<ActionResult<PlatformConfig>> GetConfig()
    {
        var config = await _configService.GetConfigAsync();
        return Ok(config);
    }

    /// <summary>
    /// Update platform configuration (admin only)
    /// </summary>
    [HttpPut("config")]
    public async Task<ActionResult<PlatformConfig>> UpdateConfig([FromBody] UpdateConfigRequest request)
    {
        try
        {
            var updated = await _configService.UpdateConfigAsync(request.Config, request.AdminAddress);
            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating config");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Check if address is admin
    /// </summary>
    [HttpGet("is-admin/{address}")]
    public async Task<ActionResult<bool>> IsAdmin(string address)
    {
        var isAdmin = await _configService.IsAdminAsync(address);
        return Ok(new { isAdmin });
    }

    /// <summary>
    /// Add a new admin
    /// </summary>
    [HttpPost("admins")]
    public async Task<ActionResult> AddAdmin([FromBody] AdminRequest request)
    {
        try
        {
            await _configService.AddAdminAsync(request.Address, request.CurrentAdmin);
            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove an admin
    /// </summary>
    [HttpDelete("admins/{address}")]
    public async Task<ActionResult> RemoveAdmin(string address, [FromQuery] string currentAdmin)
    {
        try
        {
            await _configService.RemoveAdminAsync(address, currentAdmin);
            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get supported networks
    /// </summary>
    [HttpGet("networks")]
    public async Task<ActionResult<IEnumerable<SupportedNetwork>>> GetNetworks()
    {
        var networks = await _configService.GetSupportedNetworksAsync();
        return Ok(networks);
    }

    /// <summary>
    /// Add a network (admin only)
    /// </summary>
    [HttpPost("networks")]
    public async Task<ActionResult> AddNetwork([FromBody] AddNetworkRequest request)
    {
        try
        {
            await _configService.AddNetworkAsync(request.Network, request.AdminAddress);
            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove a network (admin only)
    /// </summary>
    [HttpDelete("networks/{chainId}")]
    public async Task<ActionResult> RemoveNetwork(int chainId, [FromQuery] string adminAddress)
    {
        try
        {
            await _configService.RemoveNetworkAsync(chainId, adminAddress);
            return Ok(new { success = true });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get branding configuration (public - for frontend)
    /// </summary>
    [HttpGet("branding")]
    public async Task<ActionResult> GetBranding()
    {
        var config = await _configService.GetConfigAsync();
        return Ok(new
        {
            config.PlatformName,
            config.LogoUrl,
            config.FaviconUrl,
            config.Title,
            config.Description,
            config.PrimaryColor,
            config.SecondaryColor,
            config.AccentColor,
            config.BackgroundColor,
            config.CardColor,
            config.TextColor,
            config.Website,
            config.Twitter,
            config.Telegram,
            config.Discord,
            config.Medium,
            config.Email
        });
    }

    /// <summary>
    /// Get fee configuration
    /// </summary>
    [HttpGet("fees")]
    public async Task<ActionResult> GetFees()
    {
        var config = await _configService.GetConfigAsync();
        return Ok(new
        {
            config.PoolCreationFee,
            config.PlatformFeePercentage,
            config.TokenLockFee,
            config.EmergencyWithdrawFee,
            config.FeeReceiverAddress
        });
    }
}

// Request DTOs
public record UpdateConfigRequest(PlatformConfig Config, string AdminAddress);
public record AdminRequest(string Address, string CurrentAdmin);
public record AddNetworkRequest(SupportedNetwork Network, string AdminAddress);
