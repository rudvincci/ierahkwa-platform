using Microsoft.AspNetCore.Mvc;
using Mamey.Government.Identity.Models;
using Mamey.Government.Identity.Services;

namespace Mamey.Government.Identity.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;
    
    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterIdentityRequest request)
    {
        var identity = await _identityService.RegisterAsync(request);
        return Ok(new
        {
            success = true,
            identity = new
            {
                identity.Id,
                identity.FutureWampumId,
                identity.CitizenId,
                identity.Email,
                identity.ReferralCode,
                identity.VerificationLevel
            }
        });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var identity = await _identityService.GetByIdAsync(id);
        if (identity == null)
            return NotFound(new { error = "Identity not found" });
        
        return Ok(new { identity });
    }
    
    [HttpGet("fwid/{fwid}")]
    public async Task<IActionResult> GetByFwId(string fwid)
    {
        var identity = await _identityService.GetByFwIdAsync(fwid);
        if (identity == null)
            return NotFound(new { error = "Identity not found" });
        
        return Ok(new { identity });
    }
    
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var identity = await _identityService.GetByEmailAsync(email);
        if (identity == null)
            return NotFound(new { error = "Identity not found" });
        
        return Ok(new { identity });
    }
    
    [HttpGet("wallet/{address}")]
    public async Task<IActionResult> GetByWallet(string address)
    {
        var identity = await _identityService.GetByWalletAsync(address);
        if (identity == null)
            return NotFound(new { error = "Identity not found" });
        
        return Ok(new { identity });
    }
    
    [HttpPost("{id:guid}/verify")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerificationRequest request)
    {
        var result = await _identityService.VerifyIdentityAsync(id, request);
        if (!result)
            return BadRequest(new { error = "Verification failed" });
        
        return Ok(new { success = true, message = "Identity verified" });
    }
    
    [HttpPost("{id:guid}/link-wallet")]
    public async Task<IActionResult> LinkWallet(Guid id, [FromBody] LinkWalletRequest request)
    {
        var result = await _identityService.LinkWalletAsync(id, request.WalletAddress, request.PublicKey);
        if (!result)
            return BadRequest(new { error = "Failed to link wallet" });
        
        return Ok(new { success = true, message = "Wallet linked" });
    }
    
    [HttpPost("{id:guid}/membership")]
    public async Task<IActionResult> SetMembership(Guid id, [FromBody] SetMembershipRequest request)
    {
        var result = await _identityService.SetMembershipAsync(id, request.Tier);
        if (!result)
            return BadRequest(new { error = "Failed to set membership" });
        
        return Ok(new { success = true, tier = request.Tier });
    }
    
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        var token = await _identityService.AuthenticateAsync(request);
        return Ok(token);
    }
    
    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        var isValid = await _identityService.ValidateTokenAsync(request.Token);
        return Ok(new { valid = isValid });
    }
    
    [HttpGet("generate-fwid")]
    public async Task<IActionResult> GenerateFwId()
    {
        var fwid = await _identityService.GenerateFutureWampumIdAsync();
        return Ok(new { futureWampumId = fwid });
    }
}

// Request DTOs
public record LinkWalletRequest(string WalletAddress, string PublicKey);
public record SetMembershipRequest(MembershipTier Tier);
public record ValidateTokenRequest(string Token);
