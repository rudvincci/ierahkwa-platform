using ESignature.Core.Interfaces;
using ESignature.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESignature.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SigningController : ControllerBase
{
    private readonly ISignatureService _signatureService;

    public SigningController(ISignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    [HttpGet("token/{token}")]
    public async Task<ActionResult<Signer>> GetSignerByToken(string token)
    {
        var signer = await _signatureService.GetSignerByTokenAsync(token);
        if (signer == null) return NotFound(new { message = "Invalid or expired token" });
        return signer;
    }

    [HttpPost("{signerId}/view")]
    public async Task<ActionResult<Signer>> MarkAsViewed(Guid signerId)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers.UserAgent.ToString();
        var signer = await _signatureService.MarkAsViewedAsync(signerId, ipAddress, userAgent);
        return signer;
    }

    [HttpPost("{signerId}/sign")]
    public async Task<ActionResult<Signer>> SignDocument(Guid signerId, [FromBody] SignDocumentRequest request)
    {
        request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        request.UserAgent = Request.Headers.UserAgent.ToString();
        
        var signer = await _signatureService.SignDocumentAsync(signerId, request);
        return signer;
    }

    [HttpPost("{signerId}/decline")]
    public async Task<ActionResult<Signer>> DeclineSignature(Guid signerId, [FromBody] DeclineRequest request)
    {
        var signer = await _signatureService.DeclineSignatureAsync(signerId, request.Reason);
        return signer;
    }

    [HttpPost("{signerId}/delegate")]
    public async Task<ActionResult<Signer>> DelegateSignature(Guid signerId, [FromBody] DelegateRequest request)
    {
        var signer = await _signatureService.DelegateSignatureAsync(signerId, request.Email, request.Name);
        return signer;
    }

    [HttpPost("{signerId}/reminder")]
    public async Task<ActionResult> SendReminder(Guid signerId)
    {
        await _signatureService.SendReminderAsync(signerId);
        return Ok();
    }

    [HttpGet("{signerId}/url")]
    public async Task<ActionResult<string>> GetSigningUrl(Guid signerId)
    {
        var url = await _signatureService.GenerateSigningUrlAsync(signerId);
        return Ok(new { SigningUrl = url });
    }

    [HttpPost("{signerId}/verify")]
    public async Task<ActionResult<SignatureVerification>> VerifySignature(Guid signerId)
    {
        var verification = await _signatureService.VerifySignatureAsync(signerId);
        return verification;
    }
}

public class DeclineRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class DelegateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
