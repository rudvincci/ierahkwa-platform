using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Crear nueva transacción
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequest request)
    {
        try
        {
            var result = await _transactionService.CreateTransactionAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener transacción por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(string id)
    {
        var transaction = await _transactionService.GetTransactionAsync(id);
        if (transaction == null)
            return NotFound(new { success = false, error = "Transaction not found" });
        
        return Ok(new { success = true, data = transaction });
    }

    /// <summary>
    /// Listar transacciones de un wallet
    /// </summary>
    [HttpGet("wallet/{walletId}")]
    public async Task<IActionResult> GetWalletTransactions(string walletId, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var transactions = await _transactionService.GetWalletTransactionsAsync(walletId, page, limit);
        return Ok(new { success = true, data = transactions });
    }

    /// <summary>
    /// Transferencia entre wallets
    /// </summary>
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            var result = await _transactionService.TransferAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (InsufficientFundsException)
        {
            return BadRequest(new { success = false, error = "Insufficient funds" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transfer");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener historial de transacciones
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit = 50)
    {
        var history = await _transactionService.GetHistoryAsync(from, to, limit);
        return Ok(new { success = true, data = history });
    }

    /// <summary>
    /// Estadísticas de transacciones
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _transactionService.GetStatsAsync();
        return Ok(new { success = true, data = stats });
    }

    /// <summary>
    /// Verificar estado de transacción
    /// </summary>
    [HttpGet("status/{txHash}")]
    public async Task<IActionResult> GetStatus(string txHash)
    {
        var status = await _transactionService.GetStatusAsync(txHash);
        return Ok(new { success = true, data = status });
    }

    /// <summary>
    /// Cancelar transacción pendiente
    /// </summary>
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelTransaction(string id)
    {
        try
        {
            var result = await _transactionService.CancelAsync(id);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}
