using ContractManager.Core.Interfaces;
using ContractManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _service;
    public ContractsController(IContractService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Contract>> Create([FromBody] Contract contract) => CreatedAtAction(nameof(GetById), new { id = (await _service.CreateContractAsync(contract)).Id }, contract);
    [HttpGet("{id}")] public async Task<ActionResult<Contract>> GetById(Guid id) { var c = await _service.GetContractByIdAsync(id); return c == null ? NotFound() : c; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Contract>>> GetAll([FromQuery] ContractStatus? status, [FromQuery] string? department, [FromQuery] Guid? vendorId) => Ok(await _service.GetContractsAsync(status, department, vendorId));
    [HttpGet("expiring")] public async Task<ActionResult<IEnumerable<Contract>>> GetExpiring([FromQuery] int days = 30) => Ok(await _service.GetExpiringContractsAsync(days));
    [HttpPut("{id}")] public async Task<ActionResult<Contract>> Update(Guid id, [FromBody] Contract contract) => await _service.UpdateContractAsync(contract);
    [HttpPost("{id}/approve")] public async Task<ActionResult<Contract>> Approve(Guid id, [FromQuery] Guid approvedBy) => await _service.ApproveContractAsync(id, approvedBy);
    [HttpPost("{id}/terminate")] public async Task<ActionResult<Contract>> Terminate(Guid id, [FromBody] string reason) => await _service.TerminateContractAsync(id, reason);
    [HttpPost("{id}/renew")] public async Task<ActionResult<Contract>> Renew(Guid id, [FromBody] DateTime newEndDate) => await _service.RenewContractAsync(id, newEndDate);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteContractAsync(id); return NoContent(); }
    [HttpPost("{id}/milestones")] public async Task<ActionResult<ContractMilestone>> AddMilestone(Guid id, [FromBody] ContractMilestone milestone) { milestone.ContractId = id; return await _service.AddMilestoneAsync(milestone); }
    [HttpGet("{id}/milestones")] public async Task<ActionResult<IEnumerable<ContractMilestone>>> GetMilestones(Guid id) => Ok(await _service.GetMilestonesAsync(id));
    [HttpPost("{id}/payments")] public async Task<ActionResult<ContractPayment>> AddPayment(Guid id, [FromBody] ContractPayment payment) { payment.ContractId = id; return await _service.AddPaymentAsync(payment); }
    [HttpGet("{id}/payments")] public async Task<ActionResult<IEnumerable<ContractPayment>>> GetPayments(Guid id) => Ok(await _service.GetPaymentsAsync(id));
    [HttpGet("payments/overdue")] public async Task<ActionResult<IEnumerable<ContractPayment>>> GetOverduePayments() => Ok(await _service.GetOverduePaymentsAsync());
    [HttpPost("{id}/amendments")] public async Task<ActionResult<ContractAmendment>> AddAmendment(Guid id, [FromBody] ContractAmendment amendment) { amendment.ContractId = id; return await _service.AddAmendmentAsync(amendment); }
    [HttpGet("{id}/amendments")] public async Task<ActionResult<IEnumerable<ContractAmendment>>> GetAmendments(Guid id) => Ok(await _service.GetAmendmentsAsync(id));
    [HttpGet("statistics")] public async Task<ActionResult<ContractStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IContractService _service;
    public VendorsController(IContractService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Vendor>> Create([FromBody] Vendor vendor) => await _service.CreateVendorAsync(vendor);
    [HttpGet("{id}")] public async Task<ActionResult<Vendor>> GetById(Guid id) { var v = await _service.GetVendorByIdAsync(id); return v == null ? NotFound() : v; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Vendor>>> GetAll([FromQuery] VendorStatus? status) => Ok(await _service.GetVendorsAsync(status));
    [HttpPut("{id}")] public async Task<ActionResult<Vendor>> Update(Guid id, [FromBody] Vendor vendor) => await _service.UpdateVendorAsync(vendor);
    [HttpGet("{id}/contracts")] public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(Guid id) => Ok(await _service.GetVendorContractsAsync(id));
}
