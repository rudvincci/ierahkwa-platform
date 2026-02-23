using Microsoft.AspNetCore.Mvc;
using Mamey.SICB.ZeroKnowledgeProofs.Models;
using Mamey.SICB.ZeroKnowledgeProofs.Services;

namespace Mamey.SICB.ZeroKnowledgeProofs.Controllers;

[ApiController]
[Route("api/v1/zkp")]
public class ZKProofController : ControllerBase
{
    private readonly IZKProofService _zkService;
    
    public ZKProofController(IZKProofService zkService)
    {
        _zkService = zkService;
    }
    
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateProof([FromBody] ZKProofRequest request)
    {
        var proof = await _zkService.GenerateProofAsync(request);
        return Ok(new { success = true, proof });
    }
    
    [HttpPost("verify/{id:guid}")]
    public async Task<IActionResult> VerifyProof(Guid id, [FromBody] ZKVerificationRequest request)
    {
        request.ProofId = id;
        var isValid = await _zkService.VerifyProofAsync(id, request);
        return Ok(new { success = true, valid = isValid });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProof(Guid id)
    {
        var proof = await _zkService.GetProofAsync(id);
        if (proof == null)
            return NotFound(new { error = "Proof not found" });
        
        return Ok(new { proof });
    }
    
    [HttpPost("identity")]
    public async Task<IActionResult> GenerateIdentityProof([FromBody] IdentityProofRequest request)
    {
        var proof = await _zkService.GenerateIdentityProofAsync(
            request.FutureWampumId, 
            request.ProveAge, 
            request.AgeThreshold
        );
        return Ok(new { success = true, proof });
    }
    
    [HttpPost("balance")]
    public async Task<IActionResult> GenerateBalanceProof([FromBody] BalanceProofRequest request)
    {
        var proof = await _zkService.GenerateBalanceProofAsync(
            request.Address, 
            request.Token, 
            request.MinimumBalance
        );
        return Ok(new { success = true, proof });
    }
    
    [HttpPost("treaty-compliance")]
    public async Task<IActionResult> GenerateTreatyComplianceProof([FromBody] TreatyComplianceProofRequest request)
    {
        var proof = await _zkService.GenerateTreatyComplianceProofAsync(
            request.EntityId, 
            request.TreatyId
        );
        return Ok(new { success = true, proof });
    }
    
    [HttpPost("age")]
    public async Task<IActionResult> GenerateAgeProof([FromBody] AgeProofRequest request)
    {
        var proof = await _zkService.GenerateAgeProofAsync(
            request.DateOfBirth, 
            request.AgeThreshold
        );
        return Ok(new { success = true, proof });
    }
    
    [HttpPost("membership")]
    public async Task<IActionResult> GenerateMembershipProof([FromBody] MembershipProofRequest request)
    {
        var proof = await _zkService.GenerateMembershipProofAsync(
            request.FutureWampumId, 
            request.MinimumTier
        );
        return Ok(new { success = true, proof });
    }
}

// Request DTOs
public record IdentityProofRequest(string FutureWampumId, bool ProveAge = false, int? AgeThreshold = null);
public record BalanceProofRequest(string Address, string Token, string MinimumBalance);
public record TreatyComplianceProofRequest(string EntityId, string TreatyId);
public record AgeProofRequest(DateTime DateOfBirth, int AgeThreshold);
public record MembershipProofRequest(string FutureWampumId, int MinimumTier);
