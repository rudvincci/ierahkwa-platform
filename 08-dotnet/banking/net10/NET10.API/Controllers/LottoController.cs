using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LottoController : ControllerBase
    {
        private readonly ILottoServices _lottoServices;

        public LottoController(ILottoServices lottoServices)
        {
            _lottoServices = lottoServices;
        }

        [HttpGet("games")]
        public async Task<ActionResult<List<LottoGame>>> GetGames()
        {
            var games = await _lottoServices.GetGamesAsync();
            return Ok(games);
        }

        [HttpGet("games/{gameId}")]
        public async Task<ActionResult<LottoGame>> GetGameById(string gameId)
        {
            var game = await _lottoServices.GetGameByIdAsync(gameId);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpGet("games/active")]
        public async Task<ActionResult<List<LottoGame>>> GetActiveGames()
        {
            var games = await _lottoServices.GetActiveGamesAsync();
            return Ok(games);
        }

        [HttpPost("tickets/purchase")]
        public async Task<ActionResult<LottoTicket>> PurchaseTicket([FromBody] PurchaseTicketRequest request)
        {
            try
            {
                var ticket = await _lottoServices.PurchaseTicketAsync(request);
                return CreatedAtAction(nameof(GetTicketById), new { ticketId = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tickets/{ticketId}")]
        public async Task<ActionResult<LottoTicket>> GetTicketById(string ticketId)
        {
            var ticket = await _lottoServices.GetTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet("tickets/user/{userId}")]
        public async Task<ActionResult<List<LottoTicket>>> GetUserTickets(Guid userId, [FromQuery] string? gameId)
        {
            var tickets = await _lottoServices.GetUserTicketsAsync(userId, gameId);
            return Ok(tickets);
        }

        [HttpGet("draws")]
        public async Task<ActionResult<List<LottoDraw>>> GetDraws([FromQuery] string? gameId, [FromQuery] int limit = 50)
        {
            var draws = await _lottoServices.GetDrawsAsync(gameId, limit);
            return Ok(draws);
        }

        [HttpGet("draws/{drawId}")]
        public async Task<ActionResult<LottoDraw>> GetDrawById(string drawId)
        {
            var draw = await _lottoServices.GetDrawByIdAsync(drawId);
            if (draw == null) return NotFound();
            return Ok(draw);
        }

        [HttpGet("draws/{gameId}/next")]
        public async Task<ActionResult<LottoDraw>> GetNextDraw(string gameId)
        {
            var draw = await _lottoServices.GetNextDrawAsync(gameId);
            if (draw == null) return NotFound();
            return Ok(draw);
        }

        [HttpGet("draws/{gameId}/last")]
        public async Task<ActionResult<LottoDraw>> GetLastDraw(string gameId)
        {
            var draw = await _lottoServices.GetLastDrawAsync(gameId);
            if (draw == null) return NotFound();
            return Ok(draw);
        }

        [HttpPost("draws")]
        public async Task<ActionResult<LottoDraw>> CreateDraw([FromBody] string gameId)
        {
            try
            {
                var draw = await _lottoServices.CreateDrawAsync(gameId);
                return CreatedAtAction(nameof(GetDrawById), new { drawId = draw.Id }, draw);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("results/{drawId}")]
        public async Task<ActionResult<DrawResult>> GetDrawResult(string drawId)
        {
            var result = await _lottoServices.GetDrawResultAsync(drawId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("results/recent")]
        public async Task<ActionResult<List<DrawResult>>> GetRecentResults([FromQuery] int limit = 10)
        {
            var results = await _lottoServices.GetRecentResultsAsync(limit);
            return Ok(results);
        }

        [HttpGet("winners")]
        public async Task<ActionResult<List<LottoWinner>>> GetWinners([FromQuery] string? drawId, [FromQuery] int limit = 50)
        {
            var winners = await _lottoServices.GetWinnersAsync(drawId, limit);
            return Ok(winners);
        }

        [HttpGet("winners/big")]
        public async Task<ActionResult<List<LottoWinner>>> GetBigWinners([FromQuery] int limit = 10)
        {
            var winners = await _lottoServices.GetBigWinnersAsync(limit);
            return Ok(winners);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<LottoStatistics>> GetStatistics()
        {
            var stats = await _lottoServices.GetStatisticsAsync();
            return Ok(stats);
        }

        [HttpGet("statistics/user/{userId}")]
        public async Task<ActionResult<LottoStatistics>> GetUserStatistics(Guid userId)
        {
            var stats = await _lottoServices.GetUserStatisticsAsync(userId);
            return Ok(stats);
        }
    }
}
