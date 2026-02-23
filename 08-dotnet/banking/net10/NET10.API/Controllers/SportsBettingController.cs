using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SportsBettingController : ControllerBase
    {
        private readonly ISportsBettingServices _sportsBettingServices;

        public SportsBettingController(ISportsBettingServices sportsBettingServices)
        {
            _sportsBettingServices = sportsBettingServices;
        }

        // Sports
        [HttpGet("sports")]
        public async Task<ActionResult<List<Sport>>> GetSports()
        {
            var sports = await _sportsBettingServices.GetSportsAsync();
            return Ok(sports);
        }

        // Events
        [HttpGet("events")]
        public async Task<ActionResult<List<SportEvent>>> GetEvents([FromQuery] string? sportId, [FromQuery] DateTime? date)
        {
            var events = await _sportsBettingServices.GetEventsAsync(sportId, date);
            return Ok(events);
        }

        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<SportEvent>> GetEventById(string eventId)
        {
            var evt = await _sportsBettingServices.GetEventByIdAsync(eventId);
            if (evt == null) return NotFound();
            return Ok(evt);
        }

        [HttpGet("events/live")]
        public async Task<ActionResult<List<SportEvent>>> GetLiveEvents()
        {
            var events = await _sportsBettingServices.GetLiveEventsAsync();
            return Ok(events);
        }

        [HttpGet("events/upcoming")]
        public async Task<ActionResult<List<SportEvent>>> GetUpcomingEvents([FromQuery] int hours = 24)
        {
            var events = await _sportsBettingServices.GetUpcomingEventsAsync(hours);
            return Ok(events);
        }

        // Markets
        [HttpGet("events/{eventId}/markets")]
        public async Task<ActionResult<List<BettingMarket>>> GetMarkets(string eventId)
        {
            var markets = await _sportsBettingServices.GetMarketsAsync(eventId);
            return Ok(markets);
        }

        [HttpGet("markets/{marketId}")]
        public async Task<ActionResult<BettingMarket>> GetMarketById(string marketId)
        {
            var market = await _sportsBettingServices.GetMarketByIdAsync(marketId);
            if (market == null) return NotFound();
            return Ok(market);
        }

        [HttpGet("markets/popular")]
        public async Task<ActionResult<List<BettingMarket>>> GetPopularMarkets()
        {
            var markets = await _sportsBettingServices.GetPopularMarketsAsync();
            return Ok(markets);
        }

        // Odds
        [HttpGet("markets/{marketId}/odds")]
        public async Task<ActionResult<List<Odds>>> GetOdds(string marketId)
        {
            var odds = await _sportsBettingServices.GetOddsAsync(marketId);
            return Ok(odds);
        }

        [HttpGet("odds/{oddsId}")]
        public async Task<ActionResult<Odds>> GetOddsById(string oddsId)
        {
            var odds = await _sportsBettingServices.GetOddsByIdAsync(oddsId);
            if (odds == null) return NotFound();
            return Ok(odds);
        }

        [HttpGet("events/{eventId}/best-odds")]
        public async Task<ActionResult<List<Odds>>> GetBestOdds(string eventId, [FromQuery] string marketType)
        {
            var odds = await _sportsBettingServices.GetBestOddsAsync(eventId, marketType);
            return Ok(odds);
        }

        // Bets
        [HttpPost("bets")]
        public async Task<ActionResult<Bet>> PlaceBet([FromBody] PlaceBetRequest request)
        {
            try
            {
                var bet = await _sportsBettingServices.PlaceBetAsync(request);
                return CreatedAtAction(nameof(GetBetById), new { betId = bet.Id }, bet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("bets/{betId}")]
        public async Task<ActionResult<Bet>> GetBetById(string betId)
        {
            var bet = await _sportsBettingServices.GetBetByIdAsync(betId);
            if (bet == null) return NotFound();
            return Ok(bet);
        }

        [HttpGet("users/{userId}/bets")]
        public async Task<ActionResult<List<Bet>>> GetUserBets(Guid userId, [FromQuery] BetStatus? status)
        {
            var bets = await _sportsBettingServices.GetUserBetsAsync(userId, status);
            return Ok(bets);
        }

        [HttpPost("bets/{betId}/cancel")]
        public async Task<ActionResult> CancelBet(string betId, [FromBody] Guid userId)
        {
            var result = await _sportsBettingServices.CancelBetAsync(betId, userId);
            if (!result) return BadRequest("Bet cannot be cancelled");
            return Ok(new { message = "Bet cancelled successfully" });
        }

        [HttpPost("bets/{betId}/cashout")]
        public async Task<ActionResult<Bet>> CashOutBet(string betId, [FromBody] Guid userId)
        {
            try
            {
                var bet = await _sportsBettingServices.CashOutBetAsync(betId, userId);
                return Ok(bet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Bet Slips
        [HttpPost("betslips")]
        public async Task<ActionResult<BetSlip>> CreateBetSlip([FromBody] Guid userId)
        {
            var betSlip = await _sportsBettingServices.CreateBetSlipAsync(userId);
            return Ok(betSlip);
        }

        [HttpGet("users/{userId}/betslip")]
        public async Task<ActionResult<BetSlip>> GetBetSlip(Guid userId)
        {
            var betSlip = await _sportsBettingServices.GetBetSlipAsync(userId);
            if (betSlip == null) return NotFound();
            return Ok(betSlip);
        }

        [HttpPost("users/{userId}/betslip/add")]
        public async Task<ActionResult<BetSlip>> AddToBetSlip(Guid userId, [FromBody] AddToBetSlipRequest request)
        {
            var betSlip = await _sportsBettingServices.AddToBetSlipAsync(userId, request);
            return Ok(betSlip);
        }

        [HttpDelete("users/{userId}/betslip/{selectionId}")]
        public async Task<ActionResult> RemoveFromBetSlip(Guid userId, string selectionId)
        {
            var result = await _sportsBettingServices.RemoveFromBetSlipAsync(userId, selectionId);
            if (!result) return NotFound();
            return Ok(new { message = "Selection removed" });
        }

        [HttpDelete("users/{userId}/betslip")]
        public async Task<ActionResult<BetSlip>> ClearBetSlip(Guid userId)
        {
            var betSlip = await _sportsBettingServices.ClearBetSlipAsync(userId);
            return Ok(betSlip);
        }

        [HttpPost("users/{userId}/betslip/place")]
        public async Task<ActionResult<Bet>> PlaceBetSlip(Guid userId, [FromBody] decimal stake)
        {
            try
            {
                var bet = await _sportsBettingServices.PlaceBetSlipAsync(userId, stake);
                return Ok(bet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Results
        [HttpGet("results")]
        public async Task<ActionResult<List<EventResult>>> GetResults([FromQuery] DateTime? date, [FromQuery] string? sportId)
        {
            var results = await _sportsBettingServices.GetResultsAsync(date, sportId);
            return Ok(results);
        }

        [HttpGet("results/{eventId}")]
        public async Task<ActionResult<EventResult>> GetEventResult(string eventId)
        {
            var result = await _sportsBettingServices.GetEventResultAsync(eventId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("results/{eventId}/settle")]
        public async Task<ActionResult> SettleBets(string eventId)
        {
            var result = await _sportsBettingServices.SettleBetsAsync(eventId);
            if (!result) return BadRequest("Event cannot be settled");
            return Ok(new { message = "Bets settled successfully" });
        }

        // Statistics
        [HttpGet("users/{userId}/statistics")]
        public async Task<ActionResult<BettingStatistics>> GetStatistics(Guid userId)
        {
            var stats = await _sportsBettingServices.GetStatisticsAsync(userId);
            return Ok(stats);
        }

        [HttpGet("bets/popular")]
        public async Task<ActionResult<List<PopularBet>>> GetPopularBets()
        {
            var bets = await _sportsBettingServices.GetPopularBetsAsync();
            return Ok(bets);
        }

        [HttpGet("events/trending")]
        public async Task<ActionResult<List<TrendingEvent>>> GetTrendingEvents()
        {
            var events = await _sportsBettingServices.GetTrendingEventsAsync();
            return Ok(events);
        }
    }
}
