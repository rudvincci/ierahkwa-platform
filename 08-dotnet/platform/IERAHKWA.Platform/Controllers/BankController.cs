using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankController : ControllerBase
{
    private readonly IBankService _bankService;
    private readonly ILogger<BankController> _logger;

    public BankController(IBankService bankService, ILogger<BankController> logger)
    {
        _bankService = bankService;
        _logger = logger;
    }

    /// <summary>
    /// Crear cuenta bancaria
    /// </summary>
    [HttpPost("account/create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var account = await _bankService.CreateAccountAsync(request);
            return Ok(new { success = true, data = account });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener cuenta por ID
    /// </summary>
    [HttpGet("account/{id}")]
    public async Task<IActionResult> GetAccount(string id)
    {
        var account = await _bankService.GetAccountAsync(id);
        if (account == null)
            return NotFound(new { success = false, error = "Account not found" });
        
        return Ok(new { success = true, data = account });
    }

    /// <summary>
    /// Listar cuentas del usuario
    /// </summary>
    [HttpGet("accounts/user/{userId}")]
    public async Task<IActionResult> GetUserAccounts(string userId)
    {
        var accounts = await _bankService.GetUserAccountsAsync(userId);
        return Ok(new { success = true, data = accounts });
    }

    /// <summary>
    /// Transferencia bancaria interna
    /// </summary>
    [HttpPost("transfer/internal")]
    public async Task<IActionResult> InternalTransfer([FromBody] InternalTransferRequest request)
    {
        try
        {
            var result = await _bankService.InternalTransferAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (InsufficientFundsException)
        {
            return BadRequest(new { success = false, error = "Insufficient funds" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Transferencia SWIFT internacional
    /// </summary>
    [HttpPost("transfer/swift")]
    public async Task<IActionResult> SwiftTransfer([FromBody] SwiftTransferRequest request)
    {
        try
        {
            var result = await _bankService.SwiftTransferAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SWIFT transfer error");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Información del banco
    /// </summary>
    [HttpGet("info")]
    public async Task<IActionResult> GetBankInfo()
    {
        var info = await _bankService.GetBankInfoAsync();
        return Ok(new { success = true, data = info });
    }

    /// <summary>
    /// Tasas de cambio
    /// </summary>
    [HttpGet("exchange-rates")]
    public async Task<IActionResult> GetExchangeRates()
    {
        var rates = await _bankService.GetExchangeRatesAsync();
        return Ok(new { success = true, data = rates });
    }

    /// <summary>
    /// Convertir moneda
    /// </summary>
    [HttpPost("exchange")]
    public async Task<IActionResult> Exchange([FromBody] ExchangeRequest request)
    {
        try
        {
            var result = await _bankService.ExchangeAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Historial de movimientos
    /// </summary>
    [HttpGet("account/{id}/movements")]
    public async Task<IActionResult> GetMovements(string id, [FromQuery] int page = 1, [FromQuery] int limit = 20)
    {
        var movements = await _bankService.GetMovementsAsync(id, page, limit);
        return Ok(new { success = true, data = movements });
    }

    /// <summary>
    /// Generar estado de cuenta
    /// </summary>
    [HttpGet("account/{id}/statement")]
    public async Task<IActionResult> GetStatement(string id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var statement = await _bankService.GenerateStatementAsync(id, from, to);
        return Ok(new { success = true, data = statement });
    }
}
