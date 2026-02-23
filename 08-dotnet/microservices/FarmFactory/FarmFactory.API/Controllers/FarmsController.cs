using FarmFactory.Core.Interfaces;
using FarmFactory.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmFactory.API.Controllers;

/// <summary>
/// Farms (deposits) - IERAHKWA FarmFactory
/// Deposit/Withdraw staking tokens, claim reward tokens. Rewards by (amount × time) share.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FarmsController : ControllerBase
{
    private readonly IFarmFactoryService _farm;
    private readonly ILogger<FarmsController> _log;

    public FarmsController(IFarmFactoryService farm, ILogger<FarmsController> log)
    {
        _farm = farm;
        _log = log;
    }

    /// <summary>Deposit (stake) staking tokens into a pool.</summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<FarmDeposit>> Deposit([FromBody] DepositRequest req)
    {
        try
        {
            var d = await _farm.DepositAsync(req.PoolId, req.UserWallet, req.Amount);
            _log.LogInformation("IERAHKWA FarmFactory: Deposit {Id} {Amount} {Wallet} in pool {PoolId}", d.Id, d.Amount, d.UserWallet, d.PoolId);
            return CreatedAtAction(nameof(GetDeposit), new { id = d.Id }, d);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Withdraw staked tokens from a deposit.</summary>
    [HttpPost("withdraw")]
    public async Task<ActionResult<FarmDeposit>> Withdraw([FromBody] WithdrawRequest req)
    {
        try
        {
            var d = await _farm.WithdrawAsync(req.DepositId, req.UserWallet);
            return Ok(d);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Claim reward tokens. Pass depositId, or (userWallet + poolId) to claim all in that pool.</summary>
    [HttpPost("claim")]
    public async Task<ActionResult<ClaimResult>> Claim([FromBody] ClaimRequest req)
    {
        try
        {
            var amount = await _farm.ClaimAsync(req.DepositId, req.UserWallet, req.PoolId);
            _log.LogInformation("IERAHKWA FarmFactory: Claimed {Amount} for {Wallet}", amount, req.UserWallet);
            return Ok(new ClaimResult(amount));
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>List deposits. Optional: userWallet, poolId.</summary>
    [HttpGet("deposits")]
    public async Task<ActionResult<IEnumerable<FarmDeposit>>> GetDeposits([FromQuery] string? wallet = null, [FromQuery] Guid? poolId = null)
    {
        var list = await _farm.GetDepositsAsync(wallet, poolId);
        return Ok(list);
    }

    /// <summary>Get deposit by id.</summary>
    [HttpGet("deposits/{id:guid}")]
    public async Task<ActionResult<FarmDeposit>> GetDeposit(Guid id)
    {
        var d = await _farm.GetDepositAsync(id);
        if (d == null) return NotFound();
        return Ok(d);
    }

    /// <summary>Pending reward for a deposit.</summary>
    [HttpGet("deposits/{id:guid}/pending-reward")]
    public async Task<ActionResult<decimal>> GetDepositPendingReward(Guid id)
    {
        var v = await _farm.GetPendingRewardAsync(id);
        return Ok(new { depositId = id, pendingReward = v });
    }

    /// <summary>Pending reward for wallet in pool. Query: wallet, poolId.</summary>
    [HttpGet("pending-reward")]
    public async Task<ActionResult<decimal>> GetPendingReward([FromQuery] string wallet, [FromQuery] Guid poolId)
    {
        if (string.IsNullOrWhiteSpace(wallet)) return BadRequest("wallet required");
        var v = await _farm.GetPendingRewardForWalletInPoolAsync(wallet, poolId);
        return Ok(new { wallet, poolId, pendingReward = v });
    }
}

public record DepositRequest(Guid PoolId, string UserWallet, decimal Amount);
public record WithdrawRequest(Guid DepositId, string UserWallet);
public record ClaimRequest(string UserWallet, Guid? DepositId = null, Guid? PoolId = null);
public record ClaimResult(decimal ClaimedAmount);
