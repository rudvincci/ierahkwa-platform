using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.API.Controllers.Transactions;

/// <summary>
/// API de Transacciones - Ierahkwa TradeX
/// Historial, VIP, estado on-chain, depósitos, retiros y transferencias entre usuarios.
/// Carpeta: Controllers/Transactions — todas las rutas bajo /api/transactions
/// </summary>
[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IWalletService _wallet;
    private readonly IIerahkwaNodeService _node;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        IWalletService wallet,
        IIerahkwaNodeService node,
        ILogger<TransactionsController> logger)
    {
        _wallet = wallet;
        _node = node;
        _logger = logger;
    }

    // ─── Historial y VIP ─────────────────────────────────────────────────────

    /// <summary>Historial de transacciones de un usuario. vipOnly para solo VIP.</summary>
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetHistory(
        [FromQuery] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool vipOnly = false)
    {
        var list = await _wallet.GetTransactionHistoryAsync(userId, page, pageSize, vipOnly);
        return Ok(list);
    }

    /// <summary>Transacciones VIP (prioridad). Opcional: userId.</summary>
    [HttpGet("vip")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetVip(
        [FromQuery] Guid? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var list = await _wallet.GetVipTransactionsAsync(userId, page, pageSize);
        return Ok(list);
    }

    /// <summary>Estado de una transacción on-chain por txHash (Ierahkwa Node).</summary>
    [HttpGet("status")]
    public async Task<ActionResult> GetStatus([FromQuery] string txHash)
    {
        var status = await _node.GetTransactionStatusAsync(txHash);
        return Ok(new { txHash, status });
    }

    /// <summary>Rutas registradas de la carpeta Transacciones.</summary>
    [HttpGet]
    [HttpGet("index")]
    public ActionResult Index() => Ok(new
    {
        folder = "Controllers/Transactions",
        baseRoute = TransactionsRoutes.Base,
        endpoints = TransactionsRoutes.All
    });

    // ─── Depósito, Retiro, Transferencia ─────────────────────────────────────

    /// <summary>Registrar depósito (userId, asset, monto, txHash on-chain).</summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<Transaction>> Deposit([FromBody] DepositRequest req)
    {
        try
        {
            var t = await _wallet.DepositAsync(req.UserId, req.AssetId, req.Amount, req.TxHash);
            _logger.LogInformation("Deposit: User={UserId}, Amount={Amount}, TxHash={TxHash}", req.UserId, req.Amount, req.TxHash);
            return Ok(t);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Retiro a una dirección. Aplica fee con descuento VIP si aplica.</summary>
    [HttpPost("withdraw")]
    public async Task<ActionResult<Transaction>> Withdraw([FromBody] WithdrawRequest req)
    {
        try
        {
            var t = await _wallet.WithdrawAsync(req.UserId, req.AssetId, req.Amount, req.ToAddress);
            _logger.LogInformation("Withdraw: User={UserId}, Amount={Amount}, To={To}", req.UserId, req.Amount, req.ToAddress);
            return Ok(t);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Transferencia entre usuarios (por userId). Descuento VIP al emisor.</summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<Transaction>> Transfer([FromBody] TransferUserRequest req)
    {
        try
        {
            var t = await _wallet.TransferAsync(req.FromUserId, req.ToUserId, req.AssetId, req.Amount);
            _logger.LogInformation("Transfer: {From} -> {To}, Amount={Amount}", req.FromUserId, req.ToUserId, req.Amount);
            return Ok(t);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

// ─── DTOs (registro de la carpeta Transacciones) ────────────────────────────

public record DepositRequest(Guid UserId, Guid AssetId, decimal Amount, string TxHash);
public record WithdrawRequest(Guid UserId, Guid AssetId, decimal Amount, string ToAddress);
public record TransferUserRequest(Guid FromUserId, Guid ToUserId, Guid AssetId, decimal Amount);
