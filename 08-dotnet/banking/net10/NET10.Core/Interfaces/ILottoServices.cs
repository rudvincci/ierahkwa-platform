using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET10.Core.Interfaces
{
    public interface ILottoServices
    {
        // Lotto Games
        Task<List<LottoGame>> GetGamesAsync();
        Task<LottoGame?> GetGameByIdAsync(string gameId);
        Task<List<LottoGame>> GetActiveGamesAsync();

        // Tickets
        Task<LottoTicket> PurchaseTicketAsync(PurchaseTicketRequest request);
        Task<LottoTicket?> GetTicketByIdAsync(string ticketId);
        Task<List<LottoTicket>> GetUserTicketsAsync(Guid userId, string? gameId = null);
        Task<List<LottoTicket>> GetWinningTicketsAsync(string drawId);

        // Draws
        Task<List<LottoDraw>> GetDrawsAsync(string? gameId = null, int limit = 50);
        Task<LottoDraw?> GetDrawByIdAsync(string drawId);
        Task<LottoDraw?> GetNextDrawAsync(string gameId);
        Task<LottoDraw?> GetLastDrawAsync(string gameId);
        Task<LottoDraw> CreateDrawAsync(string gameId);

        // Results
        Task<DrawResult?> GetDrawResultAsync(string drawId);
        Task<List<DrawResult>> GetRecentResultsAsync(int limit = 10);
        Task<bool> CheckTicketWinAsync(string ticketId, string drawId);

        // Winners
        Task<List<LottoWinner>> GetWinnersAsync(string? drawId = null, int limit = 50);
        Task<List<LottoWinner>> GetBigWinnersAsync(int limit = 10);

        // Statistics
        Task<LottoStatistics> GetStatisticsAsync();
        Task<LottoStatistics> GetUserStatisticsAsync(Guid userId);
    }

    // Models
    public class LottoGame
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Classic, Express, Mega
        public string Description { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public int NumberCount { get; set; }
        public int MaxNumber { get; set; }
        public int BonusNumberCount { get; set; }
        public DateTime? NextDrawTime { get; set; }
        public decimal? CurrentJackpot { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, decimal> PrizeTiers { get; set; } = new();
    }

    public class LottoTicket
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public List<int> Numbers { get; set; } = new();
        public List<int>? BonusNumbers { get; set; }
        public string DrawId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PurchasedAt { get; set; }
        public LottoTicketStatus Status { get; set; }
        public decimal? WinAmount { get; set; }
        public string? PrizeTier { get; set; }
    }

    public enum LottoTicketStatus
    {
        Pending,
        Active,
        Won,
        Lost,
        Void
    }

    public class PurchaseTicketRequest
    {
        public Guid UserId { get; set; }
        public string GameId { get; set; } = string.Empty;
        public List<int> Numbers { get; set; } = new();
        public List<int>? BonusNumbers { get; set; }
        public int Quantity { get; set; } = 1;
        public bool QuickPick { get; set; } = false;
    }

    public class LottoDraw
    {
        public string Id { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public DateTime DrawTime { get; set; }
        public LottoDrawStatus Status { get; set; }
        public List<int>? WinningNumbers { get; set; }
        public List<int>? BonusNumbers { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalPrizePool { get; set; }
        public decimal? Jackpot { get; set; }
    }

    public enum LottoDrawStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public class DrawResult
    {
        public string DrawId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public DateTime DrawTime { get; set; }
        public List<int> WinningNumbers { get; set; } = new();
        public List<int> BonusNumbers { get; set; } = new();
        public Dictionary<string, int> WinnersByTier { get; set; } = new();
        public Dictionary<string, decimal> PrizeByTier { get; set; } = new();
        public decimal TotalPrizePool { get; set; }
        public bool IsSettled { get; set; }
    }

    public class LottoWinner
    {
        public string TicketId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string DrawId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public string PrizeTier { get; set; } = string.Empty;
        public decimal WinAmount { get; set; }
        public DateTime WonAt { get; set; }
        public bool IsClaimed { get; set; }
    }

    public class LottoStatistics
    {
        public int TotalGames { get; set; }
        public int ActiveGames { get; set; }
        public int TotalDraws { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalPrizePool { get; set; }
        public decimal TotalPayouts { get; set; }
        public int TotalWinners { get; set; }
        public decimal? BiggestJackpot { get; set; }
        public DateTime? LastDrawTime { get; set; }
    }
}
