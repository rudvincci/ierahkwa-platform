using ESignature.Core.Interfaces;
using ESignature.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESignature.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SignatureRequestsController : ControllerBase
{
    private readonly ISignatureService _signatureService;

    public SignatureRequestsController(ISignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    [HttpPost]
    public async Task<ActionResult<SignatureRequest>> CreateRequest([FromBody] SignatureRequest request)
    {
        var created = await _signatureService.CreateRequestAsync(request);
        return CreatedAtAction(nameof(GetRequest), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SignatureRequest>> GetRequest(Guid id)
    {
        var request = await _signatureService.GetRequestByIdAsync(id);
        if (request == null) return NotFound();
        return request;
    }

    [HttpGet("number/{requestNumber}")]
    public async Task<ActionResult<SignatureRequest>> GetRequestByNumber(string requestNumber)
    {
        var request = await _signatureService.GetRequestByNumberAsync(requestNumber);
        if (request == null) return NotFound();
        return request;
    }

    [HttpGet("sender/{senderId}")]
    public async Task<ActionResult<IEnumerable<SignatureRequest>>> GetRequestsBySender(Guid senderId)
    {
        var requests = await _signatureService.GetRequestsBySenderAsync(senderId);
        return Ok(requests);
    }

    [HttpGet("pending/{userId}")]
    public async Task<ActionResult<IEnumerable<SignatureRequest>>> GetPendingRequests(Guid userId)
    {
        var requests = await _signatureService.GetPendingRequestsAsync(userId);
        return Ok(requests);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SignatureRequest>> UpdateRequest(Guid id, [FromBody] SignatureRequest request)
    {
        if (id != request.Id) return BadRequest();
        var updated = await _signatureService.UpdateRequestAsync(request);
        return updated;
    }

    [HttpPost("{id}/send")]
    public async Task<ActionResult<SignatureRequest>> SendRequest(Guid id)
    {
        var request = await _signatureService.SendRequestAsync(id);
        return request;
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<SignatureRequest>> CancelRequest(Guid id, [FromBody] CancelRequest cancelRequest)
    {
        var request = await _signatureService.CancelRequestAsync(id, cancelRequest.Reason);
        return request;
    }

    [HttpPost("{id}/void")]
    public async Task<ActionResult<SignatureRequest>> VoidRequest(Guid id, [FromBody] CancelRequest voidRequest)
    {
        var request = await _signatureService.VoidRequestAsync(id, voidRequest.Reason);
        return request;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRequest(Guid id)
    {
        await _signatureService.DeleteRequestAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/signers")]
    public async Task<ActionResult<Signer>> AddSigner(Guid id, [FromBody] Signer signer)
    {
        var added = await _signatureService.AddSignerAsync(id, signer);
        return CreatedAtAction(nameof(GetRequest), new { id }, added);
    }

    [HttpPost("{id}/reminders")]
    public async Task<ActionResult> SendReminders(Guid id)
    {
        await _signatureService.SendBulkRemindersAsync(id);
        return Ok();
    }

    [HttpGet("{id}/fields")]
    public async Task<ActionResult<IEnumerable<SignatureField>>> GetFields(Guid id)
    {
        var fields = await _signatureService.GetFieldsAsync(id);
        return Ok(fields);
    }

    [HttpPost("{id}/fields")]
    public async Task<ActionResult<SignatureField>> AddField(Guid id, [FromBody] SignatureField field)
    {
        field.SignatureRequestId = id;
        var added = await _signatureService.AddFieldAsync(field);
        return CreatedAtAction(nameof(GetFields), new { id }, added);
    }

    [HttpGet("{id}/audit-logs")]
    public async Task<ActionResult<IEnumerable<SignatureAuditLog>>> GetAuditLogs(Guid id)
    {
        var logs = await _signatureService.GetAuditLogsAsync(id);
        return Ok(logs);
    }

    [HttpGet("{id}/audit-trail/pdf")]
    public async Task<ActionResult> GetAuditTrailPdf(Guid id)
    {
        var pdf = await _signatureService.GenerateAuditTrailPdfAsync(id);
        return File(pdf, "application/pdf", $"audit-trail-{id}.pdf");
    }

    [HttpPost("{id}/verify")]
    public async Task<ActionResult<SignatureVerification>> VerifyDocument(Guid id)
    {
        var verification = await _signatureService.VerifyDocumentAsync(id);
        return verification;
    }

    [HttpPost("{id}/blockchain/register")]
    public async Task<ActionResult<string>> RegisterOnBlockchain(Guid id)
    {
        var txId = await _signatureService.RegisterOnBlockchainAsync(id);
        return Ok(new { TransactionId = txId });
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> DownloadSignedDocument(Guid id)
    {
        var document = await _signatureService.GetSignedDocumentAsync(id);
        return File(document, "application/pdf", $"signed-document-{id}.pdf");
    }

    [HttpGet("{id}/certificate")]
    public async Task<ActionResult> GetCertificateOfCompletion(Guid id)
    {
        var cert = await _signatureService.GenerateCertificateOfCompletionAsync(id);
        return File(cert, "application/pdf", $"certificate-{id}.pdf");
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<SignatureStatistics>> GetStatistics([FromQuery] Guid? userId)
    {
        var stats = await _signatureService.GetStatisticsAsync(userId);
        return stats;
    }
}

public class CancelRequest
{
    public string Reason { get; set; } = string.Empty;
}
