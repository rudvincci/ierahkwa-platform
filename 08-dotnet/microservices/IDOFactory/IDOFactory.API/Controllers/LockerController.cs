using Microsoft.AspNetCore.Mvc;
using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LockerController : ControllerBase
{
    private readonly ITokenLockerService _lockerService;
    private readonly ILogger<LockerController> _logger;

    public LockerController(ITokenLockerService lockerService, ILogger<LockerController> logger)
    {
        _lockerService = lockerService;
        _logger = logger;
    }

    /// <summary>
    /// Get all token locks
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TokenLocker>>> GetAllLockers()
    {
        var lockers = await _lockerService.GetAllLockersAsync();
        return Ok(lockers);
    }

    /// <summary>
    /// Get locks by owner address
    /// </summary>
    [HttpGet("owner/{ownerAddress}")]
    public async Task<ActionResult<IEnumerable<TokenLocker>>> GetLockersByOwner(string ownerAddress)
    {
        var lockers = await _lockerService.GetLockersByOwnerAsync(ownerAddress);
        return Ok(lockers);
    }

    /// <summary>
    /// Get locks by token address
    /// </summary>
    [HttpGet("token/{tokenAddress}")]
    public async Task<ActionResult<IEnumerable<TokenLocker>>> GetLockersByToken(string tokenAddress)
    {
        var lockers = await _lockerService.GetLockersByTokenAsync(tokenAddress);
        return Ok(lockers);
    }

    /// <summary>
    /// Get a specific locker by ID
    /// </summary>
    [HttpGet("{lockerId}")]
    public async Task<ActionResult<TokenLocker>> GetLocker(string lockerId)
    {
        var locker = await _lockerService.GetLockerByIdAsync(lockerId);
        if (locker == null) return NotFound();
        return Ok(locker);
    }

    /// <summary>
    /// Create a new token lock
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TokenLocker>> CreateLock([FromBody] CreateLockRequest request)
    {
        try
        {
            var locker = new TokenLocker
            {
                Name = request.Name,
                Description = request.Description,
                TokenAddress = request.TokenAddress,
                TokenSymbol = request.TokenSymbol,
                TokenName = request.TokenName,
                TokenDecimals = request.TokenDecimals,
                LogoUrl = request.LogoUrl,
                Amount = request.Amount,
                LockDate = DateTime.UtcNow,
                UnlockDate = request.UnlockDate,
                IsLinearVesting = request.IsLinearVesting,
                VestingPeriodDays = request.VestingPeriodDays,
                LockType = request.LockType,
                OwnerAddress = request.OwnerAddress,
                BeneficiaryAddress = request.BeneficiaryAddress ?? request.OwnerAddress,
                ChainId = request.ChainId,
                Network = request.Network,
                RelatedPoolId = request.RelatedPoolId
            };

            var created = await _lockerService.CreateLockAsync(locker);
            return CreatedAtAction(nameof(GetLocker), new { lockerId = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lock");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Unlock tokens
    /// </summary>
    [HttpPost("{lockerId}/unlock")]
    public async Task<ActionResult<TokenLocker>> UnlockTokens(string lockerId, [FromBody] UnlockRequest request)
    {
        try
        {
            var locker = await _lockerService.UnlockTokensAsync(lockerId, request.OwnerAddress, request.Amount);
            return Ok(locker);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Transfer lock ownership
    /// </summary>
    [HttpPost("{lockerId}/transfer")]
    public async Task<ActionResult<TokenLocker>> TransferOwnership(string lockerId, [FromBody] TransferRequest request)
    {
        try
        {
            var locker = await _lockerService.TransferOwnershipAsync(lockerId, request.CurrentOwner, request.NewOwner);
            return Ok(locker);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Emergency withdraw (with penalty)
    /// </summary>
    [HttpPost("{lockerId}/emergency-withdraw")]
    public async Task<ActionResult<TokenLocker>> EmergencyWithdraw(string lockerId, [FromBody] EmergencyRequest request)
    {
        try
        {
            var locker = await _lockerService.EmergencyWithdrawAsync(lockerId, request.OwnerAddress);
            return Ok(locker);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get unlockable amount
    /// </summary>
    [HttpGet("{lockerId}/unlockable")]
    public async Task<ActionResult<decimal>> GetUnlockableAmount(string lockerId)
    {
        try
        {
            var amount = await _lockerService.GetUnlockableAmountAsync(lockerId);
            return Ok(new { unlockableAmount = amount });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get next unlock date
    /// </summary>
    [HttpGet("{lockerId}/next-unlock")]
    public async Task<ActionResult<DateTime>> GetNextUnlockDate(string lockerId)
    {
        try
        {
            var date = await _lockerService.GetNextUnlockDateAsync(lockerId);
            return Ok(new { nextUnlockDate = date });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get locker statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<TokenLockerStatistics>> GetStatistics()
    {
        var stats = await _lockerService.GetStatisticsAsync();
        return Ok(stats);
    }
}

// Request DTOs
public record CreateLockRequest(
    string Name,
    string Description,
    string TokenAddress,
    string TokenSymbol,
    string TokenName,
    int TokenDecimals,
    string LogoUrl,
    decimal Amount,
    DateTime UnlockDate,
    bool IsLinearVesting,
    int VestingPeriodDays,
    TokenLockType LockType,
    string OwnerAddress,
    string? BeneficiaryAddress,
    int ChainId,
    string Network,
    string? RelatedPoolId
);

public record UnlockRequest(string OwnerAddress, decimal Amount);
public record TransferRequest(string CurrentOwner, string NewOwner);
public record EmergencyRequest(string OwnerAddress);
