using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface ICasinoService
{
    // User
    Task<CasinoUser> GetUserAsync(string id);
    Task<CasinoUser> CreateUserAsync(string username);
    Task<decimal> GetBalanceAsync(string userId);
    Task<bool> DepositAsync(string userId, decimal amount);
    Task<bool> WithdrawAsync(string userId, decimal amount);
    
    // Slots
    Task<SlotSpinResult> SpinSlotsAsync(SlotSpinRequest request);
    
    // Roulette
    Task<RouletteResult> SpinRouletteAsync(RouletteBetRequest request);
    
    // Blackjack
    Task<BlackjackGame> StartBlackjackAsync(BlackjackRequest request);
    Task<BlackjackGame> BlackjackActionAsync(BlackjackAction action);
    
    // Crash
    Task<CrashGame> GetCurrentCrashGameAsync();
    Task<CrashBet> PlaceCrashBetAsync(CrashBetRequest request);
    Task<CrashBet?> CashoutCrashAsync(string gameId, string oderId);
    
    // Dice
    Task<DiceRollResult> RollDiceAsync(DiceRollRequest request);
    
    // Lottery
    Task<LotteryTicket> BuyLotteryTicketAsync(string userId, int[]? numbers);
    Task<LotteryDraw> GetCurrentLotteryAsync();
    
    // Sports
    Task<List<SportEvent>> GetSportEventsAsync(string? sport);
    Task<SportBet> PlaceSportBetAsync(SportBetRequest request);
    
    // Stats
    Task<List<CasinoLeaderboardEntry>> GetLeaderboardAsync(string period);
    Task<List<CasinoTransaction>> GetTransactionsAsync(string userId, int limit);
    Task<List<CasinoPromotion>> GetPromotionsAsync();
}

public class CasinoService : ICasinoService
{
    private readonly ILogger<CasinoService> _logger;
    private readonly Random _random = new();
    
    // In-memory storage
    private readonly Dictionary<string, CasinoUser> _users = new();
    private readonly Dictionary<string, BlackjackGame> _blackjackGames = new();
    private readonly List<CasinoTransaction> _transactions = new();
    private CrashGame _currentCrashGame = new();
    private LotteryDraw _currentLottery = new();
    
    // Slot symbols
    private readonly string[][] _slotSymbols = new[]
    {
        new[] { "🍒", "🍋", "🍊", "🍇", "🔔", "⭐", "7️⃣", "💎" }, // Classic
        new[] { "🍒", "🍋", "🍊", "🍇", "🍉", "🍓", "🍌", "🥝" }, // Fruits
        new[] { "👁️", "🏛️", "🐍", "⚱️", "🔱", "📜", "💀", "👑" }, // Egypt
        new[] { "🚀", "🌟", "🛸", "👽", "🌙", "⭐", "🪐", "☄️" }, // Space
        new[] { "₿", "Ξ", "💰", "🪙", "💎", "📈", "🚀", "🌙" }  // Crypto
    };

    public CasinoService(ILogger<CasinoService> logger)
    {
        _logger = logger;
        InitializeCrashGame();
        InitializeLottery();
        InitializeSampleUsers();
    }

    private void InitializeSampleUsers()
    {
        var users = new[] { "whale_king", "lucky7", "crypto_queen", "high_roller", "diamond_vip" };
        foreach (var username in users)
        {
            var user = new CasinoUser
            {
                Username = username,
                Balance = _random.Next(1000, 100000),
                TotalWon = _random.Next(10000, 500000),
                GamesPlayed = _random.Next(100, 5000),
                VipLevel = new[] { "Bronze", "Silver", "Gold", "Platinum", "Diamond" }[_random.Next(5)]
            };
            _users[user.Id] = user;
        }
    }

    private void InitializeCrashGame()
    {
        _currentCrashGame = new CrashGame
        {
            CrashPoint = GenerateCrashPoint(),
            Status = "betting"
        };
    }

    private void InitializeLottery()
    {
        _currentLottery = new LotteryDraw
        {
            JackpotAmount = 1000000,
            DrawTime = DateTime.UtcNow.AddHours(24),
            WinningNumbers = Enumerable.Range(1, 49).OrderBy(_ => _random.Next()).Take(6).ToArray()
        };
    }

    private decimal GenerateCrashPoint()
    {
        // House edge ~3%
        var r = _random.NextDouble();
        return (decimal)Math.Max(1.0, (1.0 / (1.0 - r * 0.97)) * 0.99);
    }

    // ============= USER METHODS =============

    public async Task<CasinoUser> GetUserAsync(string id)
    {
        await Task.Delay(10);
        if (_users.TryGetValue(id, out var user))
            return user;
        
        // Create new user with safe substring
        var suffix = id.Length >= 8 ? id[..8] : id;
        return await CreateUserAsync($"player_{suffix}");
    }

    public async Task<CasinoUser> CreateUserAsync(string username)
    {
        await Task.Delay(10);
        var user = new CasinoUser { Username = username ?? "player", Balance = 1000 };
        _users[user.Id] = user;
        _logger.LogInformation("Created casino user: {Username}", user.Username);
        return user;
    }

    public async Task<decimal> GetBalanceAsync(string userId)
    {
        var user = await GetUserAsync(userId);
        return user.Balance;
    }

    public async Task<bool> DepositAsync(string userId, decimal amount)
    {
        var user = await GetUserAsync(userId);
        user.Balance += amount;
        
        _transactions.Add(new CasinoTransaction
        {
            UserId = userId,
            Type = "deposit",
            Amount = amount,
            BalanceAfter = user.Balance
        });
        
        return true;
    }

    public async Task<bool> WithdrawAsync(string userId, decimal amount)
    {
        var user = await GetUserAsync(userId);
        if (user.Balance < amount) return false;
        
        user.Balance -= amount;
        
        _transactions.Add(new CasinoTransaction
        {
            UserId = userId,
            Type = "withdraw",
            Amount = -amount,
            BalanceAfter = user.Balance
        });
        
        return true;
    }

    // ============= SLOTS =============

    public async Task<SlotSpinResult> SpinSlotsAsync(SlotSpinRequest request)
    {
        await Task.Delay(100);
        
        var user = await GetUserAsync(request.UserId);
        if (user.Balance < request.BetAmount)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= request.BetAmount;
        user.GamesPlayed++;

        var machineIndex = request.Machine switch
        {
            "fruits" => 1,
            "egypt" => 2,
            "space" => 3,
            "crypto" => 4,
            _ => 0
        };

        var symbols = _slotSymbols[machineIndex];
        var reels = new string[5];
        
        for (int i = 0; i < 5; i++)
        {
            reels[i] = symbols[_random.Next(symbols.Length)];
        }

        // Calculate win
        var (isWin, winType, multiplier) = CalculateSlotWin(reels);
        var winAmount = isWin ? request.BetAmount * multiplier : 0;
        var freeSpins = winType == "jackpot" ? 10 : 0;

        user.Balance += winAmount;
        if (isWin) user.TotalWon += winAmount;
        else user.TotalLost += request.BetAmount;

        // Update VIP
        user.LoyaltyPoints += (int)(request.BetAmount / 10);
        UpdateVipLevel(user);

        _logger.LogInformation("Slot spin: {Win} - {WinType} - {Amount}", isWin, winType, winAmount);

        return new SlotSpinResult
        {
            Reels = reels,
            IsWin = isWin,
            WinType = winType,
            WinAmount = winAmount,
            BetAmount = request.BetAmount,
            NewBalance = user.Balance,
            FreeSpinsWon = freeSpins,
            Multiplier = multiplier
        };
    }

    private (bool isWin, string? winType, decimal multiplier) CalculateSlotWin(string[] reels)
    {
        // Count matches
        var groups = reels.GroupBy(r => r).OrderByDescending(g => g.Count()).ToList();
        var maxCount = groups.First().Count();
        var symbol = groups.First().Key;

        return maxCount switch
        {
            5 => symbol == "💎" || symbol == "7️⃣" ? (true, "mega_jackpot", 1000m) : (true, "jackpot", 100m),
            4 => (true, "four_of_a_kind", 25m),
            3 => (true, "triple", 5m),
            2 when groups.Count(g => g.Count() == 2) >= 2 => (true, "two_pair", 2m),
            _ => (false, null, 0m)
        };
    }

    private void UpdateVipLevel(CasinoUser user)
    {
        user.VipLevel = user.LoyaltyPoints switch
        {
            >= 100000 => "Diamond",
            >= 50000 => "Platinum",
            >= 10000 => "Gold",
            >= 1000 => "Silver",
            _ => "Bronze"
        };
    }

    // ============= ROULETTE =============

    public async Task<RouletteResult> SpinRouletteAsync(RouletteBetRequest request)
    {
        await Task.Delay(200);
        
        var user = await GetUserAsync(request.UserId);
        var totalBet = request.Bets.Sum(b => b.Amount);
        
        if (user.Balance < totalBet)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= totalBet;
        user.GamesPlayed++;

        // Spin the wheel
        var winningNumber = _random.Next(0, 37); // 0-36
        var color = GetRouletteColor(winningNumber);

        var betResults = new List<RouletteBetResult>();
        decimal totalWin = 0;

        foreach (var bet in request.Bets)
        {
            var won = CheckRouletteBet(bet, winningNumber, color);
            var payout = won ? bet.Amount * GetRoulettePayoutMultiplier(bet.Type) : 0;
            
            betResults.Add(new RouletteBetResult
            {
                Bet = bet,
                Won = won,
                Payout = payout
            });
            
            totalWin += payout;
        }

        user.Balance += totalWin;
        if (totalWin > 0) user.TotalWon += totalWin;
        else user.TotalLost += totalBet;

        _logger.LogInformation("Roulette: {Number} {Color} - Win: {Win}", winningNumber, color, totalWin);

        return new RouletteResult
        {
            WinningNumber = winningNumber,
            Color = color,
            BetResults = betResults,
            TotalWin = totalWin,
            TotalBet = totalBet,
            NewBalance = user.Balance
        };
    }

    private string GetRouletteColor(int number)
    {
        if (number == 0) return "green";
        var reds = new[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
        return reds.Contains(number) ? "red" : "black";
    }

    private bool CheckRouletteBet(RouletteBet bet, int number, string color)
    {
        return bet.Type switch
        {
            "number" => int.TryParse(bet.Value, out var n) && n == number,
            "red" => color == "red",
            "black" => color == "black",
            "odd" => number > 0 && number % 2 == 1,
            "even" => number > 0 && number % 2 == 0,
            "low" => number >= 1 && number <= 18,
            "high" => number >= 19 && number <= 36,
            "dozen1" => number >= 1 && number <= 12,
            "dozen2" => number >= 13 && number <= 24,
            "dozen3" => number >= 25 && number <= 36,
            _ => false
        };
    }

    private decimal GetRoulettePayoutMultiplier(string betType)
    {
        return betType switch
        {
            "number" => 36m,
            "red" or "black" or "odd" or "even" or "low" or "high" => 2m,
            "dozen1" or "dozen2" or "dozen3" or "column" => 3m,
            _ => 2m
        };
    }

    // ============= BLACKJACK =============

    public async Task<BlackjackGame> StartBlackjackAsync(BlackjackRequest request)
    {
        await Task.Delay(100);
        
        var user = await GetUserAsync(request.UserId);
        if (user.Balance < request.BetAmount)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= request.BetAmount;
        user.GamesPlayed++;

        var game = new BlackjackGame
        {
            UserId = request.UserId,
            BetAmount = request.BetAmount
        };

        // Deal cards
        game.PlayerHand.Add(DrawCard());
        game.DealerHand.Add(DrawCard());
        game.PlayerHand.Add(DrawCard());
        game.DealerHand.Add(DrawCard()); // Face down

        game.PlayerScore = CalculateHandScore(game.PlayerHand);
        game.DealerScore = game.DealerHand[0].Value; // Only show first card

        // Check for blackjack
        if (game.PlayerScore == 21)
        {
            game.Status = "blackjack";
            game.WinAmount = request.BetAmount * 2.5m;
            user.Balance += game.WinAmount;
            user.TotalWon += game.WinAmount - request.BetAmount;
        }

        game.NewBalance = user.Balance;
        game.CanDouble = game.PlayerHand.Count == 2 && user.Balance >= request.BetAmount;
        game.CanSplit = game.PlayerHand.Count == 2 && 
                        game.PlayerHand[0].Value == game.PlayerHand[1].Value &&
                        user.Balance >= request.BetAmount;

        _blackjackGames[game.Id] = game;

        _logger.LogInformation("Blackjack started: Player {Score}", game.PlayerScore);

        return game;
    }

    public async Task<BlackjackGame> BlackjackActionAsync(BlackjackAction action)
    {
        await Task.Delay(50);

        if (!_blackjackGames.TryGetValue(action.GameId, out var game))
            throw new KeyNotFoundException("Game not found");

        var user = await GetUserAsync(game.UserId);

        switch (action.Action)
        {
            case "hit":
                game.PlayerHand.Add(DrawCard());
                game.PlayerScore = CalculateHandScore(game.PlayerHand);
                
                if (game.PlayerScore > 21)
                {
                    game.Status = "player_bust";
                    user.TotalLost += game.BetAmount;
                }
                else if (game.PlayerScore == 21)
                {
                    game.CanHit = false;
                }
                game.CanDouble = false;
                game.CanSplit = false;
                break;

            case "stand":
                // Dealer plays
                game.DealerScore = CalculateHandScore(game.DealerHand);
                
                while (game.DealerScore < 17)
                {
                    game.DealerHand.Add(DrawCard());
                    game.DealerScore = CalculateHandScore(game.DealerHand);
                }

                if (game.DealerScore > 21)
                {
                    game.Status = "dealer_bust";
                    game.WinAmount = game.BetAmount * 2;
                }
                else if (game.PlayerScore > game.DealerScore)
                {
                    game.Status = "player_win";
                    game.WinAmount = game.BetAmount * 2;
                }
                else if (game.PlayerScore < game.DealerScore)
                {
                    game.Status = "dealer_win";
                }
                else
                {
                    game.Status = "push";
                    game.WinAmount = game.BetAmount; // Return bet
                }

                user.Balance += game.WinAmount;
                if (game.WinAmount > game.BetAmount) 
                    user.TotalWon += game.WinAmount - game.BetAmount;
                else if (game.WinAmount == 0)
                    user.TotalLost += game.BetAmount;

                game.CanHit = false;
                game.CanStand = false;
                break;

            case "double":
                if (user.Balance < game.BetAmount)
                    throw new InvalidOperationException("Insufficient balance");
                
                user.Balance -= game.BetAmount;
                game.BetAmount *= 2;
                game.PlayerHand.Add(DrawCard());
                game.PlayerScore = CalculateHandScore(game.PlayerHand);
                
                // Force stand after double
                return await BlackjackActionAsync(new BlackjackAction { GameId = game.Id, Action = "stand" });
        }

        game.NewBalance = user.Balance;
        
        _logger.LogInformation("Blackjack {Action}: Player {PScore} Dealer {DScore} - {Status}", 
            action.Action, game.PlayerScore, game.DealerScore, game.Status);

        return game;
    }

    private Card DrawCard()
    {
        var suits = new[] { "hearts", "diamonds", "clubs", "spades" };
        var ranks = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        
        var rank = ranks[_random.Next(ranks.Length)];
        var value = rank switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.Parse(rank)
        };

        return new Card
        {
            Suit = suits[_random.Next(suits.Length)],
            Rank = rank,
            Value = value
        };
    }

    private int CalculateHandScore(List<Card> hand)
    {
        var score = hand.Sum(c => c.Value);
        var aces = hand.Count(c => c.Rank == "A");
        
        while (score > 21 && aces > 0)
        {
            score -= 10;
            aces--;
        }
        
        return score;
    }

    // ============= CRASH =============

    public async Task<CrashGame> GetCurrentCrashGameAsync()
    {
        await Task.Delay(10);
        return _currentCrashGame;
    }

    public async Task<CrashBet> PlaceCrashBetAsync(CrashBetRequest request)
    {
        await Task.Delay(50);
        
        if (_currentCrashGame.Status != "betting")
            throw new InvalidOperationException("Betting is closed");

        var user = await GetUserAsync(request.UserId);
        if (user.Balance < request.BetAmount)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= request.BetAmount;

        var bet = new CrashBet
        {
            UserId = request.UserId,
            Username = user.Username,
            BetAmount = request.BetAmount
        };

        _currentCrashGame.Bets.Add(bet);

        return bet;
    }

    public async Task<CrashBet?> CashoutCrashAsync(string gameId, string oderId)
    {
        await Task.Delay(50);
        
        if (_currentCrashGame.Id != gameId || _currentCrashGame.Status != "running")
            return null;

        var bet = _currentCrashGame.Bets.FirstOrDefault(b => b.UserId == oderId);
        if (bet == null || bet.CashedOutAt.HasValue)
            return null;

        bet.CashedOutAt = _currentCrashGame.CurrentMultiplier;
        bet.WinAmount = bet.BetAmount * bet.CashedOutAt.Value;

        var user = await GetUserAsync(oderId);
        user.Balance += bet.WinAmount.Value;

        return bet;
    }

    // ============= DICE =============

    public async Task<DiceRollResult> RollDiceAsync(DiceRollRequest request)
    {
        await Task.Delay(100);
        
        var user = await GetUserAsync(request.UserId);
        if (user.Balance < request.BetAmount)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= request.BetAmount;
        user.GamesPlayed++;

        var rolled = (decimal)Math.Round(_random.NextDouble() * 100, 2);
        
        var won = request.Prediction switch
        {
            "over" => rolled > request.Target,
            "under" => rolled < request.Target,
            "exact" => Math.Abs(rolled - request.Target) < 1,
            _ => false
        };

        var multiplier = request.Prediction switch
        {
            "over" => 100m / (100m - request.Target),
            "under" => 100m / request.Target,
            "exact" => 99m,
            _ => 0m
        };

        var winAmount = won ? request.BetAmount * multiplier * 0.97m : 0; // 3% house edge

        user.Balance += winAmount;
        if (won) user.TotalWon += winAmount;
        else user.TotalLost += request.BetAmount;

        _logger.LogInformation("Dice: {Rolled} vs {Target} ({Prediction}) - Won: {Won}", 
            rolled, request.Target, request.Prediction, won);

        return new DiceRollResult
        {
            RolledNumber = rolled,
            Target = request.Target,
            Prediction = request.Prediction,
            Won = won,
            Multiplier = Math.Round(multiplier, 2),
            BetAmount = request.BetAmount,
            WinAmount = Math.Round(winAmount, 2),
            NewBalance = user.Balance
        };
    }

    // ============= LOTTERY =============

    public async Task<LotteryTicket> BuyLotteryTicketAsync(string userId, int[]? numbers)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(userId);
        if (user.Balance < 5)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= 5;

        numbers ??= Enumerable.Range(1, 49).OrderBy(_ => _random.Next()).Take(6).ToArray();

        var ticket = new LotteryTicket
        {
            UserId = userId,
            Numbers = numbers,
            DrawId = _currentLottery.Id
        };

        _currentLottery.JackpotAmount += 3; // Part of ticket goes to jackpot

        return ticket;
    }

    public async Task<LotteryDraw> GetCurrentLotteryAsync()
    {
        await Task.Delay(10);
        return _currentLottery;
    }

    // ============= SPORTS =============

    public async Task<List<SportEvent>> GetSportEventsAsync(string? sport)
    {
        await Task.Delay(50);
        
        var events = new List<SportEvent>
        {
            new() { Sport = "football", League = "Premier League", HomeTeam = "Man City", AwayTeam = "Liverpool", HomeOdds = 1.85m, DrawOdds = 3.40m, AwayOdds = 4.20m, StartTime = DateTime.UtcNow.AddHours(2) },
            new() { Sport = "football", League = "La Liga", HomeTeam = "Real Madrid", AwayTeam = "Barcelona", HomeOdds = 2.10m, DrawOdds = 3.30m, AwayOdds = 3.50m, StartTime = DateTime.UtcNow.AddHours(5) },
            new() { Sport = "basketball", League = "NBA", HomeTeam = "Lakers", AwayTeam = "Warriors", HomeOdds = 1.95m, DrawOdds = 0, AwayOdds = 1.90m, StartTime = DateTime.UtcNow.AddHours(8) },
            new() { Sport = "tennis", League = "ATP Finals", HomeTeam = "Djokovic", AwayTeam = "Alcaraz", HomeOdds = 1.70m, DrawOdds = 0, AwayOdds = 2.20m, StartTime = DateTime.UtcNow.AddHours(3) },
            new() { Sport = "esports", League = "League of Legends Worlds", HomeTeam = "T1", AwayTeam = "Gen.G", HomeOdds = 1.60m, DrawOdds = 0, AwayOdds = 2.40m, StartTime = DateTime.UtcNow.AddHours(12) },
            new() { Sport = "football", League = "Champions League", HomeTeam = "Bayern", AwayTeam = "PSG", HomeOdds = 1.75m, DrawOdds = 3.80m, AwayOdds = 4.50m, StartTime = DateTime.UtcNow.AddDays(1), Status = "upcoming" }
        };

        if (!string.IsNullOrEmpty(sport))
            events = events.Where(e => e.Sport == sport).ToList();

        return events;
    }

    public async Task<SportBet> PlaceSportBetAsync(SportBetRequest request)
    {
        await Task.Delay(50);
        
        var user = await GetUserAsync(request.UserId);
        if (user.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        user.Balance -= request.Amount;

        var events = await GetSportEventsAsync(null);
        var ev = events.FirstOrDefault(e => e.Id == request.EventId) ?? events.First();

        var bet = new SportBet
        {
            UserId = request.UserId,
            Event = ev,
            BetType = request.BetType,
            Amount = request.Amount,
            Odds = request.Odds
        };

        _logger.LogInformation("Sport bet placed: {Team} @ {Odds} - {Amount}", request.BetType, request.Odds, request.Amount);

        return bet;
    }

    // ============= STATS =============

    public async Task<List<CasinoLeaderboardEntry>> GetLeaderboardAsync(string period)
    {
        await Task.Delay(50);
        
        return _users.Values
            .OrderByDescending(u => u.TotalWon)
            .Take(20)
            .Select((u, i) => new CasinoLeaderboardEntry
            {
                Rank = i + 1,
                UserId = u.Id,
                Username = u.Username,
                TotalWon = u.TotalWon,
                BiggestWin = u.TotalWon / Math.Max(1, u.GamesPlayed) * 10,
                GamesPlayed = u.GamesPlayed,
                VipLevel = u.VipLevel
            })
            .ToList();
    }

    public async Task<List<CasinoTransaction>> GetTransactionsAsync(string userId, int limit)
    {
        await Task.Delay(50);
        return _transactions.Where(t => t.UserId == userId).Take(limit).ToList();
    }

    public async Task<List<CasinoPromotion>> GetPromotionsAsync()
    {
        await Task.Delay(50);
        
        return new List<CasinoPromotion>
        {
            new() { Name = "Welcome Bonus", Description = "100% up to $500 + 200 Free Spins", Type = "welcome", BonusAmount = 500, FreeSpins = 200 },
            new() { Name = "Daily Reload", Description = "50% reload bonus up to $200", Type = "reload", BonusAmount = 200 },
            new() { Name = "Cashback Weekend", Description = "10% cashback on losses", Type = "cashback", BonusAmount = 0.10m },
            new() { Name = "VIP Free Spins", Description = "50 free spins every Monday for Gold+ members", Type = "freeSpins", FreeSpins = 50 }
        };
    }
}
