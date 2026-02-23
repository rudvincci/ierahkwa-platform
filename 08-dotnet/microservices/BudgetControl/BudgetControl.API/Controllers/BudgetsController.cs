using BudgetControl.Core.Interfaces;
using BudgetControl.Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace BudgetControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _service;
    public BudgetsController(IBudgetService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Budget>> Create([FromBody] Budget budget) => await _service.CreateBudgetAsync(budget);
    [HttpGet("{id}")] public async Task<ActionResult<Budget>> GetById(Guid id) { var b = await _service.GetBudgetByIdAsync(id); return b == null ? NotFound() : b; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Budget>>> GetAll([FromQuery] int? fiscalYear, [FromQuery] string? department, [FromQuery] BudgetStatus? status) => Ok(await _service.GetBudgetsAsync(fiscalYear, department, status));
    [HttpPut("{id}")] public async Task<ActionResult<Budget>> Update(Guid id, [FromBody] Budget budget) => await _service.UpdateBudgetAsync(budget);
    [HttpPost("{id}/approve")] public async Task<ActionResult<Budget>> Approve(Guid id, [FromQuery] Guid approvedBy) => await _service.ApproveBudgetAsync(id, approvedBy);
    [HttpPost("{id}/freeze")] public async Task<ActionResult<Budget>> Freeze(Guid id) => await _service.FreezeBudgetAsync(id);
    [HttpPost("{id}/lines")] public async Task<ActionResult<BudgetLine>> AddLine(Guid id, [FromBody] BudgetLine line) { line.BudgetId = id; return await _service.AddLineAsync(line); }
    [HttpGet("{id}/lines")] public async Task<ActionResult<IEnumerable<BudgetLine>>> GetLines(Guid id) => Ok(await _service.GetLinesAsync(id));
    [HttpPost("{id}/transactions")] public async Task<ActionResult<BudgetTransaction>> RecordTransaction(Guid id, [FromBody] BudgetTransaction txn) { txn.BudgetId = id; return await _service.RecordTransactionAsync(txn); }
    [HttpGet("{id}/transactions")] public async Task<ActionResult<IEnumerable<BudgetTransaction>>> GetTransactions(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to) => Ok(await _service.GetTransactionsAsync(id, from, to));
    [HttpGet("{id}/availability")] public async Task<ActionResult<bool>> CheckAvailability(Guid id, [FromQuery] decimal amount) => await _service.CheckAvailabilityAsync(id, amount);
    [HttpGet("statistics")] public async Task<ActionResult<BudgetStatistics>> GetStatistics([FromQuery] int? fiscalYear, [FromQuery] string? department) => await _service.GetStatisticsAsync(fiscalYear, department);
}

[ApiController]
[Route("api/budget-transfers")]
public class TransfersController : ControllerBase
{
    private readonly IBudgetService _service;
    public TransfersController(IBudgetService service) => _service = service;
    [HttpPost] public async Task<ActionResult<BudgetTransfer>> Request([FromBody] BudgetTransfer transfer) => await _service.RequestTransferAsync(transfer);
    [HttpGet] public async Task<ActionResult<IEnumerable<BudgetTransfer>>> GetAll([FromQuery] TransferStatus? status) => Ok(await _service.GetTransfersAsync(status));
    [HttpPost("{id}/approve")] public async Task<ActionResult<BudgetTransfer>> Approve(Guid id, [FromQuery] Guid approvedBy) => await _service.ApproveTransferAsync(id, approvedBy);
}
