using Microsoft.AspNetCore.Mvc;
using ProcurementHub.Core.Interfaces;
using ProcurementHub.Core.Models;

namespace ProcurementHub.API.Controllers;

[ApiController]
[Route("api/requisitions")]
public class RequisitionsController : ControllerBase
{
    private readonly IProcurementService _service;
    public RequisitionsController(IProcurementService service) => _service = service;

    [HttpPost] public async Task<ActionResult<PurchaseRequisition>> Create([FromBody] PurchaseRequisition req) => await _service.CreateRequisitionAsync(req);
    [HttpGet("{id}")] public async Task<ActionResult<PurchaseRequisition>> GetById(Guid id) { var r = await _service.GetRequisitionByIdAsync(id); return r == null ? NotFound() : r; }
    [HttpGet] public async Task<ActionResult<IEnumerable<PurchaseRequisition>>> GetAll([FromQuery] RequisitionStatus? status, [FromQuery] string? department) => Ok(await _service.GetRequisitionsAsync(status, department));
    [HttpPut("{id}")] public async Task<ActionResult<PurchaseRequisition>> Update(Guid id, [FromBody] PurchaseRequisition req) => await _service.UpdateRequisitionAsync(req);
    [HttpPost("{id}/approve")] public async Task<ActionResult<PurchaseRequisition>> Approve(Guid id, [FromQuery] Guid approvedBy) => await _service.ApproveRequisitionAsync(id, approvedBy);
    [HttpPost("{id}/reject")] public async Task<ActionResult<PurchaseRequisition>> Reject(Guid id, [FromBody] string reason) => await _service.RejectRequisitionAsync(id, reason);
    [HttpPost("{id}/convert")] public async Task<ActionResult<PurchaseOrder>> Convert(Guid id, [FromQuery] Guid vendorId, [FromQuery] Guid createdBy) => await _service.ConvertRequisitionToOrderAsync(id, vendorId, createdBy);
}

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IProcurementService _service;
    public PurchaseOrdersController(IProcurementService service) => _service = service;

    [HttpPost] public async Task<ActionResult<PurchaseOrder>> Create([FromBody] PurchaseOrder order) => await _service.CreatePurchaseOrderAsync(order);
    [HttpGet("{id}")] public async Task<ActionResult<PurchaseOrder>> GetById(Guid id) { var o = await _service.GetPurchaseOrderByIdAsync(id); return o == null ? NotFound() : o; }
    [HttpGet] public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAll([FromQuery] PurchaseOrderStatus? status, [FromQuery] string? department, [FromQuery] Guid? vendorId) => Ok(await _service.GetPurchaseOrdersAsync(status, department, vendorId));
    [HttpPut("{id}")] public async Task<ActionResult<PurchaseOrder>> Update(Guid id, [FromBody] PurchaseOrder order) => await _service.UpdatePurchaseOrderAsync(order);
    [HttpPost("{id}/approve")] public async Task<ActionResult<PurchaseOrder>> Approve(Guid id, [FromQuery] Guid approvedBy) => await _service.ApprovePurchaseOrderAsync(id, approvedBy);
}

[ApiController]
[Route("api/tenders")]
public class TendersController : ControllerBase
{
    private readonly IProcurementService _service;
    public TendersController(IProcurementService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Tender>> Create([FromBody] Tender tender) => await _service.CreateTenderAsync(tender);
    [HttpGet("{id}")] public async Task<ActionResult<Tender>> GetById(Guid id) { var t = await _service.GetTenderByIdAsync(id); return t == null ? NotFound() : t; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Tender>>> GetAll([FromQuery] TenderStatus? status) => Ok(await _service.GetTendersAsync(status));
    [HttpPost("{id}/publish")] public async Task<ActionResult<Tender>> Publish(Guid id) => await _service.PublishTenderAsync(id);
    [HttpPost("{id}/bids")] public async Task<ActionResult<TenderBid>> SubmitBid(Guid id, [FromBody] TenderBid bid) { bid.TenderId = id; return await _service.SubmitBidAsync(bid); }
    [HttpPost("{id}/award")] public async Task<ActionResult<Tender>> Award(Guid id, [FromQuery] Guid vendorId, [FromQuery] decimal amount) => await _service.AwardTenderAsync(id, vendorId, amount);
}

[ApiController]
[Route("api/procurement")]
public class ProcurementStatsController : ControllerBase
{
    private readonly IProcurementService _service;
    public ProcurementStatsController(IProcurementService service) => _service = service;
    [HttpGet("statistics")] public async Task<ActionResult<ProcurementStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}
