namespace IERAHKWA.Platform.Models;

// ============= CASINO MODELS =============

public class CasinoUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = "";
    public decimal Balance { get; set; } = 1000m; // Starting balance
    public decimal TotalWon { get; set; } = 0m;
    public decimal TotalLost { get; set; } = 0m;
    public int GamesPlayed { get; set; } = 0;
    public string VipLevel { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum, Diamond
    public int LoyaltyPoints { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
}

// SLOTS
public class SlotSpinRequest
{
    public string UserId { get; set; } = "";
    public decimal BetAmount { get; set; } = 10m;
    public string Machine { get; set; } = "classic"; // classic, fruits, egypt, space, crypto
}

public class SlotSpinResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string[] Reels { get; set; } = new string[5];
    public bool IsWin { get; set; }
    public decimal WinAmount { get; set; }
    public decimal BetAmount { get; set; }
    public string? WinType { get; set; } // null, "pair", "triple", "jackpot", "mega_jackpot"
    public decimal NewBalance { get; set; }
    public int FreeSpinsWon { get; set; }
    public decimal Multiplier { get; set; } = 1m;
    public DateTime SpinAt { get; set; } = DateTime.UtcNow;
}

// ROULETTE
public class RouletteBetRequest
{
    public string UserId { get; set; } = "";
    public List<RouletteBet> Bets { get; set; } = new();
}

public class RouletteBet
{
    public string Type { get; set; } = "number"; // number, red, black, odd, even, low, high, dozen, column
    public string Value { get; set; } = "0"; // number or bet type
    public decimal Amount { get; set; } = 10m;
}

public class RouletteResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int WinningNumber { get; set; }
    public string Color { get; set; } = "green"; // red, black, green
    public List<RouletteBetResult> BetResults { get; set; } = new();
    public decimal TotalWin { get; set; }
    public decimal TotalBet { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime SpinAt { get; set; } = DateTime.UtcNow;
}

public class RouletteBetResult
{
    public RouletteBet Bet { get; set; } = new();
    public bool Won { get; set; }
    public decimal Payout { get; set; }
}

// BLACKJACK
public class BlackjackRequest
{
    public string UserId { get; set; } = "";
    public decimal BetAmount { get; set; } = 25m;
}

public class BlackjackGame
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public List<Card> PlayerHand { get; set; } = new();
    public List<Card> DealerHand { get; set; } = new();
    public int PlayerScore { get; set; }
    public int DealerScore { get; set; }
    public decimal BetAmount { get; set; }
    public string Status { get; set; } = "playing"; // playing, player_bust, dealer_bust, player_win, dealer_win, push, blackjack
    public decimal WinAmount { get; set; }
    public decimal NewBalance { get; set; }
    public bool CanHit { get; set; } = true;
    public bool CanStand { get; set; } = true;
    public bool CanDouble { get; set; } = true;
    public bool CanSplit { get; set; } = false;
}

public class Card
{
    public string Suit { get; set; } = ""; // hearts, diamonds, clubs, spades
    public string Rank { get; set; } = ""; // 2-10, J, Q, K, A
    public int Value { get; set; }
    public string Display => $"{Rank}{SuitEmoji}";
    public string SuitEmoji => Suit switch
    {
        "hearts" => "♥️",
        "diamonds" => "♦️",
        "clubs" => "♣️",
        "spades" => "♠️",
        _ => ""
    };
}

public class BlackjackAction
{
    public string GameId { get; set; } = "";
    public string Action { get; set; } = "hit"; // hit, stand, double, split
}

// POKER
public class PokerTable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public int MaxPlayers { get; set; } = 6;
    public decimal MinBet { get; set; } = 10m;
    public decimal MaxBet { get; set; } = 1000m;
    public List<PokerPlayer> Players { get; set; } = new();
    public List<Card> CommunityCards { get; set; } = new();
    public decimal Pot { get; set; }
    public string Phase { get; set; } = "waiting"; // waiting, preflop, flop, turn, river, showdown
    public int CurrentTurn { get; set; }
}

public class PokerPlayer
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public List<Card> Hand { get; set; } = new();
    public decimal Chips { get; set; }
    public decimal CurrentBet { get; set; }
    public bool IsFolded { get; set; }
    public bool IsAllIn { get; set; }
    public int Seat { get; set; }
}

// CRASH GAME
public class CrashBetRequest
{
    public string UserId { get; set; } = "";
    public decimal BetAmount { get; set; } = 10m;
    public decimal? AutoCashout { get; set; } // Optional auto cashout multiplier
}

public class CrashGame
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal CurrentMultiplier { get; set; } = 1.00m;
    public decimal CrashPoint { get; set; } // Hidden until crash
    public string Status { get; set; } = "betting"; // betting, running, crashed
    public List<CrashBet> Bets { get; set; } = new();
    public DateTime StartTime { get; set; }
}

public class CrashBet
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public decimal BetAmount { get; set; }
    public decimal? CashedOutAt { get; set; }
    public decimal? WinAmount { get; set; }
}

// DICE
public class DiceRollRequest
{
    public string UserId { get; set; } = "";
    public decimal BetAmount { get; set; } = 10m;
    public string Prediction { get; set; } = "over"; // over, under, exact
    public decimal Target { get; set; } = 50m; // 0-100
}

public class DiceRollResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal RolledNumber { get; set; }
    public decimal Target { get; set; }
    public string Prediction { get; set; } = "";
    public bool Won { get; set; }
    public decimal Multiplier { get; set; }
    public decimal BetAmount { get; set; }
    public decimal WinAmount { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime RolledAt { get; set; } = DateTime.UtcNow;
}

// LOTTERY
public class LotteryTicket
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public int[] Numbers { get; set; } = new int[6];
    public string DrawId { get; set; } = "";
    public decimal Price { get; set; } = 5m;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
}

public class LotteryDraw
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int[] WinningNumbers { get; set; } = new int[6];
    public decimal JackpotAmount { get; set; }
    public DateTime DrawTime { get; set; }
    public bool IsComplete { get; set; }
    public List<LotteryWinner> Winners { get; set; } = new();
}

public class LotteryWinner
{
    public string UserId { get; set; } = "";
    public string TicketId { get; set; } = "";
    public int MatchedNumbers { get; set; }
    public decimal PrizeAmount { get; set; }
}

// SPORTS BETTING
public class SportEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Sport { get; set; } = ""; // football, basketball, tennis, esports, etc.
    public string League { get; set; } = "";
    public string HomeTeam { get; set; } = "";
    public string AwayTeam { get; set; } = "";
    public decimal HomeOdds { get; set; }
    public decimal DrawOdds { get; set; }
    public decimal AwayOdds { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = "upcoming"; // upcoming, live, finished
    public string? Score { get; set; }
}

public class SportBetRequest
{
    public string UserId { get; set; } = "";
    public string EventId { get; set; } = "";
    public string BetType { get; set; } = "home"; // home, draw, away, over, under
    public decimal Amount { get; set; } = 10m;
    public decimal Odds { get; set; }
}

public class SportBet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public SportEvent Event { get; set; } = new();
    public string BetType { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal Odds { get; set; }
    public string Status { get; set; } = "pending"; // pending, won, lost, cancelled
    public decimal? WinAmount { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
}

// TRANSACTIONS
public class CasinoTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Type { get; set; } = ""; // deposit, withdraw, bet, win, bonus
    public string Game { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// LEADERBOARD
public class CasinoLeaderboardEntry
{
    public int Rank { get; set; }
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public decimal TotalWon { get; set; }
    public decimal BiggestWin { get; set; }
    public int GamesPlayed { get; set; }
    public string VipLevel { get; set; } = "";
}

// PROMOTIONS
public class CasinoPromotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = ""; // welcome, reload, cashback, freeSpins
    public decimal BonusAmount { get; set; }
    public int FreeSpins { get; set; }
    public decimal WageringRequirement { get; set; } = 30m;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
