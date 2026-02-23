using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleServices _raffleServices;

        public RaffleController(IRaffleServices raffleServices)
        {
            _raffleServices = raffleServices;
        }

        [HttpGet]
        public async Task<ActionResult<List<Raffle>>> GetRaffles([FromQuery] RaffleStatus? status)
        {
            var raffles = await _raffleServices.GetRafflesAsync(status);
            return Ok(raffles);
        }

        [HttpGet("{raffleId}")]
        public async Task<ActionResult<Raffle>> GetRaffleById(string raffleId)
        {
            var raffle = await _raffleServices.GetRaffleByIdAsync(raffleId);
            if (raffle == null) return NotFound();
            return Ok(raffle);
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<Raffle>>> GetActiveRaffles()
        {
            var raffles = await _raffleServices.GetActiveRafflesAsync();
            return Ok(raffles);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<Raffle>>> GetUpcomingRaffles()
        {
            var raffles = await _raffleServices.GetUpcomingRafflesAsync();
            return Ok(raffles);
        }

        [HttpPost("tickets/purchase")]
        public async Task<ActionResult<RaffleTicket>> PurchaseTicket([FromBody] PurchaseRaffleTicketRequest request)
        {
            try
            {
                var ticket = await _raffleServices.PurchaseTicketAsync(request);
                return CreatedAtAction(nameof(GetTicketById), new { ticketId = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tickets/{ticketId}")]
        public async Task<ActionResult<RaffleTicket>> GetTicketById(string ticketId)
        {
            var ticket = await _raffleServices.GetTicketByIdAsync(ticketId);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet("tickets/user/{userId}")]
        public async Task<ActionResult<List<RaffleTicket>>> GetUserTickets(Guid userId, [FromQuery] string? raffleId)
        {
            var tickets = await _raffleServices.GetUserTicketsAsync(userId, raffleId);
            return Ok(tickets);
        }

        [HttpGet("tickets/count/{raffleId}")]
        public async Task<ActionResult<int>> GetTicketCount(string raffleId)
        {
            var count = await _raffleServices.GetTicketCountAsync(raffleId);
            return Ok(count);
        }

        [HttpGet("tickets/user/{userId}/count/{raffleId}")]
        public async Task<ActionResult<int>> GetUserTicketCount(Guid userId, string raffleId)
        {
            var count = await _raffleServices.GetUserTicketCountAsync(userId, raffleId);
            return Ok(count);
        }

        [HttpPost("draws")]
        public async Task<ActionResult<RaffleDraw>> CreateDraw([FromBody] string raffleId)
        {
            try
            {
                var draw = await _raffleServices.CreateDrawAsync(raffleId);
                return CreatedAtAction(nameof(GetDrawById), new { drawId = draw.Id }, draw);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("draws/{drawId}")]
        public async Task<ActionResult<RaffleDraw>> GetDrawById(string drawId)
        {
            var draw = await _raffleServices.GetDrawByIdAsync(drawId);
            if (draw == null) return NotFound();
            return Ok(draw);
        }

        [HttpGet("draws/raffle/{raffleId}")]
        public async Task<ActionResult<RaffleDraw>> GetRaffleDraw(string raffleId)
        {
            var draw = await _raffleServices.GetRaffleDrawAsync(raffleId);
            if (draw == null) return NotFound();
            return Ok(draw);
        }

        [HttpPost("draws/{raffleId}/execute")]
        public async Task<ActionResult<RaffleDrawResult>> ExecuteDraw(string raffleId)
        {
            try
            {
                var result = await _raffleServices.ExecuteDrawAsync(raffleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("winners/{raffleId}")]
        public async Task<ActionResult<List<RaffleWinner>>> GetWinners(string raffleId)
        {
            var winners = await _raffleServices.GetWinnersAsync(raffleId);
            return Ok(winners);
        }

        [HttpGet("winners/user/{userId}")]
        public async Task<ActionResult<List<RaffleWinner>>> GetUserWins(Guid userId)
        {
            var winners = await _raffleServices.GetUserWinsAsync(userId);
            return Ok(winners);
        }

        [HttpPost("winners/claim")]
        public async Task<ActionResult> ClaimPrize([FromBody] ClaimPrizeRequest request)
        {
            var result = await _raffleServices.ClaimPrizeAsync(request.TicketId, request.UserId);
            if (!result) return BadRequest("Prize cannot be claimed");
            return Ok(new { message = "Prize claimed successfully" });
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<RaffleStatistics>> GetStatistics()
        {
            var stats = await _raffleServices.GetStatisticsAsync();
            return Ok(stats);
        }

        [HttpGet("statistics/raffle/{raffleId}")]
        public async Task<ActionResult<RaffleStatistics>> GetRaffleStatistics(string raffleId)
        {
            var stats = await _raffleServices.GetRaffleStatisticsAsync(raffleId);
            return Ok(stats);
        }

        [HttpGet("statistics/user/{userId}")]
        public async Task<ActionResult<RaffleStatistics>> GetUserStatistics(Guid userId)
        {
            var stats = await _raffleServices.GetUserStatisticsAsync(userId);
            return Ok(stats);
        }
    }

    public class ClaimPrizeRequest
    {
        public string TicketId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
