using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NET10.Core.Interfaces
{
    public interface ICasinoServices
    {
        // Games
        Task<List<CasinoGame>> GetGamesAsync(string? category = null);
        Task<CasinoGame?> GetGameByIdAsync(string gameId);
        Task<List<CasinoGame>> GetPopularGamesAsync();
        Task<List<CasinoGame>> GetLiveCasinoGamesAsync();

        // Slots
        Task<List<SlotGame>> GetSlotsAsync();
        Task<SlotGame?> GetSlotByIdAsync(string slotId);
        Task<List<SlotGame>> GetProgressiveSlotsAsync();

        // Table Games
        Task<List<TableGame>> GetTableGamesAsync();
        Task<TableGame?> GetTableGameByIdAsync(string gameId);

        // Live Casino
        Task<List<LiveCasinoTable>> GetLiveTablesAsync();
        Task<LiveCasinoTable?> GetLiveTableByIdAsync(string tableId);

        // Video Poker
        Task<List<VideoPokerGame>> GetVideoPokerGamesAsync();
        Task<VideoPokerGame?> GetVideoPokerByIdAsync(string gameId);

        // Game Sessions
        Task<GameSession> StartGameSessionAsync(StartGameRequest request);
        Task<GameSession?> GetSessionByIdAsync(string sessionId);
        Task<GameResult> PlayGameAsync(string sessionId, GameAction action);
        Task<bool> EndSessionAsync(string sessionId);

        // Wallet & Transactions
        Task<CasinoWallet> GetWalletAsync(Guid userId);
        Task<CasinoTransaction> DepositAsync(Guid userId, decimal amount);
        Task<CasinoTransaction> WithdrawAsync(Guid userId, decimal amount);
        Task<List<CasinoTransaction>> GetTransactionsAsync(Guid userId, int limit = 50);

        // VIP & Rewards
        Task<VIPStatus> GetVIPStatusAsync(Guid userId);
        Task<List<Reward>> GetRewardsAsync(Guid userId);
        Task<Reward?> ClaimRewardAsync(Guid userId, string rewardId);

        // Statistics
        Task<CasinoStatistics> GetStatisticsAsync(Guid userId);
    }

    // Models
    public class CasinoGame
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal RTP { get; set; }
        public bool IsLive { get; set; }
        public bool IsPopular { get; set; }
    }

    public class SlotGame : CasinoGame
    {
        public int Reels { get; set; }
        public int Paylines { get; set; }
        public bool IsProgressive { get; set; }
        public decimal? JackpotAmount { get; set; }
        public string Theme { get; set; } = string.Empty;
    }

    public class TableGame : CasinoGame
    {
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string Rules { get; set; } = string.Empty;
    }

    public class LiveCasinoTable
    {
        public string Id { get; set; } = string.Empty;
        public string GameType { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public bool IsAvailable { get; set; }
        public string StreamUrl { get; set; } = string.Empty;
    }

    public class VideoPokerGame : CasinoGame
    {
        public string Variant { get; set; } = string.Empty;
        public int HandSize { get; set; }
    }

    public class GameSession
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string GameId { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public CasinoSessionStatus Status { get; set; }
    }

    public enum CasinoSessionStatus
    {
        Active,
        Ended,
        Paused
    }

    public class StartGameRequest
    {
        public Guid UserId { get; set; }
        public string GameId { get; set; } = string.Empty;
        public decimal InitialBet { get; set; }
    }

    public class GameAction
    {
        public string Type { get; set; } = string.Empty; // "spin", "bet", "hit", "stand", etc.
        public decimal? Amount { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
    }

    public class GameResult
    {
        public bool Won { get; set; }
        public decimal WinAmount { get; set; }
        public decimal NewBalance { get; set; }
        public Dictionary<string, object>? GameData { get; set; }
        public string? Message { get; set; }
    }

    public class CasinoWallet
    {
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal TotalWon { get; set; }
        public decimal TotalWagered { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class CasinoTransaction
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public CasinoTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CasinoTransactionStatus Status { get; set; }
        public string? Description { get; set; }
    }

    public enum CasinoTransactionType
    {
        Deposit,
        Withdrawal,
        Bet,
        Win,
        Bonus,
        Refund
    }

    public enum CasinoTransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Cancelled
    }

    public class VIPStatus
    {
        public Guid UserId { get; set; }
        public VIPLevel Level { get; set; }
        public decimal Points { get; set; }
        public decimal PointsToNextLevel { get; set; }
        public decimal CashbackPercent { get; set; }
        public List<string> Benefits { get; set; } = new();
    }

    public enum VIPLevel
    {
        Bronze,
        Silver,
        Gold,
        Diamond,
        Platinum
    }

    public class Reward
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RewardType Type { get; set; }
        public decimal? Amount { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime? ClaimedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public enum RewardType
    {
        Bonus,
        FreeSpin,
        Cashback,
        TournamentEntry,
        VIPEvent
    }

    public class CasinoStatistics
    {
        public Guid UserId { get; set; }
        public int TotalGamesPlayed { get; set; }
        public decimal TotalWagered { get; set; }
        public decimal TotalWon { get; set; }
        public decimal NetProfit { get; set; }
        public decimal WinRate { get; set; }
        public string FavoriteGame { get; set; } = string.Empty;
        public int LongestWinStreak { get; set; }
    }
}
