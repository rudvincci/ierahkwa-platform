using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasinoController : ControllerBase
    {
        private readonly ICasinoServices _casinoServices;

        public CasinoController(ICasinoServices casinoServices)
        {
            _casinoServices = casinoServices;
        }

        [HttpGet("games")]
        public async Task<ActionResult<List<CasinoGame>>> GetGames([FromQuery] string? category)
        {
            var games = await _casinoServices.GetGamesAsync(category);
            return Ok(games);
        }

        [HttpGet("games/{gameId}")]
        public async Task<ActionResult<CasinoGame>> GetGameById(string gameId)
        {
            var game = await _casinoServices.GetGameByIdAsync(gameId);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpGet("games/popular")]
        public async Task<ActionResult<List<CasinoGame>>> GetPopularGames()
        {
            var games = await _casinoServices.GetPopularGamesAsync();
            return Ok(games);
        }

        [HttpGet("slots")]
        public async Task<ActionResult<List<SlotGame>>> GetSlots()
        {
            var slots = await _casinoServices.GetSlotsAsync();
            return Ok(slots);
        }

        [HttpGet("slots/{slotId}")]
        public async Task<ActionResult<SlotGame>> GetSlotById(string slotId)
        {
            var slot = await _casinoServices.GetSlotByIdAsync(slotId);
            if (slot == null) return NotFound();
            return Ok(slot);
        }

        [HttpGet("slots/progressive")]
        public async Task<ActionResult<List<SlotGame>>> GetProgressiveSlots()
        {
            var slots = await _casinoServices.GetProgressiveSlotsAsync();
            return Ok(slots);
        }

        [HttpGet("table-games")]
        public async Task<ActionResult<List<TableGame>>> GetTableGames()
        {
            var games = await _casinoServices.GetTableGamesAsync();
            return Ok(games);
        }

        [HttpGet("live-tables")]
        public async Task<ActionResult<List<LiveCasinoTable>>> GetLiveTables()
        {
            var tables = await _casinoServices.GetLiveTablesAsync();
            return Ok(tables);
        }

        [HttpGet("video-poker")]
        public async Task<ActionResult<List<VideoPokerGame>>> GetVideoPokerGames()
        {
            var games = await _casinoServices.GetVideoPokerGamesAsync();
            return Ok(games);
        }

        [HttpPost("sessions")]
        public async Task<ActionResult<GameSession>> StartSession([FromBody] StartGameRequest request)
        {
            try
            {
                var session = await _casinoServices.StartGameSessionAsync(request);
                return CreatedAtAction(nameof(GetSessionById), new { sessionId = session.Id }, session);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sessions/{sessionId}")]
        public async Task<ActionResult<GameSession>> GetSessionById(string sessionId)
        {
            var session = await _casinoServices.GetSessionByIdAsync(sessionId);
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpPost("sessions/{sessionId}/play")]
        public async Task<ActionResult<GameResult>> PlayGame(string sessionId, [FromBody] GameAction action)
        {
            try
            {
                var result = await _casinoServices.PlayGameAsync(sessionId, action);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sessions/{sessionId}/end")]
        public async Task<ActionResult> EndSession(string sessionId)
        {
            var result = await _casinoServices.EndSessionAsync(sessionId);
            if (!result) return NotFound();
            return Ok(new { message = "Session ended" });
        }

        [HttpGet("wallet/{userId}")]
        public async Task<ActionResult<CasinoWallet>> GetWallet(Guid userId)
        {
            var wallet = await _casinoServices.GetWalletAsync(userId);
            return Ok(wallet);
        }

        [HttpPost("wallet/{userId}/deposit")]
        public async Task<ActionResult<CasinoTransaction>> Deposit(Guid userId, [FromBody] decimal amount)
        {
            try
            {
                var transaction = await _casinoServices.DepositAsync(userId, amount);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("wallet/{userId}/withdraw")]
        public async Task<ActionResult<CasinoTransaction>> Withdraw(Guid userId, [FromBody] decimal amount)
        {
            try
            {
                var transaction = await _casinoServices.WithdrawAsync(userId, amount);
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("transactions/{userId}")]
        public async Task<ActionResult<List<CasinoTransaction>>> GetTransactions(Guid userId, [FromQuery] int limit = 50)
        {
            var transactions = await _casinoServices.GetTransactionsAsync(userId, limit);
            return Ok(transactions);
        }

        [HttpGet("vip/{userId}")]
        public async Task<ActionResult<VIPStatus>> GetVIPStatus(Guid userId)
        {
            var vip = await _casinoServices.GetVIPStatusAsync(userId);
            return Ok(vip);
        }

        [HttpGet("rewards/{userId}")]
        public async Task<ActionResult<List<Reward>>> GetRewards(Guid userId)
        {
            var rewards = await _casinoServices.GetRewardsAsync(userId);
            return Ok(rewards);
        }

        [HttpPost("rewards/{userId}/claim/{rewardId}")]
        public async Task<ActionResult<Reward>> ClaimReward(Guid userId, string rewardId)
        {
            var reward = await _casinoServices.ClaimRewardAsync(userId, rewardId);
            if (reward == null) return NotFound();
            return Ok(reward);
        }

        [HttpGet("statistics/{userId}")]
        public async Task<ActionResult<CasinoStatistics>> GetStatistics(Guid userId)
        {
            var stats = await _casinoServices.GetStatisticsAsync(userId);
            return Ok(stats);
        }
    }
}
