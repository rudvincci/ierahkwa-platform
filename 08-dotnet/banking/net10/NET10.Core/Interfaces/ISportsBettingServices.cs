using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET10.Core.Interfaces
{
    public interface ISportsBettingServices
    {
        // Sports & Events
        Task<List<Sport>> GetSportsAsync();
        Task<List<SportEvent>> GetEventsAsync(string? sportId = null, DateTime? date = null);
        Task<SportEvent?> GetEventByIdAsync(string eventId);
        Task<List<SportEvent>> GetLiveEventsAsync();
        Task<List<SportEvent>> GetUpcomingEventsAsync(int hours = 24);

        // Betting Markets
        Task<List<BettingMarket>> GetMarketsAsync(string eventId);
        Task<BettingMarket?> GetMarketByIdAsync(string marketId);
        Task<List<BettingMarket>> GetPopularMarketsAsync();

        // Odds
        Task<List<Odds>> GetOddsAsync(string marketId);
        Task<Odds?> GetOddsByIdAsync(string oddsId);
        Task<List<Odds>> GetBestOddsAsync(string eventId, string marketType);

        // Bets
        Task<Bet> PlaceBetAsync(PlaceBetRequest request);
        Task<Bet?> GetBetByIdAsync(string betId);
        Task<List<Bet>> GetUserBetsAsync(Guid userId, BetStatus? status = null);
        Task<bool> CancelBetAsync(string betId, Guid userId);
        Task<Bet> CashOutBetAsync(string betId, Guid userId);

        // Bet Slips
        Task<BetSlip> CreateBetSlipAsync(Guid userId);
        Task<BetSlip?> GetBetSlipAsync(Guid userId);
        Task<BetSlip> AddToBetSlipAsync(Guid userId, AddToBetSlipRequest request);
        Task<bool> RemoveFromBetSlipAsync(Guid userId, string selectionId);
        Task<BetSlip> ClearBetSlipAsync(Guid userId);
        Task<Bet> PlaceBetSlipAsync(Guid userId, decimal stake);

        // Results & Settlements
        Task<List<EventResult>> GetResultsAsync(DateTime? date = null, string? sportId = null);
        Task<EventResult?> GetEventResultAsync(string eventId);
        Task<bool> SettleBetsAsync(string eventId);

        // Statistics
        Task<BettingStatistics> GetStatisticsAsync(Guid userId);
        Task<List<PopularBet>> GetPopularBetsAsync();
        Task<List<TrendingEvent>> GetTrendingEventsAsync();
    }

    // Models
    public class Sport
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int ActiveEvents { get; set; }
        public bool IsLive { get; set; }
        public int Priority { get; set; }
    }

    public class SportEvent
    {
        public string Id { get; set; } = string.Empty;
        public string SportId { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public string HomeTeamLogo { get; set; } = string.Empty;
        public string AwayTeamLogo { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public EventStatus Status { get; set; }
        public string? Score { get; set; }
        public string? League { get; set; }
        public string? Country { get; set; }
        public int MarketsCount { get; set; }
        public bool IsLive { get; set; }
        public List<BettingMarket> Markets { get; set; } = new();
    }

    public enum EventStatus
    {
        Scheduled,
        Live,
        Finished,
        Cancelled,
        Postponed
    }

    public class BettingMarket
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public MarketType Type { get; set; }
        public bool IsActive { get; set; }
        public List<Odds> Odds { get; set; } = new();
    }

    public enum MarketType
    {
        MatchWinner,
        OverUnder,
        Handicap,
        BothTeamsToScore,
        CorrectScore,
        FirstGoalScorer,
        TotalGoals,
        HalfTimeFullTime,
        DoubleChance,
        DrawNoBet
    }

    public class Odds
    {
        public string Id { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string Selection { get; set; } = string.Empty;
        public decimal DecimalOdds { get; set; }
        public string FractionalOdds { get; set; } = string.Empty;
        public string AmericanOdds { get; set; } = string.Empty;
        public decimal Probability { get; set; }
        public bool IsActive { get; set; }
    }

    public class Bet
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public string Selection { get; set; } = string.Empty;
        public decimal Stake { get; set; }
        public decimal Odds { get; set; }
        public decimal PotentialWin { get; set; }
        public BetStatus Status { get; set; }
        public DateTime PlacedAt { get; set; }
        public DateTime? SettledAt { get; set; }
        public decimal? Payout { get; set; }
        public string? Result { get; set; }
    }

    public enum BetStatus
    {
        Pending,
        Won,
        Lost,
        Void,
        Cancelled,
        CashOut
    }

    public class PlaceBetRequest
    {
        public Guid UserId { get; set; }
        public string EventId { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string OddsId { get; set; } = string.Empty;
        public decimal Stake { get; set; }
    }

    public class BetSlip
    {
        public Guid UserId { get; set; }
        public List<BetSlipSelection> Selections { get; set; } = new();
        public decimal TotalStake { get; set; }
        public decimal TotalPotentialWin { get; set; }
        public decimal CombinedOdds { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BetSlipSelection
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public string Selection { get; set; } = string.Empty;
        public decimal Odds { get; set; }
    }

    public class AddToBetSlipRequest
    {
        public string EventId { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string OddsId { get; set; } = string.Empty;
        public string Selection { get; set; } = string.Empty;
        public decimal Odds { get; set; }
    }

    public class EventResult
    {
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string SportId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? FinalScore { get; set; }
        public Dictionary<string, string> MarketResults { get; set; } = new();
        public bool IsSettled { get; set; }
    }

    public class BettingStatistics
    {
        public Guid UserId { get; set; }
        public int TotalBets { get; set; }
        public int WonBets { get; set; }
        public int LostBets { get; set; }
        public int PendingBets { get; set; }
        public decimal TotalStaked { get; set; }
        public decimal TotalWon { get; set; }
        public decimal TotalLost { get; set; }
        public decimal NetProfit { get; set; }
        public decimal WinRate { get; set; }
        public decimal AverageOdds { get; set; }
    }

    public class PopularBet
    {
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string MarketName { get; set; } = string.Empty;
        public string Selection { get; set; } = string.Empty;
        public int BetCount { get; set; }
        public decimal TotalStake { get; set; }
    }

    public class TrendingEvent
    {
        public string EventId { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int BetCount { get; set; }
        public DateTime StartTime { get; set; }
    }
}
