using DigitalVault.Core.Interfaces;
using DigitalVault.Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace DigitalVault.API.Controllers;

[ApiController]
[Route("api/vault")]
public class VaultController : ControllerBase
{
    private readonly IVaultService _service;
    public VaultController(IVaultService service) => _service = service;

    [HttpPost("items")] public async Task<ActionResult<ArchiveItem>> Archive([FromForm] ArchiveItem item, IFormFile file) { using var stream = file.OpenReadStream(); item.FileName = file.FileName; item.FileSize = file.Length; item.ContentType = file.ContentType; return await _service.ArchiveItemAsync(item, stream); }
    [HttpGet("items/{id}")] public async Task<ActionResult<ArchiveItem>> GetItem(Guid id) { var i = await _service.GetItemByIdAsync(id); return i == null ? NotFound() : i; }
    [HttpGet("items")] public async Task<ActionResult<IEnumerable<ArchiveItem>>> Search([FromQuery] string? query, [FromQuery] ArchiveType? type, [FromQuery] ClassificationLevel? classification, [FromQuery] string? department) => Ok(await _service.SearchItemsAsync(query, type, classification, department));
    [HttpPut("items/{id}")] public async Task<ActionResult<ArchiveItem>> UpdateItem(Guid id, [FromBody] ArchiveItem item) => await _service.UpdateItemAsync(item);
    [HttpGet("items/{id}/download")] public async Task<ActionResult> Download(Guid id, [FromQuery] Guid userId) { var item = await _service.GetItemByIdAsync(id); if (item == null) return NotFound(); var stream = await _service.DownloadItemAsync(id, userId); return File(stream, item.ContentType, item.FileName); }
    [HttpPost("items/{id}/legal-hold")] public async Task<ActionResult<ArchiveItem>> SetLegalHold(Guid id, [FromQuery] bool hold, [FromQuery] string? reason) => await _service.SetLegalHoldAsync(id, hold, reason);
    [HttpDelete("items/{id}")] public async Task<ActionResult> DeleteItem(Guid id) { await _service.SoftDeleteItemAsync(id); return NoContent(); }
    [HttpPost("items/{id}/restore")] public async Task<ActionResult<ArchiveItem>> Restore(Guid id) => await _service.RestoreItemAsync(id);
    [HttpPost("items/{id}/blockchain")] public async Task<ActionResult<string>> RegisterBlockchain(Guid id) => Ok(await _service.RegisterOnBlockchainAsync(id));
    [HttpGet("items/{id}/verify")] public async Task<ActionResult<bool>> Verify(Guid id) => await _service.VerifyIntegrityAsync(id);
    [HttpGet("items/{id}/access-log")] public async Task<ActionResult<IEnumerable<ArchiveAccess>>> GetAccessLog(Guid id) => Ok(await _service.GetAccessLogAsync(id));

    [HttpPost("folders")] public async Task<ActionResult<ArchiveFolder>> CreateFolder([FromBody] ArchiveFolder folder) => await _service.CreateFolderAsync(folder);
    [HttpGet("folders")] public async Task<ActionResult<IEnumerable<ArchiveFolder>>> GetFolders([FromQuery] Guid? parentId, [FromQuery] string? department) => Ok(await _service.GetFoldersAsync(parentId, department));
    [HttpGet("folders/{id}/items")] public async Task<ActionResult<IEnumerable<ArchiveItem>>> GetFolderItems(Guid id) => Ok(await _service.GetFolderItemsAsync(id));

    [HttpPost("requests")] public async Task<ActionResult<ArchiveRequest>> CreateRequest([FromBody] ArchiveRequest request) => await _service.CreateRequestAsync(request);
    [HttpGet("requests")] public async Task<ActionResult<IEnumerable<ArchiveRequest>>> GetRequests([FromQuery] RequestStatus? status, [FromQuery] Guid? userId) => Ok(await _service.GetRequestsAsync(status, userId));
    [HttpPost("requests/{id}/process")] public async Task<ActionResult<ArchiveRequest>> ProcessRequest(Guid id, [FromQuery] bool approved, [FromQuery] Guid approvedBy, [FromQuery] string? notes) => await _service.ProcessRequestAsync(id, approved, approvedBy, notes);

    [HttpGet("statistics")] public async Task<ActionResult<VaultStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}

[ApiController]
[Route("api/retention-policies")]
public class RetentionPoliciesController : ControllerBase
{
    private readonly IVaultService _service;
    public RetentionPoliciesController(IVaultService service) => _service = service;
    [HttpPost] public async Task<ActionResult<RetentionPolicy>> Create([FromBody] RetentionPolicy policy) => await _service.CreatePolicyAsync(policy);
    [HttpGet] public async Task<ActionResult<IEnumerable<RetentionPolicy>>> GetAll() => Ok(await _service.GetPoliciesAsync());
    [HttpPut("{id}")] public async Task<ActionResult<RetentionPolicy>> Update(Guid id, [FromBody] RetentionPolicy policy) => await _service.UpdatePolicyAsync(policy);
}
