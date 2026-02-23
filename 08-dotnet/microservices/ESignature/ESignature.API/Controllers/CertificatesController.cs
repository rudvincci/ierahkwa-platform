using ESignature.Core.Interfaces;
using ESignature.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESignature.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ISignatureService _signatureService;

    public CertificatesController(ISignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    [HttpPost]
    public async Task<ActionResult<Certificate>> CreateCertificate([FromBody] Certificate certificate)
    {
        var created = await _signatureService.CreateCertificateAsync(certificate);
        return CreatedAtAction(nameof(GetCertificate), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Certificate>> GetCertificate(Guid id)
    {
        var cert = await _signatureService.GetCertificateByIdAsync(id);
        if (cert == null) return NotFound();
        return cert;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Certificate>> GetCertificateByUser(Guid userId)
    {
        var cert = await _signatureService.GetCertificateByUserAsync(userId);
        if (cert == null) return NotFound();
        return cert;
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Certificate>>> GetActiveCertificates()
    {
        var certs = await _signatureService.GetActiveCertificatesAsync();
        return Ok(certs);
    }

    [HttpPost("{id}/revoke")]
    public async Task<ActionResult<Certificate>> RevokeCertificate(Guid id, [FromBody] RevokeRequest request)
    {
        var cert = await _signatureService.RevokeCertificateAsync(id, request.Reason);
        return cert;
    }

    [HttpPost("{id}/renew")]
    public async Task<ActionResult<Certificate>> RenewCertificate(Guid id)
    {
        var cert = await _signatureService.RenewCertificateAsync(id);
        return cert;
    }

    [HttpGet("{id}/validate")]
    public async Task<ActionResult<bool>> ValidateCertificate(Guid id)
    {
        var isValid = await _signatureService.ValidateCertificateAsync(id);
        return Ok(new { IsValid = isValid });
    }
}

public class RevokeRequest
{
    public string Reason { get; set; } = string.Empty;
}
