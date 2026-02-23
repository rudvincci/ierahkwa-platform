using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/casino")]
public class CasinoController : ControllerBase
{
    private readonly ICasinoService _casino;
    private readonly ILogger<CasinoController> _logger;

    public CasinoController(ICasinoService casino, ILogger<CasinoController> logger)
    {
        _casino = casino;
        _logger = logger;
    }

    // ============= USER =============

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _casino.GetUserAsync(id);
        return Ok(new { success = true, data = user });
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _casino.CreateUserAsync(request.Username);
        return Ok(new { success = true, data = user });
    }

    [HttpGet("user/{id}/balance")]
    public async Task<IActionResult> GetBalance(string id)
    {
        var balance = await _casino.GetBalanceAsync(id);
        return Ok(new { success = true, data = new { balance } });
    }

    [HttpPost("user/{id}/deposit")]
    public async Task<IActionResult> Deposit(string id, [FromBody] AmountRequest request)
    {
        var result = await _casino.DepositAsync(id, request.Amount);
        var balance = await _casino.GetBalanceAsync(id);
        return Ok(new { success = result, data = new { balance } });
    }

    [HttpPost("user/{id}/withdraw")]
    public async Task<IActionResult> Withdraw(string id, [FromBody] AmountRequest request)
    {
        var result = await _casino.WithdrawAsync(id, request.Amount);
        var balance = await _casino.GetBalanceAsync(id);
        return Ok(new { success = result, data = new { balance } });
    }

    // ============= SLOTS =============

    [HttpPost("slots/spin")]
    public async Task<IActionResult> SpinSlots([FromBody] Models.SlotSpinRequest request)
    {
        try
        {
            var result = await _casino.SpinSlotsAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= ROULETTE =============

    [HttpPost("roulette/spin")]
    public async Task<IActionResult> SpinRoulette([FromBody] Models.RouletteBetRequest request)
    {
        try
        {
            var result = await _casino.SpinRouletteAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= BLACKJACK =============

    [HttpPost("blackjack/start")]
    public async Task<IActionResult> StartBlackjack([FromBody] Models.BlackjackRequest request)
    {
        try
        {
            var game = await _casino.StartBlackjackAsync(request);
            return Ok(new { success = true, data = game });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("blackjack/action")]
    public async Task<IActionResult> BlackjackAction([FromBody] Models.BlackjackAction action)
    {
        try
        {
            var game = await _casino.BlackjackActionAsync(action);
            return Ok(new { success = true, data = game });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= CRASH =============

    [HttpGet("crash/current")]
    public async Task<IActionResult> GetCurrentCrashGame()
    {
        var game = await _casino.GetCurrentCrashGameAsync();
        return Ok(new { success = true, data = game });
    }

    [HttpPost("crash/bet")]
    public async Task<IActionResult> PlaceCrashBet([FromBody] Models.CrashBetRequest request)
    {
        try
        {
            var bet = await _casino.PlaceCrashBetAsync(request);
            return Ok(new { success = true, data = bet });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("crash/cashout")]
    public async Task<IActionResult> CrashCashout([FromBody] CrashCashoutRequest request)
    {
        var bet = await _casino.CashoutCrashAsync(request.GameId, request.UserId);
        return bet != null 
            ? Ok(new { success = true, data = bet })
            : BadRequest(new { success = false, error = "Could not cashout" });
    }

    // ============= DICE =============

    [HttpPost("dice/roll")]
    public async Task<IActionResult> RollDice([FromBody] Models.DiceRollRequest request)
    {
        try
        {
            var result = await _casino.RollDiceAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= LOTTERY =============

    [HttpGet("lottery/current")]
    public async Task<IActionResult> GetCurrentLottery()
    {
        var lottery = await _casino.GetCurrentLotteryAsync();
        return Ok(new { success = true, data = lottery });
    }

    [HttpPost("lottery/buy")]
    public async Task<IActionResult> BuyLotteryTicket([FromBody] BuyLotteryRequest request)
    {
        try
        {
            var ticket = await _casino.BuyLotteryTicketAsync(request.UserId, request.Numbers);
            return Ok(new { success = true, data = ticket });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= SPORTS =============

    [HttpGet("sports/events")]
    public async Task<IActionResult> GetSportEvents([FromQuery] string? sport)
    {
        var events = await _casino.GetSportEventsAsync(sport);
        return Ok(new { success = true, data = events });
    }

    [HttpPost("sports/bet")]
    public async Task<IActionResult> PlaceSportBet([FromBody] Models.SportBetRequest request)
    {
        try
        {
            var bet = await _casino.PlaceSportBetAsync(request);
            return Ok(new { success = true, data = bet });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    // ============= STATS =============

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard([FromQuery] string period = "daily")
    {
        var leaderboard = await _casino.GetLeaderboardAsync(period);
        return Ok(new { success = true, data = leaderboard });
    }

    [HttpGet("user/{id}/transactions")]
    public async Task<IActionResult> GetTransactions(string id, [FromQuery] int limit = 20)
    {
        var transactions = await _casino.GetTransactionsAsync(id, limit);
        return Ok(new { success = true, data = transactions });
    }

    [HttpGet("promotions")]
    public async Task<IActionResult> GetPromotions()
    {
        var promotions = await _casino.GetPromotionsAsync();
        return Ok(new { success = true, data = promotions });
    }
}

// Request DTOs
public class CreateUserRequest
{
    public string Username { get; set; } = "";
}

public class AmountRequest
{
    public decimal Amount { get; set; }
}

public class CrashCashoutRequest
{
    public string GameId { get; set; } = "";
    public string UserId { get; set; } = "";
}

public class BuyLotteryRequest
{
    public string UserId { get; set; } = "";
    public int[]? Numbers { get; set; }
}
