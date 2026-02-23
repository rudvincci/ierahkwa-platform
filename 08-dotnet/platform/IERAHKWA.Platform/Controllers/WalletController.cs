using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// Crear nuevo wallet
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request)
    {
        try
        {
            var wallet = await _walletService.CreateWalletAsync(request);
            return Ok(new { success = true, data = wallet });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wallet");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener wallet por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWallet(string id)
    {
        var wallet = await _walletService.GetWalletAsync(id);
        if (wallet == null)
            return NotFound(new { success = false, error = "Wallet not found" });
        
        return Ok(new { success = true, data = wallet });
    }

    /// <summary>
    /// Obtener balance del wallet
    /// </summary>
    [HttpGet("{id}/balance")]
    public async Task<IActionResult> GetBalance(string id)
    {
        var balance = await _walletService.GetBalanceAsync(id);
        return Ok(new { success = true, data = balance });
    }

    /// <summary>
    /// Listar todos los wallets del usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserWallets(string userId)
    {
        var wallets = await _walletService.GetUserWalletsAsync(userId);
        return Ok(new { success = true, data = wallets });
    }

    /// <summary>
    /// Depositar fondos
    /// </summary>
    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(string id, [FromBody] DepositRequest request)
    {
        try
        {
            var result = await _walletService.DepositAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing deposit");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Retirar fondos
    /// </summary>
    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> Withdraw(string id, [FromBody] WithdrawRequest request)
    {
        try
        {
            var result = await _walletService.WithdrawAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (InsufficientFundsException)
        {
            return BadRequest(new { success = false, error = "Insufficient funds" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing withdrawal");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener tokens del wallet
    /// </summary>
    [HttpGet("{id}/tokens")]
    public async Task<IActionResult> GetTokens(string id)
    {
        var tokens = await _walletService.GetTokensAsync(id);
        return Ok(new { success = true, data = tokens });
    }

    /// <summary>
    /// Agregar token al wallet
    /// </summary>
    [HttpPost("{id}/tokens/add")]
    public async Task<IActionResult> AddToken(string id, [FromBody] AddTokenRequest request)
    {
        try
        {
            var result = await _walletService.AddTokenAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Exportar wallet (backup)
    /// </summary>
    [HttpPost("{id}/export")]
    public async Task<IActionResult> ExportWallet(string id, [FromBody] ExportRequest request)
    {
        try
        {
            var encrypted = await _walletService.ExportWalletAsync(id, request.Password);
            return Ok(new { success = true, data = new { encrypted } });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Importar wallet
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> ImportWallet([FromBody] ImportWalletRequest request)
    {
        try
        {
            var wallet = await _walletService.ImportWalletAsync(request);
            return Ok(new { success = true, data = wallet });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}
