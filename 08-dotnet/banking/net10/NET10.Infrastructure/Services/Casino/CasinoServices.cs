using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Casino
{
    public class CasinoServices : ICasinoServices
    {
        private readonly List<CasinoGame> _games;
        private readonly List<SlotGame> _slots;
        private readonly List<TableGame> _tableGames;
        private readonly List<LiveCasinoTable> _liveTables;
        private readonly List<VideoPokerGame> _videoPoker;
        private readonly Dictionary<string, GameSession> _sessions;
        private readonly Dictionary<Guid, CasinoWallet> _wallets;
        private readonly List<CasinoTransaction> _transactions;
        private readonly Dictionary<Guid, VIPStatus> _vipStatuses;
        private readonly Dictionary<Guid, List<Reward>> _rewards;

        public CasinoServices()
        {
            _games = new List<CasinoGame>();
            _slots = new List<SlotGame>();
            _tableGames = new List<TableGame>();
            _liveTables = new List<LiveCasinoTable>();
            _videoPoker = new List<VideoPokerGame>();
            _sessions = new Dictionary<string, GameSession>();
            _wallets = new Dictionary<Guid, CasinoWallet>();
            _transactions = new List<CasinoTransaction>();
            _vipStatuses = new Dictionary<Guid, VIPStatus>();
            _rewards = new Dictionary<Guid, List<Reward>>();

            InitializeGames();
            InitializeSlots();
            InitializeTableGames();
            InitializeLiveTables();
            InitializeVideoPoker();
        }

        private void InitializeGames()
        {
            _games.AddRange(new[]
            {
                new CasinoGame { Id = "blackjack", Name = "Blackjack", Category = "Table", Icon = "🃏", MinBet = 5, MaxBet = 5000, RTP = 99.5m, IsPopular = true },
                new CasinoGame { Id = "roulette", Name = "Roulette", Category = "Table", Icon = "🎡", MinBet = 1, MaxBet = 10000, RTP = 97.3m, IsPopular = true },
                new CasinoGame { Id = "baccarat", Name = "Baccarat", Category = "Table", Icon = "🎴", MinBet = 10, MaxBet = 5000, RTP = 98.9m, IsPopular = true },
                new CasinoGame { Id = "craps", Name = "Craps", Category = "Table", Icon = "🎲", MinBet = 5, MaxBet = 3000, RTP = 98.6m, IsPopular = false },
                new CasinoGame { Id = "keno", Name = "Keno", Category = "Lottery", Icon = "🎯", MinBet = 1, MaxBet = 100, RTP = 75.0m, IsPopular = false },
                new CasinoGame { Id = "wheel", Name = "Wheel of Fortune", Category = "Wheel", Icon = "🎪", MinBet = 1, MaxBet = 500, RTP = 96.5m, IsPopular = true },
                new CasinoGame { Id = "sicbo", Name = "Sic Bo", Category = "Dice", Icon = "💰", MinBet = 5, MaxBet = 2000, RTP = 97.2m, IsPopular = false }
            });
        }

        private void InitializeSlots()
        {
            _slots.AddRange(new[]
            {
                new SlotGame { Id = "diamond", Name = "Diamond Rush", Category = "Slots", Icon = "💎", MinBet = 0.01m, MaxBet = 100, RTP = 96.5m, Reels = 5, Paylines = 25, IsProgressive = true, JackpotAmount = 2500000, Theme = "Gems" },
                new SlotGame { Id = "dragon", Name = "Dragon Gold", Category = "Slots", Icon = "🐉", MinBet = 0.10m, MaxBet = 200, RTP = 96.8m, Reels = 5, Paylines = 30, IsProgressive = true, JackpotAmount = 1800000, Theme = "Fantasy" },
                new SlotGame { Id = "zeus", Name = "Zeus Thunder", Category = "Slots", Icon = "⚡", MinBet = 0.25m, MaxBet = 250, RTP = 97.1m, Reels = 5, Paylines = 40, IsProgressive = false, Theme = "Mythology" },
                new SlotGame { Id = "ierahkwa", Name = "Ierahkwa Riches", Category = "Slots", Icon = "🏛️", MinBet = 0.50m, MaxBet = 500, RTP = 96.9m, Reels = 5, Paylines = 50, IsProgressive = true, JackpotAmount = 5000000, Theme = "Culture" },
                new SlotGame { Id = "starburst", Name = "Starburst", Category = "Slots", Icon = "🌟", MinBet = 0.10m, MaxBet = 100, RTP = 96.1m, Reels = 5, Paylines = 10, IsProgressive = false, Theme = "Space" },
                new SlotGame { Id = "cherry", Name = "Cherry Blast", Category = "Slots", Icon = "🍒", MinBet = 0.05m, MaxBet = 50, RTP = 95.8m, Reels = 3, Paylines = 5, IsProgressive = false, Theme = "Fruits" },
                new SlotGame { Id = "mega", Name = "Mega Moolah", Category = "Slots", Icon = "🎰", MinBet = 0.25m, MaxBet = 125, RTP = 96.0m, Reels = 5, Paylines = 25, IsProgressive = true, JackpotAmount = 10000000, Theme = "Safari" },
                new SlotGame { Id = "book", Name = "Book of Ra", Category = "Slots", Icon = "📚", MinBet = 0.20m, MaxBet = 200, RTP = 96.3m, Reels = 5, Paylines = 10, IsProgressive = false, Theme = "Adventure" }
            });
        }

        private void InitializeTableGames()
        {
            _tableGames.AddRange(new[]
            {
                new TableGame { Id = "blackjack", Name = "Blackjack", Category = "Table", Icon = "🃏", MinBet = 5, MaxBet = 5000, RTP = 99.5m, MinPlayers = 1, MaxPlayers = 7, Rules = "Standard Blackjack Rules" },
                new TableGame { Id = "roulette-eu", Name = "European Roulette", Category = "Table", Icon = "🎡", MinBet = 1, MaxBet = 10000, RTP = 97.3m, MinPlayers = 1, MaxPlayers = 8, Rules = "Single Zero Roulette" },
                new TableGame { Id = "roulette-us", Name = "American Roulette", Category = "Table", Icon = "🎡", MinBet = 1, MaxBet = 10000, RTP = 94.7m, MinPlayers = 1, MaxPlayers = 8, Rules = "Double Zero Roulette" },
                new TableGame { Id = "baccarat", Name = "Baccarat", Category = "Table", Icon = "🎴", MinBet = 10, MaxBet = 5000, RTP = 98.9m, MinPlayers = 1, MaxPlayers = 14, Rules = "Punto Banco" }
            });
        }

        private void InitializeLiveTables()
        {
            _liveTables.AddRange(new[]
            {
                new LiveCasinoTable { Id = "live-blackjack-1", GameType = "Blackjack", DealerName = "Sarah", Language = "English", CurrentPlayers = 3, MaxPlayers = 7, MinBet = 5, MaxBet = 5000, IsAvailable = true, StreamUrl = "https://stream.casino.ierahkwa.gov/live/blackjack/1" },
                new LiveCasinoTable { Id = "live-roulette-1", GameType = "Roulette", DealerName = "James", Language = "English", CurrentPlayers = 5, MaxPlayers = 8, MinBet = 1, MaxBet = 10000, IsAvailable = true, StreamUrl = "https://stream.casino.ierahkwa.gov/live/roulette/1" },
                new LiveCasinoTable { Id = "live-baccarat-1", GameType = "Baccarat", DealerName = "Maria", Language = "Spanish", CurrentPlayers = 2, MaxPlayers = 14, MinBet = 10, MaxBet = 5000, IsAvailable = true, StreamUrl = "https://stream.casino.ierahkwa.gov/live/baccarat/1" },
                new LiveCasinoTable { Id = "live-gameshow-1", GameType = "Game Show", DealerName = "Alex", Language = "English", CurrentPlayers = 12, MaxPlayers = 20, MinBet = 1, MaxBet = 500, IsAvailable = true, StreamUrl = "https://stream.casino.ierahkwa.gov/live/gameshow/1" }
            });
        }

        private void InitializeVideoPoker()
        {
            _videoPoker.AddRange(new[]
            {
                new VideoPokerGame { Id = "jacks", Name = "Jacks or Better", Category = "Video Poker", Icon = "👑", MinBet = 0.25m, MaxBet = 25, RTP = 99.5m, Variant = "Jacks or Better", HandSize = 5 },
                new VideoPokerGame { Id = "deuces", Name = "Deuces Wild", Category = "Video Poker", Icon = "🃏", MinBet = 0.25m, MaxBet = 25, RTP = 98.9m, Variant = "Deuces Wild", HandSize = 5 },
                new VideoPokerGame { Id = "joker", Name = "Joker Poker", Category = "Video Poker", Icon = "🃟", MinBet = 0.25m, MaxBet = 25, RTP = 98.7m, Variant = "Joker Poker", HandSize = 5 },
                new VideoPokerGame { Id = "aces", Name = "Aces & Faces", Category = "Video Poker", Icon = "🂡", MinBet = 0.25m, MaxBet = 25, RTP = 99.2m, Variant = "Aces & Faces", HandSize = 5 }
            });
        }

        // Games
        public Task<List<CasinoGame>> GetGamesAsync(string? category = null)
        {
            var games = _games.AsQueryable();
            if (!string.IsNullOrEmpty(category))
                games = games.Where(g => g.Category == category);
            return Task.FromResult(games.ToList());
        }

        public Task<CasinoGame?> GetGameByIdAsync(string gameId) =>
            Task.FromResult(_games.FirstOrDefault(g => g.Id == gameId));

        public Task<List<CasinoGame>> GetPopularGamesAsync() =>
            Task.FromResult(_games.Where(g => g.IsPopular).ToList());

        public Task<List<CasinoGame>> GetLiveCasinoGamesAsync() =>
            Task.FromResult(_games.Where(g => g.IsLive).ToList());

        // Slots
        public Task<List<SlotGame>> GetSlotsAsync() => Task.FromResult(_slots);

        public Task<SlotGame?> GetSlotByIdAsync(string slotId) =>
            Task.FromResult(_slots.FirstOrDefault(s => s.Id == slotId));

        public Task<List<SlotGame>> GetProgressiveSlotsAsync() =>
            Task.FromResult(_slots.Where(s => s.IsProgressive).ToList());

        // Table Games
        public Task<List<TableGame>> GetTableGamesAsync() => Task.FromResult(_tableGames);

        public Task<TableGame?> GetTableGameByIdAsync(string gameId) =>
            Task.FromResult(_tableGames.FirstOrDefault(g => g.Id == gameId));

        // Live Casino
        public Task<List<LiveCasinoTable>> GetLiveTablesAsync() =>
            Task.FromResult(_liveTables.Where(t => t.IsAvailable).ToList());

        public Task<LiveCasinoTable?> GetLiveTableByIdAsync(string tableId) =>
            Task.FromResult(_liveTables.FirstOrDefault(t => t.Id == tableId));

        // Video Poker
        public Task<List<VideoPokerGame>> GetVideoPokerGamesAsync() => Task.FromResult(_videoPoker);

        public Task<VideoPokerGame?> GetVideoPokerByIdAsync(string gameId) =>
            Task.FromResult(_videoPoker.FirstOrDefault(g => g.Id == gameId));

        // Game Sessions
        public Task<GameSession> StartGameSessionAsync(StartGameRequest request)
        {
            var wallet = GetOrCreateWallet(request.UserId);
            if (wallet.Balance < request.InitialBet)
                throw new InvalidOperationException("Insufficient balance");

            var game = _games.FirstOrDefault(g => g.Id == request.GameId) ??
                      _slots.FirstOrDefault(s => s.Id == request.GameId) as CasinoGame ??
                      _tableGames.FirstOrDefault(t => t.Id == request.GameId) as CasinoGame ??
                      _videoPoker.FirstOrDefault(v => v.Id == request.GameId) as CasinoGame;

            if (game == null)
                throw new ArgumentException("Game not found");

            var session = new GameSession
            {
                Id = $"session-{Guid.NewGuid()}",
                UserId = request.UserId,
                GameId = request.GameId,
                GameName = game.Name,
                Balance = wallet.Balance,
                StartedAt = DateTime.UtcNow,
                Status = CasinoSessionStatus.Active
            };

            _sessions[session.Id] = session;
            return Task.FromResult(session);
        }

        public Task<GameSession?> GetSessionByIdAsync(string sessionId) =>
            Task.FromResult(_sessions.TryGetValue(sessionId, out var session) ? session : null);

        public Task<GameResult> PlayGameAsync(string sessionId, GameAction action)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                throw new ArgumentException("Session not found");

            var wallet = GetOrCreateWallet(session.UserId);
            var random = new Random();
            var won = random.Next(0, 2) == 1;
            var winAmount = won ? (action.Amount ?? 0) * 2 : 0;

            if (action.Amount.HasValue)
            {
                wallet.Balance -= action.Amount.Value;
                wallet.TotalWagered += action.Amount.Value;
            }

            if (won)
            {
                wallet.Balance += winAmount;
                wallet.TotalWon += winAmount;
            }

            var result = new GameResult
            {
                Won = won,
                WinAmount = winAmount,
                NewBalance = wallet.Balance,
                Message = won ? "Congratulations! You won!" : "Better luck next time!"
            };

            return Task.FromResult(result);
        }

        public Task<bool> EndSessionAsync(string sessionId)
        {
            if (!_sessions.TryGetValue(sessionId, out var session))
                return Task.FromResult(false);

            session.Status = CasinoSessionStatus.Ended;
            session.EndedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }

        // Wallet & Transactions
        public Task<CasinoWallet> GetWalletAsync(Guid userId) =>
            Task.FromResult(GetOrCreateWallet(userId));

        private CasinoWallet GetOrCreateWallet(Guid userId)
        {
            if (!_wallets.TryGetValue(userId, out var wallet))
            {
                wallet = new CasinoWallet
                {
                    UserId = userId,
                    Balance = 1000,
                    TotalDeposited = 0,
                    TotalWithdrawn = 0,
                    TotalWon = 0,
                    TotalWagered = 0,
                    Currency = "USD"
                };
                _wallets[userId] = wallet;
            }
            return wallet;
        }

        public Task<CasinoTransaction> DepositAsync(Guid userId, decimal amount)
        {
            var wallet = GetOrCreateWallet(userId);
            wallet.Balance += amount;
            wallet.TotalDeposited += amount;

            var transaction = new CasinoTransaction
            {
                Id = $"tx-{Guid.NewGuid()}",
                UserId = userId,
                Type = CasinoTransactionType.Deposit,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                Status = CasinoTransactionStatus.Completed,
                Description = "Deposit to casino wallet"
            };

            _transactions.Add(transaction);
            return Task.FromResult(transaction);
        }

        public Task<CasinoTransaction> WithdrawAsync(Guid userId, decimal amount)
        {
            var wallet = GetOrCreateWallet(userId);
            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient balance");

            wallet.Balance -= amount;
            wallet.TotalWithdrawn += amount;

            var transaction = new CasinoTransaction
            {
                Id = $"tx-{Guid.NewGuid()}",
                UserId = userId,
                Type = CasinoTransactionType.Withdrawal,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                Status = CasinoTransactionStatus.Completed,
                Description = "Withdrawal from casino wallet"
            };

            _transactions.Add(transaction);
            return Task.FromResult(transaction);
        }

        public Task<List<CasinoTransaction>> GetTransactionsAsync(Guid userId, int limit = 50) =>
            Task.FromResult(_transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).Take(limit).ToList());

        // VIP & Rewards
        public Task<VIPStatus> GetVIPStatusAsync(Guid userId)
        {
            if (!_vipStatuses.TryGetValue(userId, out var vip))
            {
                var wallet = GetOrCreateWallet(userId);
                var level = wallet.TotalWagered switch
                {
                    >= 100000 => VIPLevel.Platinum,
                    >= 50000 => VIPLevel.Diamond,
                    >= 25000 => VIPLevel.Gold,
                    >= 10000 => VIPLevel.Silver,
                    _ => VIPLevel.Bronze
                };

                vip = new VIPStatus
                {
                    UserId = userId,
                    Level = level,
                    Points = wallet.TotalWagered / 10,
                    PointsToNextLevel = level switch
                    {
                        VIPLevel.Bronze => 10000 - wallet.TotalWagered,
                        VIPLevel.Silver => 25000 - wallet.TotalWagered,
                        VIPLevel.Gold => 50000 - wallet.TotalWagered,
                        VIPLevel.Diamond => 100000 - wallet.TotalWagered,
                        _ => 0
                    },
                    CashbackPercent = level switch
                    {
                        VIPLevel.Bronze => 5,
                        VIPLevel.Silver => 10,
                        VIPLevel.Gold => 15,
                        VIPLevel.Diamond => 20,
                        VIPLevel.Platinum => 25,
                        _ => 0
                    },
                    Benefits = GetVIPBenefits(level)
                };
                _vipStatuses[userId] = vip;
            }
            return Task.FromResult(vip);
        }

        private List<string> GetVIPBenefits(VIPLevel level) => level switch
        {
            VIPLevel.Bronze => new List<string> { "5% Weekly Cashback", "Priority Support" },
            VIPLevel.Silver => new List<string> { "10% Weekly Cashback", "Exclusive Bonuses", "Priority Support" },
            VIPLevel.Gold => new List<string> { "15% Weekly Cashback", "VIP Events", "Personal Account Manager", "Exclusive Bonuses" },
            VIPLevel.Diamond => new List<string> { "20% Weekly Cashback", "VIP Events", "Personal Host", "Luxury Gifts", "Exclusive Bonuses" },
            VIPLevel.Platinum => new List<string> { "25% Weekly Cashback", "VIP Events", "Personal Host", "Luxury Gifts", "Private Tables", "Exclusive Bonuses" },
            _ => new List<string>()
        };

        public Task<List<Reward>> GetRewardsAsync(Guid userId)
        {
            if (!_rewards.TryGetValue(userId, out var rewards))
            {
                rewards = new List<Reward>();
                _rewards[userId] = rewards;
            }
            return Task.FromResult(rewards);
        }

        public Task<Reward?> ClaimRewardAsync(Guid userId, string rewardId)
        {
            if (!_rewards.TryGetValue(userId, out var rewards))
                return Task.FromResult<Reward?>(null);

            var reward = rewards.FirstOrDefault(r => r.Id == rewardId && !r.IsClaimed);
            if (reward != null)
            {
                reward.IsClaimed = true;
                reward.ClaimedAt = DateTime.UtcNow;
            }
            return Task.FromResult(reward);
        }

        // Statistics
        public Task<CasinoStatistics> GetStatisticsAsync(Guid userId)
        {
            var wallet = GetOrCreateWallet(userId);
            var userTransactions = _transactions.Where(t => t.UserId == userId).ToList();
            var userSessions = _sessions.Values.Where(s => s.UserId == userId).ToList();

            var stats = new CasinoStatistics
            {
                UserId = userId,
                TotalGamesPlayed = userSessions.Count,
                TotalWagered = wallet.TotalWagered,
                TotalWon = wallet.TotalWon,
                NetProfit = wallet.TotalWon - wallet.TotalWagered,
                WinRate = wallet.TotalWagered > 0 ? (wallet.TotalWon / wallet.TotalWagered) * 100 : 0,
                FavoriteGame = userSessions.GroupBy(s => s.GameName).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "N/A",
                LongestWinStreak = 0 // Would need to track this
            };

            return Task.FromResult(stats);
        }
    }
}
