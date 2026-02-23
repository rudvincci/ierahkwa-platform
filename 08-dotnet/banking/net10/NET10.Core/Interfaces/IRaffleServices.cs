using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET10.Core.Interfaces
{
    public interface IRaffleServices
    {
        // Raffles
        Task<List<Raffle>> GetRafflesAsync(RaffleStatus? status = null);
        Task<Raffle?> GetRaffleByIdAsync(string raffleId);
        Task<List<Raffle>> GetActiveRafflesAsync();
        Task<List<Raffle>> GetUpcomingRafflesAsync();

        // Tickets
        Task<RaffleTicket> PurchaseTicketAsync(PurchaseRaffleTicketRequest request);
        Task<RaffleTicket?> GetTicketByIdAsync(string ticketId);
        Task<List<RaffleTicket>> GetUserTicketsAsync(Guid userId, string? raffleId = null);
        Task<int> GetTicketCountAsync(string raffleId);
        Task<int> GetUserTicketCountAsync(Guid userId, string raffleId);

        // Draws
        Task<RaffleDraw> CreateDrawAsync(string raffleId);
        Task<RaffleDraw?> GetDrawByIdAsync(string drawId);
        Task<RaffleDraw?> GetRaffleDrawAsync(string raffleId);
        Task<RaffleDrawResult> ExecuteDrawAsync(string raffleId);

        // Winners
        Task<List<RaffleWinner>> GetWinnersAsync(string raffleId);
        Task<List<RaffleWinner>> GetUserWinsAsync(Guid userId);
        Task<bool> ClaimPrizeAsync(string ticketId, Guid userId);

        // Statistics
        Task<RaffleStatistics> GetStatisticsAsync();
        Task<RaffleStatistics> GetRaffleStatisticsAsync(string raffleId);
        Task<RaffleStatistics> GetUserStatisticsAsync(Guid userId);
    }

    // Models
    public class Raffle
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RaffleType Type { get; set; }
        public string Category { get; set; } = string.Empty; // Car, House, Travel, Tech, etc.
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public int SoldTickets { get; set; }
        public int AvailableTickets => TotalTickets - SoldTickets;
        public DateTime StartDate { get; set; }
        public DateTime DrawDate { get; set; }
        public RaffleStatus Status { get; set; }
        public List<Prize> Prizes { get; set; } = new();
        public string? ImageUrl { get; set; }
        public decimal TotalValue { get; set; }
    }

    public enum RaffleType
    {
        SinglePrize,
        MultiPrize,
        Progressive
    }

    public enum RaffleStatus
    {
        Upcoming,
        Active,
        SoldOut,
        DrawPending,
        Completed,
        Cancelled
    }

    public class Prize
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class RaffleTicket
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string RaffleId { get; set; } = string.Empty;
        public string RaffleName { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PurchasedAt { get; set; }
        public RaffleTicketStatus Status { get; set; }
        public bool IsWinner { get; set; }
        public string? PrizeId { get; set; }
        public string? PrizeName { get; set; }
        public decimal? PrizeValue { get; set; }
    }

    public enum RaffleTicketStatus
    {
        Active,
        Won,
        Lost,
        Void
    }

    public class PurchaseRaffleTicketRequest
    {
        public Guid UserId { get; set; }
        public string RaffleId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }

    public class RaffleDraw
    {
        public string Id { get; set; } = string.Empty;
        public string RaffleId { get; set; } = string.Empty;
        public string RaffleName { get; set; } = string.Empty;
        public DateTime DrawTime { get; set; }
        public RaffleDrawStatus Status { get; set; }
        public List<string>? WinningTicketNumbers { get; set; }
        public Dictionary<string, string>? PrizeWinners { get; set; } // PrizeId -> TicketNumber
        public int TotalTickets { get; set; }
    }

    public enum RaffleDrawStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public class RaffleDrawResult
    {
        public string DrawId { get; set; } = string.Empty;
        public string RaffleId { get; set; } = string.Empty;
        public string RaffleName { get; set; } = string.Empty;
        public DateTime DrawTime { get; set; }
        public List<RaffleWinner> Winners { get; set; } = new();
        public bool IsSettled { get; set; }
    }

    public class RaffleWinner
    {
        public string TicketId { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string RaffleId { get; set; } = string.Empty;
        public string RaffleName { get; set; } = string.Empty;
        public string PrizeId { get; set; } = string.Empty;
        public string PrizeName { get; set; } = string.Empty;
        public decimal PrizeValue { get; set; }
        public DateTime WonAt { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime? ClaimedAt { get; set; }
    }

    public class RaffleStatistics
    {
        public int TotalRaffles { get; set; }
        public int ActiveRaffles { get; set; }
        public int CompletedRaffles { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPrizesAwarded { get; set; }
        public int TotalWinners { get; set; }
        public decimal? BiggestPrize { get; set; }
    }
}
