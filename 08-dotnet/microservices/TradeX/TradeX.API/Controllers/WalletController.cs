using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.API.Controllers;

/// <summary>
/// Wallet API - Ierahkwa TradeX
/// Nivel VIP. Historial, VIP, depósito, retiro y transferencia están en /api/transactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _wallet;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService wallet, ILogger<WalletController> logger)
    {
        _wallet = wallet;
        _logger = logger;
    }

    /// <summary>Asignar nivel VIP a un usuario (Admin).</summary>
    [HttpPost("vip-level")]
    public async Task<ActionResult> SetVipLevel([FromBody] SetVipLevelRequest req)
    {
        await _wallet.SetUserVipLevelAsync(req.UserId, req.Level);
        _logger.LogInformation("VIP level set: User={UserId}, Level={Level}", req.UserId, req.Level);
        return Ok(new { userId = req.UserId, vipLevel = req.Level.ToString() });
    }
}

public record SetVipLevelRequest(Guid UserId, VipLevel Level);
