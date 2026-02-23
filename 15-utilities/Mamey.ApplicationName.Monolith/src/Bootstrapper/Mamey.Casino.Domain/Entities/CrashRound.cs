using Mamey.Casino.Domain.Enums;
using Mamey.Casino.Domain.ValueObjects;
using Mamey.Types;
using Exception = System.Exception;
using Money = Mamey.Casino.Domain.ValueObjects.Money;

namespace Mamey.Casino.Domain.Entities;

/// <summary>
/// Represents a single round of the Crash game, 
/// where a multiplier grows exponentially until the round crashes.
/// </summary>
public sealed class CrashRound
{
    private readonly List<CrashBet> _bets = new();

    /// <summary>
    /// Gets the unique identifier of this round.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round ended, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? EndTime { get; private set; }

    /// <summary>
    /// Gets the multiplier at which the round crashed.
    /// </summary>
    public double CrashedAt { get; private set; }

    /// <summary>
    /// Gets the collection of bets placed in this round.
    /// </summary>
    public IReadOnlyCollection<CrashBet> Bets => _bets.AsReadOnly();

    private CrashRound()
    {
    }

    /// <summary>
    /// Creates a new <see cref="CrashRound"/> with the specified parameters.
    /// </summary>
    /// <param name="id">
    /// The unique identifier for the round. 
    /// If <see cref="Guid.Empty"/>, a new <see cref="Guid"/> will be generated.
    /// </param>
    /// <param name="startTime">The UTC time at which the round starts.</param>
    /// <param name="crashedAt">
    /// The crash multiplier. Must be greater than 1.0, 
    /// representing the exponential drop point.
    /// </param>
    /// <returns>A new initialized <see cref="CrashRound"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="crashedAt"/> is less than or equal to 1.0.
    /// </exception>
    public static CrashRound Create(Guid id, DateTimeOffset startTime, double crashedAt)
    {
        if (crashedAt <= 1.0)
            throw new ArgumentOutOfRangeException(
                nameof(crashedAt),
                "Crash multiplier must be greater than 1.0.");

        return new CrashRound
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime,
            CrashedAt = crashedAt
        };
    }

    /// <summary>
    /// Marks the round as completed by setting its end time.
    /// </summary>
    /// <param name="endTime">The UTC time at which the round ends.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round has already been completed or 
    /// if <paramref name="endTime"/> is earlier than <see cref="StartTime"/>.
    /// </exception>
    public void Complete(DateTimeOffset endTime)
    {
        if (EndTime.HasValue)
            throw new InvalidOperationException("Crash round is already completed.");

        if (endTime < StartTime)
            throw new InvalidOperationException("End time cannot be earlier than the start time.");

        EndTime = endTime;
    }

    /// <summary>
    /// Adds a new bet to this round.
    /// </summary>
    /// <param name="bet">The <see cref="CrashBet"/> to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bet"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round is completed or the bet’s <see cref="CrashBet.RoundId"/> 
    /// does not match this round’s <see cref="Id"/>.
    /// </exception>
    public void AddBet(CrashBet bet)
    {
        if (bet is null)
            throw new ArgumentNullException(nameof(bet));

        if (EndTime.HasValue)
            throw new InvalidOperationException("Cannot place a bet on a completed round.");

        if (bet.RoundId != Id)
            throw new InvalidOperationException("Bet.RoundId does not match this round's Id.");

        _bets.Add(bet);
    }
}

/// <summary>
/// Represents a single bet placed by a user in a Crash round.
/// </summary>
public sealed class CrashBet
{
    private CrashBet()
    {
    }

    /// <summary>
    /// Gets the unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the <see cref="CrashRound"/> to which this bet belongs.
    /// </summary>
    public Guid RoundId { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who placed this bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the amount staked for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this bet has been cashed out.
    /// </summary>
    public bool HasCashedOut { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the bet was cashed out, or <c>null</c> if not yet cashed out.
    /// </summary>
    public DateTimeOffset? CashOutTime { get; private set; }

    /// <summary>
    /// Creates a new <see cref="CrashBet"/> instance.
    /// </summary>
    /// <param name="id">
    /// The bet's unique identifier. If <see cref="Guid.Empty"/>, a new GUID will be generated.
    /// </param>
    /// <param name="roundId">The identifier of the Crash round.</param>
    /// <param name="userId">The identifier of the user placing the bet.</param>
    /// <param name="amount">The stake amount. Must be positive.</param>
    /// <returns>A new <see cref="CrashBet"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="amount"/> is less than or equal to zero.
    /// </exception>
    public static CrashBet Create(Guid id, Guid roundId, Guid userId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        return new CrashBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RoundId = roundId,
            UserId = userId,
            Amount = amount,
            HasCashedOut = false,
            CashOutTime = null
        };
    }

    /// <summary>
    /// Cashes out this bet at the specified timestamp.
    /// </summary>
    /// <param name="cashOutTime">The UTC time when the bet is cashed out.</param>
    /// <param name="roundStartTime">The UTC time when the Crash round started.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the bet is already cashed out or if <paramref name="cashOutTime"/> is before the round start.
    /// </exception>
    public void CashOut(DateTimeOffset cashOutTime, DateTimeOffset roundStartTime)
    {
        if (HasCashedOut)
            throw new InvalidOperationException("Bet has already been cashed out.");

        if (cashOutTime < roundStartTime)
            throw new InvalidOperationException("Cash-out time cannot be earlier than the round start time.");

        HasCashedOut = true;
        CashOutTime = cashOutTime;
    }

    /// <summary>
    /// Calculates the payout for this bet given the crash multiplier.
    /// </summary>
    /// <param name="crashMultiplier">The multiplier at which the round crashed.</param>
    /// <returns>
    /// The total amount returned to the user (stake × multiplier) if cashed out in time; otherwise zero.
    /// </returns>
    public decimal CalculatePayout(double crashMultiplier)
    {
        if (!HasCashedOut || CashOutTime == null)
            return 0m;

        // Even if cashed out, ensure cash-out occurred before crash:
        // (Caller should ensure CashOutTime <= crashTimestamp)
        return Amount * (decimal)crashMultiplier;
    }
}

/// <summary>
/// Represents a single round of European Roulette, 
/// where a ball lands on one of 37 pockets (0–36).
/// </summary>
public sealed class EuropeanRouletteRound
{
    private readonly List<RouletteBet> _bets = new();

    /// <summary>
    /// Gets the unique identifier of this round.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round ended, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? EndTime { get; private set; }

    /// <summary>
    /// Gets the winning pocket number (0–36) for this round, 
    /// or <c>null</c> if the round hasn't been completed.
    /// </summary>
    public int? WinningNumber { get; private set; }

    /// <summary>
    /// Gets the collection of bets placed in this round.
    /// </summary>
    public IReadOnlyCollection<RouletteBet> Bets => _bets.AsReadOnly();

    private EuropeanRouletteRound()
    {
    }

    /// <summary>
    /// Creates a new <see cref="EuropeanRouletteRound"/> instance.
    /// </summary>
    /// <param name="id">
    /// The round's unique identifier. If <see cref="Guid.Empty"/>, a new GUID will be generated.
    /// </param>
    /// <param name="startTime">The UTC time at which the round starts.</param>
    /// <returns>A new <see cref="EuropeanRouletteRound"/>.</returns>
    public static EuropeanRouletteRound Create(Guid id, DateTimeOffset startTime)
    {
        return new EuropeanRouletteRound
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };
    }

    /// <summary>
    /// Completes the round by setting its end time and winning number.
    /// </summary>
    /// <param name="endTime">The UTC time at which the round ends.</param>
    /// <param name="winningNumber">The winning pocket number (0–36).</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round is already completed, 
    /// if <paramref name="endTime"/> is before <see cref="StartTime"/>, 
    /// or if <paramref name="winningNumber"/> is outside the valid range.
    /// </exception>
    public void Complete(DateTimeOffset endTime, int winningNumber)
    {
        if (EndTime.HasValue)
            throw new InvalidOperationException("Roulette round is already completed.");

        if (endTime < StartTime)
            throw new InvalidOperationException("End time cannot be earlier than the start time.");

        if (winningNumber < 0 || winningNumber > 36)
            throw new ArgumentOutOfRangeException(nameof(winningNumber),
                "Winning number must be between 0 and 36 inclusive.");

        EndTime = endTime;
        WinningNumber = winningNumber;
    }

    /// <summary>
    /// Adds a new bet to this round.
    /// </summary>
    /// <param name="bet">The <see cref="RouletteBet"/> to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bet"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round is completed, 
    /// if the bet’s <see cref="RouletteBet.RoundId"/> does not match this round’s <see cref="Id"/>, 
    /// or if a bet with the same <see cref="RouletteBet.Id"/> already exists.
    /// </exception>
    public void AddBet(RouletteBet bet)
    {
        if (bet is null)
            throw new ArgumentNullException(nameof(bet));

        if (EndTime.HasValue)
            throw new InvalidOperationException("Cannot place a bet on a completed round.");

        if (bet.RoundId != Id)
            throw new InvalidOperationException("Bet.RoundId does not match this round's Id.");

        if (_bets.Exists(b => b.Id == bet.Id))
            throw new InvalidOperationException("A bet with the same Id has already been placed in this round.");

        _bets.Add(bet);
    }
}

/// <summary>
/// Represents a single round of the 3D Dice game, 
/// in which three dice are rolled and players place bets on the outcome.
/// </summary>
public sealed class Dice3DRoll
{
    private readonly List<DiceRoll> _rolls = new();

    /// <summary>
    /// Gets the unique identifier of this dice round.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round ended, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? EndTime { get; private set; }

    /// <summary>
    /// Gets the result of die 1 (1–6) after completion.
    /// </summary>
    public int? Die1 { get; private set; }

    /// <summary>
    /// Gets the result of die 2 (1–6) after completion.
    /// </summary>
    public int? Die2 { get; private set; }

    /// <summary>
    /// Gets the result of die 3 (1–6) after completion.
    /// </summary>
    public int? Die3 { get; private set; }

    /// <summary>
    /// Gets the collection of all player rolls (bets) placed in this round.
    /// </summary>
    public ICollection<DiceRoll> Rolls => _rolls.AsReadOnly();

    private Dice3DRoll()
    {
    }

    /// <summary>
    /// Creates a new <see cref="Dice3DRoll"/> with the specified start time.
    /// </summary>
    /// <param name="id">
    /// The round identifier. If <see cref="Guid.Empty"/>, a new GUID will be generated.
    /// </param>
    /// <param name="startTime">The UTC time at which the round starts.</param>
    /// <returns>A new <see cref="Dice3DRoll"/> instance.</returns>
    public static Dice3DRoll Create(Guid id, DateTimeOffset startTime)
    {
        return new Dice3DRoll
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };
    }

    /// <summary>
    /// Adds a player’s roll (bet) to this round.
    /// </summary>
    /// <param name="roll">The <see cref="DiceRoll"/> to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="roll"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round is completed, 
    /// or if the roll’s <see cref="DiceRoll.RoundId"/> does not match this round’s <see cref="Id"/>,
    /// or if a roll with the same <see cref="DiceRoll.Id"/> already exists.
    /// </exception>
    public void AddRoll(DiceRoll roll)
    {
        if (roll is null)
            throw new ArgumentNullException(nameof(roll));

        if (EndTime.HasValue)
            throw new InvalidOperationException("Cannot place a bet on a completed dice round.");

        if (roll.RoundId != Id)
            throw new InvalidOperationException("Roll.RoundId does not match this round's Id.");

        if (_rolls.Any(r => r.Id == roll.Id))
            throw new InvalidOperationException("A roll with the same Id has already been placed.");

        _rolls.Add(roll);
    }

    /// <summary>
    /// Completes the round by rolling three dice and setting the results.
    /// </summary>
    /// <param name="endTime">The UTC time at which the round ends.</param>
    /// <param name="die1">The face value of die 1 (must be 1–6).</param>
    /// <param name="die2">The face value of die 2 (must be 1–6).</param>
    /// <param name="die3">The face value of die 3 (must be 1–6).</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round is already completed or if <paramref name="endTime"/> is earlier than <see cref="StartTime"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if any die face is not between 1 and 6 inclusive.
    /// </exception>
    public void Complete(DateTimeOffset endTime, int die1, int die2, int die3)
    {
        if (EndTime.HasValue)
            throw new InvalidOperationException("Dice round is already completed.");

        if (endTime < StartTime)
            throw new InvalidOperationException("End time cannot be earlier than the start time.");

        if (die1 < 1 || die1 > 6) throw new ArgumentOutOfRangeException(nameof(die1), "Die1 must be between 1 and 6.");
        if (die2 < 1 || die2 > 6) throw new ArgumentOutOfRangeException(nameof(die2), "Die2 must be between 1 and 6.");
        if (die3 < 1 || die3 > 6) throw new ArgumentOutOfRangeException(nameof(die3), "Die3 must be between 1 and 6.");

        EndTime = endTime;
        Die1 = die1;
        Die2 = die2;
        Die3 = die3;
    }

    /// <summary>
    /// Calculates payouts for all rolls based on the completed dice results,
    /// using the provided payout multiplier function.
    /// </summary>
    /// <param name="payoutProvider">
    /// Function that maps a single <see cref="DiceRoll"/> and the three-dice results
    /// to a multiplier (e.g. 5.0 for correct face, 0 for loss).
    /// </param>
    /// <returns>
    /// A mapping from each roll’s <see cref="DiceRoll.Id"/> to its <see cref="ValueObjects.Money"/> payout.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the round has not yet been completed.
    /// </exception>
    public IDictionary<Guid, Money> CalculatePayouts(
        Func<DiceRoll, IList<int>, double> payoutProvider)
    {
        if (!EndTime.HasValue || !Die1.HasValue || !Die2.HasValue || !Die3.HasValue)
            throw new InvalidOperationException("Cannot calculate payouts before the round is completed.");

        if (payoutProvider == null)
            throw new ArgumentNullException(nameof(payoutProvider));

        var results = new List<int> { Die1.Value, Die2.Value, Die3.Value }.AsReadOnly();
        var payouts = new Dictionary<Guid, Money>(_rolls.Count);

        foreach (var roll in _rolls)
        {
            double multiplier = payoutProvider(roll, results);
            if (multiplier < 0)
                throw new InvalidOperationException("Payout provider returned invalid multiplier.");

            payouts[roll.Id] = new Money(roll.Amount.Amount * (decimal)multiplier);
        }

        return payouts;
    }
}

/// <summary>
/// Represents a single spin (round) of a 3D Slots game.
/// Stores the bet, variation, outcome grid and computes the payout.
/// </summary>
public sealed class Slots3DRound
{
    private readonly List<int[]> _grid = new();

    private Slots3DRound()
    {
    }

    /// <summary>
    /// Gets the unique identifier of this spin.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who played this spin.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the variation of the slot machine (e.g., payline configuration).
    /// </summary>
    public SlotVariation Variation { get; private set; }

    /// <summary>
    /// Gets the amount staked for this spin.
    /// </summary>
    public Money BetAmount { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the spin was initiated.
    /// </summary>
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the spin completed, or null if not yet completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the outcome grid of symbols, represented as an array of rows (reels).
    /// Each <c>int[]</c> corresponds to symbol IDs in a reel slice.
    /// </summary>
    public IList<int[]> Grid => _grid.AsReadOnly();

    /// <summary>
    /// Gets the payout multiplier calculated based on the outcome and <see cref="Variation"/>.
    /// </summary>
    public Multiplier PayoutMultiplier { get; private set; }

    /// <summary>
    /// Gets the total payout amount (Stake × Multiplier) returned to the player.
    /// </summary>
    public Money PayoutAmount { get; private set; }

    /// <summary>
    /// Begins a new slots spin.
    /// </summary>
    /// <param name="id">The spin's unique identifier; if empty, a new GUID will be generated.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="variation">The slot variation (defines reels, paylines).</param>
    /// <param name="betAmount">The stake amount; must be positive.</param>
    /// <param name="placedAt">The UTC time when the bet is placed.</param>
    /// <returns>A new <see cref="Slots3DRound"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="betAmount"/> is not positive.</exception>
    public static Slots3DRound Create(
        Guid id,
        UserId userId,
        SlotVariation variation,
        Money betAmount,
        DateTimeOffset placedAt)
    {
        if (betAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(betAmount), "Bet amount must be greater than zero.");

        return new Slots3DRound
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            Variation = variation,
            BetAmount = betAmount,
            PlacedAt = placedAt
        };
    }

    /// <summary>
    /// Completes the spin by recording the outcome grid and computing payout via provided logic.
    /// </summary>
    /// <param name="grid">
    /// The outcome grid: each element is a reel slice represented as an array of symbol IDs.
    /// </param>
    /// <param name="multiplierProvider">
    /// Function that maps the outcome grid and variation to a multiplier, configured in <c>Slots3DSettings</c>.
    /// </param>
    /// <param name="completedAt">The UTC time when the spin completed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the spin is already completed or if <paramref name="grid"/> is invalid.
    /// </exception>
    public void Complete(
        IEnumerable<int[]> grid,
        Func<IEnumerable<int[]>, SlotVariation, double> multiplierProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Slots spin is already completed.");

        if (grid is null)
            throw new ArgumentNullException(nameof(grid));

        var rows = grid.ToList();
        if (rows.Count == 0 || rows.Any(r => r == null || r.Length == 0))
            throw new InvalidOperationException("Outcome grid must be a non-empty collection of non-empty rows.");

        // Delegate multiplier calculation to service via provider
        var multiplierValue = multiplierProvider(rows, Variation);
        if (multiplierValue < 0)
            throw new InvalidOperationException("Multiplier provider returned an invalid value.");

        // Finalize state
        _grid.Clear();
        _grid.AddRange(rows);
        PayoutMultiplier = new Multiplier(multiplierValue);
        PayoutAmount = new Money(BetAmount.Amount * (decimal)PayoutMultiplier.Value);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a single Caribbean Poker game instance, 
/// where a player competes against the dealer with a five-card hand.
/// </summary>
public sealed class CaribbeanPokerGame
{
    private readonly List<string> _playerCards = new();
    private readonly List<string> _dealerCards = new();

    private CaribbeanPokerGame()
    {
    }

    /// <summary>
    /// Gets the unique identifier of this game.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user playing this game.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the stake amount for this game.
    /// </summary>
    public Money BetAmount { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when this game was placed.
    /// </summary>
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when this game completed, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the player's five cards, represented as standard codes (e.g., "AH", "10D").
    /// </summary>
    public IList<string> PlayerCards => _playerCards.AsReadOnly();

    /// <summary>
    /// Gets the dealer's five cards.
    /// </summary>
    public IList<string> DealerCards => _dealerCards.AsReadOnly();

    /// <summary>
    /// Gets the payout multiplier applied based on hand comparison.
    /// </summary>
    public double PayoutMultiplier { get; private set; }

    /// <summary>
    /// Gets the total payout amount (stake × multiplier) awarded to the player.
    /// </summary>
    public Money PayoutAmount { get; private set; }

    /// <summary>
    /// Creates a new CaribbeanPokerGame with the given stake.
    /// </summary>
    /// <param name="id">The game identifier, or <see cref="Guid.Empty"/> to generate a new one.</param>
    /// <param name="userId">The identifier of the user playing.</param>
    /// <param name="betAmount">The stake amount; must be positive.</param>
    /// <param name="placedAt">The UTC time when the game is placed.</param>
    /// <returns>A new <see cref="CaribbeanPokerGame"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="betAmount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="betAmount"/> is not greater than zero.</exception>
    public static CaribbeanPokerGame Create(
        Guid id,
        UserId userId,
        Money betAmount,
        DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (betAmount == null)
            throw new ArgumentNullException(nameof(betAmount));
        if (betAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(betAmount), "Bet amount must be greater than zero.");

        return new CaribbeanPokerGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            BetAmount = betAmount,
            PlacedAt = placedAt
        };
    }

    /// <summary>
    /// Completes the game by recording both hands and computing the payout.
    /// </summary>
    /// <param name="playerCards">
    /// The player's five cards. Must be a collection of exactly five unique codes.
    /// </param>
    /// <param name="dealerCards">
    /// The dealer's five cards. Must be exactly five unique codes distinct from player's cards.
    /// </param>
    /// <param name="payoutProvider">
    /// A function that, given the player's and dealer's hands, returns the payout multiplier.
    /// </param>
    /// <param name="completedAt">The UTC time when the game completes.</param>
    /// <exception cref="InvalidOperationException">If the game is already completed.</exception>
    /// <exception cref="ArgumentException">
    /// If either hand is invalid (wrong size, duplicates, or overlapping cards).
    /// </exception>
    public void Complete(
        IEnumerable<string> playerCards,
        IEnumerable<string> dealerCards,
        Func<IList<string>, IList<string>, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Game has already been completed.");

        if (playerCards == null)
            throw new ArgumentNullException(nameof(playerCards));
        if (dealerCards == null)
            throw new ArgumentNullException(nameof(dealerCards));
        if (payoutProvider == null)
            throw new ArgumentNullException(nameof(payoutProvider));

        var pList = playerCards.ToList();
        var dList = dealerCards.ToList();

        if (pList.Count != 5 || dList.Count != 5)
            throw new ArgumentException("Both player and dealer must have exactly five cards.");

        if (pList.Distinct().Count() != 5)
            throw new ArgumentException("Player hand contains duplicate cards.");
        if (dList.Distinct().Count() != 5)
            throw new ArgumentException("Dealer hand contains duplicate cards.");

        if (pList.Intersect(dList).Any())
            throw new ArgumentException("Player and dealer hands cannot share the same card.");

        _playerCards.Clear();
        _playerCards.AddRange(pList);
        _dealerCards.Clear();
        _dealerCards.AddRange(dList);

        // Determine multiplier using provided game rules / odds
        PayoutMultiplier = payoutProvider(PlayerCards, DealerCards);
        if (PayoutMultiplier < 0)
            throw new InvalidOperationException("Payout provider returned an invalid multiplier.");

        // Calculate final payout
        PayoutAmount = new Money(BetAmount.Amount * (decimal)PayoutMultiplier);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a single Casino Hold’em game instance, where a player competes against the dealer
/// using Texas Hold’em rules with community cards.
/// </summary>
public sealed class CasinoHoldemGame
{
    private readonly List<string> _playerCards = new(2);
    private readonly List<string> _dealerCards = new(2);
    private readonly List<string> _communityCards = new(5);

    private CasinoHoldemGame()
    {
    }

    /// <summary>
    /// Gets the unique identifier of this game.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user playing this game.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the ante bet amount for this game.
    /// </summary>
    public Money AnteAmount { get; private set; }

    /// <summary>
    /// Gets the optional raise bet amount, if the player chose to raise.
    /// </summary>
    public Money? RaiseAmount { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when this game was initiated.
    /// </summary>
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when this game was completed, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the two hole cards dealt to the player.
    /// </summary>
    public IList<string> PlayerCards => _playerCards.AsReadOnly();

    /// <summary>
    /// Gets the two hole cards dealt to the dealer.
    /// </summary>
    public IList<string> DealerCards => _dealerCards.AsReadOnly();

    /// <summary>
    /// Gets the five community cards.
    /// </summary>
    public IList<string> CommunityCards => _communityCards.AsReadOnly();

    /// <summary>
    /// Gets the payout multiplier applied based on hand comparisons and game rules.
    /// </summary>
    public double PayoutMultiplier { get; private set; }

    /// <summary>
    /// Gets the total payout amount (ante + raise, multiplied by multiplier) awarded to the player.
    /// </summary>
    public Money PayoutAmount { get; private set; }

    /// <summary>
    /// Starts a new Casino Hold’em game with the ante stake.
    /// </summary>
    /// <param name="id">The game identifier; if <see cref="Guid.Empty"/>, a new GUID is generated.</param>
    /// <param name="userId">The identifier of the user playing.</param>
    /// <param name="anteAmount">The ante bet amount; must be positive.</param>
    /// <param name="placedAt">The UTC timestamp when the game is placed.</param>
    /// <returns>A new <see cref="CasinoHoldemGame"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="anteAmount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="anteAmount"/> is not greater than zero.</exception>
    public static CasinoHoldemGame Create(
        Guid id,
        UserId userId,
        Money anteAmount,
        DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (anteAmount == null)
            throw new ArgumentNullException(nameof(anteAmount));
        if (anteAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(anteAmount), "Ante amount must be greater than zero.");

        return new CasinoHoldemGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            AnteAmount = anteAmount,
            PlacedAt = placedAt
        };
    }

    /// <summary>
    /// Completes the game by recording hole cards, community cards, optional raise, 
    /// and computing the payout via provided rules.
    /// </summary>
    /// <param name="playerCards">Exactly two unique player hole cards.</param>
    /// <param name="dealerCards">Exactly two unique dealer hole cards distinct from player's.</param>
    /// <param name="communityCards">Exactly five unique community cards distinct from hole cards.</param>
    /// <param name="raiseAmount">Optional raise amount; if provided, must be positive.</param>
    /// <param name="payoutProvider">
    /// A function that, given the full board and stakes, returns the payout multiplier.
    /// </param>
    /// <param name="completedAt">The UTC timestamp when the game completes.</param>
    /// <exception cref="InvalidOperationException">If the game is already completed.</exception>
    /// <exception cref="ArgumentException">If card collections are invalid or overlap.</exception>
    public void Complete(
        IEnumerable<string> playerCards,
        IEnumerable<string> dealerCards,
        IEnumerable<string> communityCards,
        Money? raiseAmount,
        Func<
                IList<string>, // player hole cards
                IList<string>, // dealer hole cards
                IList<string>, // community cards
                Money, // ante amount
                Money?, // raise amount
                double> // returns multiplier
            payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Game has already been completed.");
        if (playerCards == null || dealerCards == null || communityCards == null || payoutProvider == null)
            throw new ArgumentNullException("Cards and payoutProvider must not be null.");

        var p = playerCards.ToList();
        var d = dealerCards.ToList();
        var c = communityCards.ToList();

        if (p.Count != 2 || d.Count != 2 || c.Count != 5)
            throw new ArgumentException("Wrong number of cards: player/deviler should have 2, community 5.");
        if (p.Distinct().Count() != 2 || d.Distinct().Count() != 2 || c.Distinct().Count() != 5)
            throw new ArgumentException("Card collections contain duplicates.");
        if (p.Intersect(d).Any() || p.Intersect(c).Any() || d.Intersect(c).Any())
            throw new ArgumentException("Player, dealer, and community cards must all be distinct.");

        _playerCards.Clear();
        _playerCards.AddRange(p);
        _dealerCards.Clear();
        _dealerCards.AddRange(d);
        _communityCards.Clear();
        _communityCards.AddRange(c);

        if (raiseAmount != null && raiseAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(raiseAmount), "Raise amount must be positive.");

        RaiseAmount = raiseAmount;

        // Determine multiplier including ante and raise options
        PayoutMultiplier = payoutProvider(PlayerCards, DealerCards, CommunityCards, AnteAmount, RaiseAmount);
        if (PayoutMultiplier < 0)
            throw new InvalidOperationException("Payout provider returned an invalid multiplier.");

        // Compute final payout (ante + raise) * multiplier
        var totalStake = AnteAmount.Amount + (RaiseAmount?.Amount ?? 0m);
        PayoutAmount = new Money(totalStake * (decimal)PayoutMultiplier);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a single Heads-Or-Tails game: the player bets on one side,
/// a fair coin is flipped, and wins if the result matches their choice.
/// </summary>
public sealed class HeadsOrTailsGame
{
    /// <summary>
    /// Unique identifier of this game.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Amount staked on the bet.
    /// </summary>
    public Money BetAmount { get; private set; }

    /// <summary>
    /// The side the player chose (Heads or Tails).
    /// </summary>
    public HeadsOrTailsSide ChosenSide { get; private set; }

    /// <summary>
    /// UTC time when the game was placed.
    /// </summary>
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>
    /// UTC time when the game completed, or null if still pending.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// The result side of the coin flip (set upon completion).
    /// </summary>
    public HeadsOrTailsSide? ResultSide { get; private set; }

    /// <summary>
    /// True if the player won (chosen side equals result).
    /// </summary>
    public bool IsWin { get; private set; }

    /// <summary>
    /// Amount paid out: zero on loss, stake × 2 on win.
    /// </summary>
    public Money PayoutAmount { get; private set; }

    private HeadsOrTailsGame()
    {
    }

    /// <summary>
    /// Creates a new Heads-Or-Tails game with the specified parameters.
    /// </summary>
    /// <param name="id">
    /// The game identifier; if Guid.Empty, a new GUID is generated.
    /// </param>
    /// <param name="userId">The identifier of the betting user.</param>
    /// <param name="betAmount">The stake amount; must be positive.</param>
    /// <param name="chosenSide">The side the user selects.</param>
    /// <param name="placedAt">The UTC time when the bet is placed.</param>
    /// <returns>An initialized <see cref="HeadsOrTailsGame"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="userId"/> or <paramref name="betAmount"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="betAmount"/> is not greater than zero.
    /// </exception>
    public static HeadsOrTailsGame Create(
        Guid id,
        UserId userId,
        Money betAmount,
        HeadsOrTailsSide chosenSide,
        DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (betAmount == null)
            throw new ArgumentNullException(nameof(betAmount));
        if (betAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(betAmount), "Bet amount must be greater than zero.");

        return new HeadsOrTailsGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            BetAmount = betAmount,
            ChosenSide = chosenSide,
            PlacedAt = placedAt
        };
    }

    /// <summary>
    /// Completes the coin flip, determines win/loss, and computes payout.
    /// </summary>
    /// <param name="flipSide">The side that the coin landed on.</param>
    /// <param name="completedAt">The UTC time when the game completes.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the game has already been completed.
    /// </exception>
    public void Complete(HeadsOrTailsSide flipSide, DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Game has already been completed.");

        ResultSide = flipSide;
        IsWin = (flipSide == ChosenSide);

        PayoutAmount = IsWin
            ? new Money(BetAmount.Amount * 2m)
            : new Money(0m);

        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents an individual Sic Bo bet by a user.
/// </summary>
public sealed class SicBoBet
{
    /// <summary>
    /// Gets the unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the round identifier to which this bet belongs.
    /// </summary>
    public Guid RoundId { get; private set; }

    /// <summary>
    /// Gets the user identifier who placed the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the bet type (e.g. Big, Small, SpecificTriple, Sum).
    /// </summary>
    public SicBoBetType BetType { get; private set; }

    /// <summary>
    /// Gets the amount staked for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    /// Factory to create a new SicBoBet.
    /// </summary>
    /// <param name="id">Bet ID or Guid.Empty to generate new.</param>
    /// <param name="roundId">Associated round ID.</param>
    /// <param name="userId">User placing the bet.</param>
    /// <param name="betType">Type of bet.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="SicBoBet"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="amount"/> is ≤ 0.</exception>
    public static SicBoBet Create(Guid id, Guid roundId, Guid userId, SicBoBetType betType, Money amount)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        return new SicBoBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RoundId = roundId,
            UserId = userId,
            BetType = betType,
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a single Sic Bo game, where three dice are rolled and
/// players bet on various outcomes (e.g. Big/Small, specific triples, sums).
/// </summary>
public sealed class SicBoGame
{
    private readonly List<SicBoBet> _bets = new();

    /// <summary>
    /// Gets the unique identifier of this game round.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the round completed, or <c>null</c> if not yet completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the results of the three dice (values 1–6).
    /// </summary>
    public IList<int> DiceResults { get; private set; }

    /// <summary>
    /// Collection of all player bets for this round.
    /// </summary>
    public IReadOnlyCollection<SicBoBet> Bets => _bets.AsReadOnly();

    private SicBoGame()
    {
    }

    /// <summary>
    /// Starts a new Sic Bo round.
    /// </summary>
    /// <param name="id">The round ID; if <see cref="Guid.Empty"/>, a new GUID is generated.</param>
    /// <param name="startTime">The UTC time when the round starts.</param>
    /// <returns>An initialized <see cref="SicBoGame"/>.</returns>
    public static SicBoGame Create(Guid id, DateTimeOffset startTime)
    {
        return new SicBoGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime,
            DiceResults = new List<int>(3)
        };
    }

    /// <summary>
    /// Places a bet for the given user.
    /// </summary>
    /// <param name="bet">The <see cref="SicBoBet"/> to place.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="bet"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// If the round has completed or if <paramref name="bet"/>.RoundId does not match this round.
    /// </exception>
    public void AddBet(SicBoBet bet)
    {
        if (bet is null)
            throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Cannot place bets on a completed round.");
        if (bet.RoundId != Id)
            throw new InvalidOperationException("Bet.RoundId does not match this round's Id.");
        if (_bets.Any(b => b.Id == bet.Id))
            throw new InvalidOperationException("A bet with the same Id already exists in this round.");
        _bets.Add(bet);
    }

    /// <summary>
    /// Completes the round by recording dice results and computing payouts for all bets.
    /// </summary>
    /// <param name="diceResults">
    /// A collection of exactly three values (1–6) representing the dice roll.
    /// </param>
    /// <param name="payoutProvider">
    /// Function that, given a bet and the final dice results, returns the payout multiplier.
    /// </param>
    /// <param name="completedAt">The UTC time when the round completes.</param>
    /// <exception cref="InvalidOperationException">If already completed.</exception>
    /// <exception cref="ArgumentException">
    /// If <paramref name="diceResults"/> is not length three or values outside 1–6.
    /// </exception>
    public IReadOnlyDictionary<Guid, Money> Complete(
        IEnumerable<int> diceResults,
        Func<SicBoBet, IList<int>, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Round is already completed.");

        var results = diceResults?.ToList()
                      ?? throw new ArgumentException("Dice results cannot be null.", nameof(diceResults));

        if (results.Count != 3 || results.Any(d => d < 1 || d > 6))
            throw new ArgumentException("Dice results must be exactly three values between 1 and 6 inclusive.",
                nameof(diceResults));

        DiceResults = results;
        CompletedAt = completedAt;

        var payouts = new Dictionary<Guid, Money>(_bets.Count);
        foreach (var bet in _bets)
        {
            var multiplier = payoutProvider(bet, DiceResults);
            if (multiplier < 0)
                throw new InvalidOperationException("Payout provider returned invalid multiplier.");

            var payoutAmount = new Money(bet.Amount.Amount * (decimal)multiplier);
            payouts[bet.Id] = payoutAmount;
        }

        return payouts;
    }
}

/// <summary>
/// Represents a raffle draw, where participants buy tickets and a subset of winners is drawn.
/// </summary>
public sealed class Raffle
{
    private readonly List<RaffleTicket> _tickets = new();

    /// <summary>
    /// Gets the unique identifier of this raffle.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The raffle’s title or name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// UTC time when ticket sales opened.
    /// </summary>
    public DateTimeOffset OpensAt { get; private set; }

    /// <summary>
    /// UTC time when ticket sales close.
    /// </summary>
    public DateTimeOffset ClosesAt { get; private set; }

    /// <summary>
    /// UTC time when the raffle was drawn; null until drawing occurs.
    /// </summary>
    public DateTimeOffset? DrawnAt { get; private set; }

    /// <summary>
    /// The number of winners to select.
    /// </summary>
    public int WinnerCount { get; private set; }

    /// <summary>
    /// The tickets that won, by ticket ID.
    /// </summary>
    public IReadOnlyCollection<Guid> WinningTicketIds { get; private set; } = Array.Empty<Guid>();

    /// <summary>
    /// All tickets purchased for this raffle.
    /// </summary>
    public IReadOnlyCollection<RaffleTicket> Tickets => _tickets.AsReadOnly();

    private Raffle()
    {
    }

    /// <summary>
    /// Creates a new raffle with the given parameters.
    /// </summary>
    /// <param name="id">Raffle ID; Guid.Empty generates a new one.</param>
    /// <param name="name">Non-empty name of the raffle.</param>
    /// <param name="opensAt">UTC open time; must be before <paramref name="closesAt"/>.</param>
    /// <param name="closesAt">UTC close time; must be after <paramref name="opensAt"/>.</param>
    /// <param name="winnerCount">Number of winners; must be positive.</param>
    /// <returns>A new <see cref="Raffle"/> instance.</returns>
    /// <exception cref="ArgumentException">If arguments are invalid.</exception>
    public static Raffle Create(
        Guid id,
        string name,
        DateTimeOffset opensAt,
        DateTimeOffset closesAt,
        int winnerCount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Raffle name must be provided.", nameof(name));
        if (closesAt <= opensAt)
            throw new ArgumentException("ClosesAt must be after OpensAt.");
        if (winnerCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(winnerCount), "WinnerCount must be positive.");

        return new Raffle
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Name = name,
            OpensAt = opensAt,
            ClosesAt = closesAt,
            WinnerCount = winnerCount
        };
    }

    /// <summary>
    /// Purchases a ticket for a user.
    /// </summary>
    /// <param name="ticket">The <see cref="RaffleTicket"/> to add.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="ticket"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// If the raffle is closed for sales or ticket's RaffleId mismatches.
    /// </exception>
    public void AddTicket(RaffleTicket ticket)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));
        var now = DateTimeOffset.UtcNow;
        if (now < OpensAt || now > ClosesAt)
            throw new InvalidOperationException("Ticket sales are closed.");
        if (ticket.RaffleId != Id)
            throw new InvalidOperationException("Ticket does not belong to this raffle.");
        _tickets.Add(ticket);
    }

    /// <summary>
    /// Draws winners randomly and records their ticket IDs.
    /// </summary>
    /// <param name="drawnAt">UTC time of the draw; must be after <see cref="ClosesAt"/>.</param>
    /// <param name="randomProvider">Function supplying a random integer in [0, n).</param>
    /// <exception cref="InvalidOperationException">
    /// If already drawn, not enough tickets, or draw time invalid.
    /// </exception>
    public void Draw(DateTimeOffset drawnAt, Func<int, int> randomProvider)
    {
        if (DrawnAt.HasValue)
            throw new InvalidOperationException("Raffle has already been drawn.");
        if (drawnAt < ClosesAt)
            throw new InvalidOperationException("DrawnAt must be after ClosesAt.");
        if (_tickets.Count < WinnerCount)
            throw new InvalidOperationException("Not enough tickets to select winners.");
        if (randomProvider == null)
            throw new ArgumentNullException(nameof(randomProvider));

        var winners = new HashSet<Guid>();
        var pool = new List<RaffleTicket>(_tickets);
        while (winners.Count < WinnerCount)
        {
            var idx = randomProvider(pool.Count);
            var winner = pool[idx];
            winners.Add(winner.Id);
            pool.RemoveAt(idx);
        }

        WinningTicketIds = winners.ToList().AsReadOnly();
        DrawnAt = drawnAt;
    }
}

/// <summary>
/// Represents a single spin of a classic Slots game (non-3D),
/// where a grid of symbols is spun and payouts are awarded based on paylines.
/// </summary>
public sealed class SlotsGame
{
    private readonly List<int[]> _grid = new();

    private SlotsGame()
    {
    }

    /// <summary>
    /// Gets the unique identifier of this spin.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who spun.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the amount staked on this spin.
    /// </summary>
    public Money BetAmount { get; private set; }

    /// <summary>
    /// Gets the variation of the slot (defines reels and paylines).
    /// </summary>
    public SlotVariation Variation { get; private set; }

    /// <summary>
    /// UTC time when the spin was placed.
    /// </summary>
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>
    /// UTC time when the spin completed; null until <see cref="Complete"/> is called.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// The resulting grid: each array is the symbols on one reel.
    /// </summary>
    public IList<int[]> Grid => _grid.AsReadOnly();

    /// <summary>
    /// The payout multiplier determined by the outcome and paytable.
    /// </summary>
    public Multiplier PayoutMultiplier { get; private set; }

    /// <summary>
    /// The total amount returned to the player (stake × multiplier).
    /// </summary>
    public Money PayoutAmount { get; private set; }

    /// <summary>
    /// Starts a new Slots spin.
    /// </summary>
    /// <param name="id">Spin ID; Guid.Empty to generate new.</param>
    /// <param name="userId">User placing the bet.</param>
    /// <param name="variation">Slot variation.</param>
    /// <param name="betAmount">Stake; must be positive.</param>
    /// <param name="placedAt">UTC time of spin.</param>
    /// <returns>A new <see cref="SlotsGame"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="betAmount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="betAmount"/>.Amount ≤ 0.</exception>
    public static SlotsGame Create(
        Guid id,
        UserId userId,
        SlotVariation variation,
        Money betAmount,
        DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (betAmount == null)
            throw new ArgumentNullException(nameof(betAmount));
        if (betAmount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(betAmount), "Bet amount must be greater than zero.");

        return new SlotsGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            Variation = variation,
            BetAmount = betAmount,
            PlacedAt = placedAt
        };
    }

    /// <summary>
    /// Completes the spin by recording the outcome grid and computing payout.
    /// </summary>
    /// <param name="grid">
    /// Sequence of arrays, one per reel, of symbol IDs.
    /// </param>
    /// <param name="multiplierProvider">
    /// Function that maps (grid, variation) to a payout multiplier, 
    /// as defined by the paytable in <c>SlotsSettings</c>.
    /// </param>
    /// <param name="completedAt">UTC time when spin completes.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="grid"/> or <paramref name="multiplierProvider"/> is null.</exception>
    /// <exception cref="InvalidOperationException">If already completed or invalid grid.</exception>
    public void Complete(
        IEnumerable<int[]> grid,
        Func<IEnumerable<int[]>, SlotVariation, double> multiplierProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Spin is already completed.");
        if (grid == null)
            throw new ArgumentNullException(nameof(grid));
        if (multiplierProvider == null)
            throw new ArgumentNullException(nameof(multiplierProvider));

        var rows = grid.ToList();
        if (rows.Count == 0 || rows.Any(r => r == null || r.Length == 0))
            throw new InvalidOperationException("Grid must contain non-empty reels.");

        // Delegate payout calculation
        var value = multiplierProvider(rows, Variation);
        if (value < 0)
            throw new InvalidOperationException("Multiplier provider returned invalid value.");

        _grid.Clear();
        _grid.AddRange(rows);

        PayoutMultiplier = new Multiplier(value);
        PayoutAmount = new Money(BetAmount.Amount * (decimal)PayoutMultiplier.Value);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a single spin of a Lucky Wheel, 
/// where the wheel lands on one of several sections with associated multipliers.
/// </summary>
public sealed class LuckyWheelSpin
{
    private List<LuckyWheelSection> _sections;
    private readonly List<LuckyWheelBet> _bets = new();

    private LuckyWheelSpin()
    {
    }

    /// <summary>
    /// Unique identifier of this spin.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// UTC time when the spin started.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// UTC time when the spin completed; null until <see cref="Complete"/> is called.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// The section on which the wheel landed.
    /// </summary>
    public LuckyWheelSection? ResultSection { get; private set; }

    /// <summary>
    /// Collection of all bets placed.
    /// </summary>
    public IReadOnlyCollection<LuckyWheelBet> Bets => _bets.AsReadOnly();

    /// <summary>
    /// The configured sections of the wheel for odds lookup.
    /// </summary>
    public IList<LuckyWheelSection> Sections => _sections.AsReadOnly();

    /// <summary>
    /// Factory to create a new spin with configured sections.
    /// </summary>
    /// <param name="id">Spin ID; Guid.Empty to generate new.</param>
    /// <param name="sections">Ordered list of wheel sections (must be non-empty).</param>
    /// <param name="startTime">UTC start time.</param>
    /// <returns>A new <see cref="LuckyWheelSpin"/>.</returns>
    /// <exception cref="ArgumentException">If <paramref name="sections"/> is null or empty.</exception>
    public static LuckyWheelSpin Create(Guid id, IEnumerable<LuckyWheelSection> sections, DateTimeOffset startTime)
    {
        if (sections == null)
            throw new ArgumentNullException(nameof(sections));
        var list = new List<LuckyWheelSection>(sections);
        if (list.Count == 0)
            throw new ArgumentException("Sections must be provided.", nameof(sections));

        return new LuckyWheelSpin
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            _sections = list,
            StartTime = startTime
        };
    }

    /// <summary>
    /// Places a bet on a specific section.
    /// </summary>
    /// <param name="bet">The <see cref="LuckyWheelBet"/> to place.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="bet"/> is null.</exception>
    /// <exception cref="InvalidOperationException">If spin completed or invalid section.</exception>
    public void AddBet(LuckyWheelBet bet)
    {
        if (bet == null)
            throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Cannot place bet after spin completed.");
        if (!Sections.Contains(bet.Section))
            throw new InvalidOperationException("Bet section is not configured on this wheel.");
        if (_bets.Exists(b => b.Id == bet.Id))
            throw new InvalidOperationException("A bet with the same Id already exists.");
        _bets.Add(bet);
    }

    /// <summary>
    /// Completes the spin by selecting a result index and computing payouts.
    /// </summary>
    /// <param name="resultIndex">Index into <see cref="Sections"/> where the wheel landed.</param>
    /// <param name="payoutProvider">
    /// Function that, given a bet and the result section, returns multiplier.
    /// </param>
    /// <param name="completedAt">UTC completion time; must be ≥ StartTime.</param>
    /// <returns>Mapping of bet Ids to payout Money.</returns>
    /// <exception cref="InvalidOperationException">If already completed or invalid index.</exception>
    public IReadOnlyDictionary<Guid, Money> Complete(
        int resultIndex,
        Func<LuckyWheelBet, LuckyWheelSection, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Spin already completed.");
        if (resultIndex < 0 || resultIndex >= Sections.Count)
            throw new ArgumentOutOfRangeException(nameof(resultIndex));
        if (payoutProvider == null)
            throw new ArgumentNullException(nameof(payoutProvider));
        if (completedAt < StartTime)
            throw new InvalidOperationException("Completion time cannot be before start time.");

        var resultSection = Sections[resultIndex];
        ResultSection = resultSection;
        CompletedAt = completedAt;

        var payouts = new Dictionary<Guid, Money>(_bets.Count);
        foreach (var bet in _bets)
        {
            var multiplier = payoutProvider(bet, resultSection);
            if (multiplier < 0)
                throw new InvalidOperationException("Payout provider returned invalid multiplier.");
            payouts[bet.Id] = new Money(bet.Amount.Amount * (decimal)multiplier);
        }

        return payouts;
    }
}

/// <summary>
/// Represents a single bet on a Lucky Wheel section.
/// </summary>
public sealed class LuckyWheelBet
{
    /// <summary>
    /// Gets the unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The wheel section the user bet on.
    /// </summary>
    public LuckyWheelSection Section { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    /// Factory to create a new bet.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="section">Section chosen.</param>
    /// <param name="amount">Stake; must be positive.</param>
    /// <returns>A new <see cref="LuckyWheelBet"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="amount"/>.Amount ≤ 0.</exception>
    public static LuckyWheelBet Create(Guid id, LuckyWheelSection section, Money amount)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");
        return new LuckyWheelBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Section = section,
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a single bet in a Roulette round.
/// A bet may be on a specific number (0–36) or on a color (Red/Black).
/// </summary>
public sealed class RouletteBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the Roulette round this bet belongs to.
    /// </summary>
    public Guid RoundId { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Amount staked for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    /// If betting on a specific pocket, the number (0–36); otherwise null.
    /// </summary>
    public int? Number { get; private set; }

    /// <summary>
    /// If betting on a color, the color (Red or Black); otherwise null.
    /// </summary>
    public RouletteColor? Color { get; private set; }

    private RouletteBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="RouletteBet"/> instance.
    /// </summary>
    /// <param name="id">
    /// Bet ID; pass <see cref="Guid.Empty"/> to generate a new GUID.
    /// </param>
    /// <param name="roundId">The round to which this bet belongs.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="amount">The stake amount; must be positive.</param>
    /// <param name="number">
    /// The pocket number (0–36) for a straight bet; null if not betting on number.
    /// </param>
    /// <param name="color">
    /// The color (Red or Black) for a color bet; null if not betting on color.
    /// </param>
    /// <returns>A new <see cref="RouletteBet"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="userId"/> or <paramref name="amount"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if neither <paramref name="number"/> nor <paramref name="color"/> is specified,
    /// or if both are specified, or if <paramref name="number"/> is outside 0–36.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="amount"/>.Amount is not greater than zero.
    /// </exception>
    public static RouletteBet Create(
        Guid id,
        Guid roundId,
        UserId userId,
        Money amount,
        int? number,
        RouletteColor? color)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        bool hasNumber = number.HasValue;
        bool hasColor = color.HasValue;

        if (hasNumber == hasColor)
            throw new ArgumentException("Must specify exactly one of number or color.");

        if (hasNumber && (number < 0 || number > 36))
            throw new ArgumentException("Number must be between 0 and 36 inclusive.", nameof(number));

        return new RouletteBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RoundId = roundId,
            UserId = userId,
            Amount = amount,
            Number = number,
            Color = color
        };
    }
}

/// <summary>
/// Represents a single play of a Plinko game, where a puck drops
/// through pins into one of several slots with associated payouts.
/// </summary>
public sealed class PlinkoGame
{
    private readonly List<PlinkoBet> _bets = new();

    /// <summary>
    /// Unique identifier of this Plinko game instance.
    /// </summary>
    public Guid Id { get; private set; }
    
    public UserId UserId { get; private set; }

    /// <summary>
    /// UTC time when betting opened.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// UTC time when the puck landed; null until completion.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// The slot where the puck landed.
    /// </summary>
    public PlinkoSlot? ResultSlot { get; private set; }

    /// <summary>
    /// All bets placed for this play.
    /// </summary>
    public ICollection<PlinkoBet> Bets => _bets;

    private PlinkoGame()
    {
    }

    /// <summary>
    /// Starts a new Plinko game play.
    /// </summary>
    /// <param name="id">Game ID; Guid.Empty to generate new.</param>
    /// <param name="startTime">UTC time when play starts.</param>
    /// <returns>A new <see cref="PlinkoGame"/>.</returns>
    public static PlinkoGame Create(Guid id, DateTimeOffset startTime)
    {
        return new PlinkoGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };
    }

    /// <summary>
    /// Places a bet on a particular slot.
    /// </summary>
    /// <param name="bet">The <see cref="PlinkoBet"/> to add.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="bet"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// If the game is already completed or bet.GameId mismatches.
    /// </exception>
    public void AddBet(PlinkoBet bet)
    {
        if (bet == null)
            throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Cannot place bet after game completed.");
        if (bet.GameId != Id)
            throw new InvalidOperationException("Bet.GameId does not match this game's Id.");
        if (_bets.Any(x => x.Id == bet.Id))
            throw new InvalidOperationException("A bet with the same Id already exists.");
        _bets.Add(bet);
    }

    /// <summary>
    /// Completes the Plinko drop by specifying the result slot and computing payouts.
    /// </summary>
    /// <param name="resultSlot">The slot where the puck landed.</param>
    /// <param name="payoutProvider">
    /// Function that, given a bet and the result slot, returns a payout multiplier.
    /// </param>
    /// <param name="completedAt">UTC time when the play completes.</param>
    /// <returns>Mapping of bet Id to payout Money.</returns>
    /// <exception cref="InvalidOperationException">
    /// If already completed or invalid multiplier.
    /// </exception>
    public IReadOnlyDictionary<Guid, Money> Complete(
        PlinkoSlot resultSlot,
        Func<PlinkoBet, PlinkoSlot, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue)
            throw new InvalidOperationException("Game already completed.");

        ResultSlot = resultSlot;
        CompletedAt = completedAt;

        if (payoutProvider == null)
            throw new ArgumentNullException(nameof(payoutProvider));

        var payouts = new Dictionary<Guid, Money>(_bets.Count);
        foreach (var bet in _bets)
        {
            var multiplier = payoutProvider(bet, resultSlot);
            if (multiplier < 0)
                throw new InvalidOperationException("Payout provider returned invalid multiplier.");
            payouts[bet.Id] = new Money(bet.Amount.Amount * (decimal)multiplier);
        }

        return payouts;
    }
}

/// <summary>
/// Represents a single bet in a Plinko game.
/// </summary>
public sealed class PlinkoBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }
    
    public UserId UserId { get; private set; }

    /// <summary>
    /// The Plinko game this bet belongs to.
    /// </summary>
    public Guid GameId { get; private set; }

    /// <summary>
    /// The slot the user is betting on.
    /// </summary>
    public PlinkoSlot Slot { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private PlinkoBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PlinkoBet"/>.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="gameId">The Plinko game ID.</param>
    /// <param name="slot">The slot chosen.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="PlinkoBet"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="amount"/>.Amount ≤ 0.</exception>
    public static PlinkoBet Create(
        Guid id,
        UserId userId,
        Guid gameId,
        PlinkoSlot slot,
        Money amount)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        return new PlinkoBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            GameId = gameId,
            Slot = slot,
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a unique identifier for bets across games.
/// Wraps a non-empty GUID to ensure semantic clarity.
/// </summary>
public readonly struct BetId : IEquatable<BetId>
{
    /// <summary>
    /// Underlying GUID value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BetId"/> with the specified GUID.
    /// </summary>
    /// <param name="value">The GUID to wrap; must not be <see cref="Guid.Empty"/>.</param>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public BetId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("BetId cannot be an empty GUID.", nameof(value));
        Value = value;
    }

    /// <inheritdoc/>
    public bool Equals(BetId other) => Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is BetId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of the underlying GUID.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicitly converts a <see cref="BetId"/> to a <see cref="Guid"/>.
    /// </summary>
    public static implicit operator Guid(BetId id) => id.Value;

    /// <summary>
    /// Implicitly creates a <see cref="BetId"/> from a non-empty <see cref="Guid"/>.
    /// </summary>
    public static implicit operator BetId(Guid value) => new BetId(value);
}

/// <summary>
/// Represents a single bet on a standard dice roll (one die).
/// </summary>
public sealed class DiceRoll
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the dice round this bet belongs to.
    /// </summary>
    public Guid RoundId { get; private set; }

    /// <summary>
    /// Identifier of the user who placed the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// The face value (1–6) the user bets will appear.
    /// </summary>
    public DiceResult BetOn { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private DiceRoll()
    {
    }

    /// <summary>
    /// Creates a new <see cref="DiceRoll"/> bet.
    /// </summary>
    /// <param name="id">Bet ID; <see cref="Guid.Empty"/> to generate a new one.</param>
    /// <param name="roundId">The dice round’s ID.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="betOn">The face value (1–6) being bet on.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="DiceRoll"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="amount"/>.Amount ≤ 0.
    /// </exception>
    public static DiceRoll Create(
        Guid id,
        Guid roundId,
        UserId userId,
        DiceResult betOn,
        Money amount)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        return new DiceRoll
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RoundId = roundId,
            UserId = userId,
            BetOn = betOn,
            Amount = amount
        };
    }

    /// <summary>
    /// Calculates the payout for this bet given the actual roll result.
    /// </summary>
    /// <param name="actual">
    /// The face value rolled (1–6).
    /// </param>
    /// <param name="payoutMultiplier">
    /// Multiplier to apply on win (e.g., 5 for correct face, 0 for loss).
    /// </param>
    /// <returns>
    /// The payout amount (stake × multiplier) if <paramref name="actual"/> equals <see cref="BetOn"/>, 
    /// otherwise zero.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="actual"/> is outside 1–6,
    /// or if <paramref name="payoutMultiplier"/> is negative.
    /// </exception>
    public Money CalculatePayout(DiceResult actual, double payoutMultiplier)
    {
        int actualValue = (int)actual;
        if (actualValue < 1 || actualValue > 6)
            throw new ArgumentOutOfRangeException(nameof(actual), "Actual result must be between 1 and 6.");
        if (payoutMultiplier < 0)
            throw new ArgumentOutOfRangeException(nameof(payoutMultiplier), "Payout multiplier cannot be negative.");

        if (actual == BetOn)
            return new Money(Amount.Amount * (decimal)payoutMultiplier);

        return new Money(0m);
    }
}

/// <summary>
/// Represents a single ticket purchased in a raffle.
/// </summary>
public sealed class RaffleTicket
{
    /// <summary>
    /// Gets the unique identifier of this ticket.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the raffle this ticket belongs to.
    /// </summary>
    public Guid RaffleId { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who purchased the ticket.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the ticket was purchased.
    /// </summary>
    public DateTimeOffset PurchasedAt { get; private set; }

    /// <summary>
    /// Gets the price paid for this ticket.
    /// </summary>
    public Money Price { get; private set; }

    private RaffleTicket()
    {
    }

    /// <summary>
    /// Creates a new raffle ticket.
    /// </summary>
    /// <param name="id">
    /// The ticket's unique identifier. If <see cref="Guid.Empty"/>, a new GUID will be generated.
    /// </param>
    /// <param name="raffleId">The raffle to which this ticket belongs.</param>
    /// <param name="userId">The user purchasing the ticket.</param>
    /// <param name="price">The price paid; must be positive.</param>
    /// <param name="purchasedAt">UTC time of purchase.</param>
    /// <returns>A new <see cref="RaffleTicket"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="userId"/> or <paramref name="price"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="price"/>.Amount is less than or equal to zero.
    /// </exception>
    public static RaffleTicket Create(
        Guid id,
        Guid raffleId,
        UserId userId,
        Money price,
        DateTimeOffset purchasedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (price == null)
            throw new ArgumentNullException(nameof(price));
        if (price.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Ticket price must be greater than zero.");

        return new RaffleTicket
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RaffleId = raffleId,
            UserId = userId,
            Price = price,
            PurchasedAt = purchasedAt
        };
    }
}

/// <summary>
/// Represents a single 3D-dice bet placed by a user.
/// </summary>
public sealed class Dice3DBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the dice round.
    /// </summary>
    public Guid RoundId { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// The type of 3D-dice bet (big, small, sum, etc.).
    /// </summary>
    public Dice3DType BetType { get; private set; }

    /// <summary>
    /// If BetType is Sum, this specifies the exact sum (3–18); otherwise null.
    /// </summary>
    public int? SumValue { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private Dice3DBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="Dice3DBet"/>.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="roundId">The dice-3D round ID.</param>
    /// <param name="userId">User placing the bet.</param>
    /// <param name="betType">Type of bet.</param>
    /// <param name="sumValue">Exact sum if applicable (3–18).</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="Dice3DBet"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="amount"/>.Amount ≤ 0, or if <paramref name="sumValue"/> is out of range for Sum bets.
    /// </exception>
    public static Dice3DBet Create(
        Guid id,
        Guid roundId,
        UserId userId,
        Dice3DType betType,
        int? sumValue,
        Money amount)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        if (betType == Dice3DType.Sum)
        {
            if (!sumValue.HasValue || sumValue < 3 || sumValue > 18)
                throw new ArgumentOutOfRangeException(nameof(sumValue), "Sum must be between 3 and 18.");
        }

        return new Dice3DBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RoundId = roundId,
            UserId = userId,
            BetType = betType,
            SumValue = sumValue,
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a single round of Multiplayer Roulette, 
/// where multiple players place bets on the same spin.
/// </summary>
public sealed class MultiplayerRouletteRound
{
    private readonly List<RouletteBet> _bets = new();

    /// <summary>
    /// Unique identifier of this round.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// UTC start time.
    /// </summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>
    /// UTC end time once completed.
    /// </summary>
    public DateTimeOffset? EndTime { get; private set; }

    /// <summary>
    /// Winning pocket (0–36).
    /// </summary>
    public int? WinningNumber { get; private set; }

    /// <summary>
    /// All bets placed by players.
    /// </summary>
    public IReadOnlyCollection<RouletteBet> Bets => _bets.AsReadOnly();

    private MultiplayerRouletteRound()
    {
    }

    /// <summary>
    /// Creates a new multiplayer round.
    /// </summary>
    public static MultiplayerRouletteRound Create(Guid id, DateTimeOffset startTime) =>
        new MultiplayerRouletteRound
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };

    /// <summary>
    /// Place a bet in this round.
    /// </summary>
    public void AddBet(RouletteBet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (EndTime.HasValue) throw new InvalidOperationException("Round completed.");
        if (bet.RoundId != Id) throw new InvalidOperationException("RoundId mismatch.");
        if (_bets.Any(b => b.Id == bet.Id)) throw new InvalidOperationException("Duplicate bet.");
        _bets.Add(bet);
    }

    /// <summary>
    /// Completes the spin and computes all payouts.
    /// </summary>
    public IReadOnlyDictionary<Guid, Money> Complete(
        int winningNumber,
        Func<RouletteBet, int, double> payoutProvider,
        DateTimeOffset endTime)
    {
        if (EndTime.HasValue) throw new InvalidOperationException("Already completed.");
        if (winningNumber < 0 || winningNumber > 36) throw new ArgumentOutOfRangeException(nameof(winningNumber));
        if (endTime < StartTime) throw new InvalidOperationException("End before start.");

        WinningNumber = winningNumber;
        EndTime = endTime;

        var results = new Dictionary<Guid, Money>();
        foreach (var bet in _bets)
        {
            var mult = payoutProvider(bet, winningNumber);
            if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");
            results[bet.Id] = new Money(bet.Amount.Amount * (decimal)mult);
        }

        return results;
    }
}

/// <summary>
/// Represents a single Keno game, 
/// where players choose a set of numbers and then a draw is made.
/// </summary>
public sealed class KenoGame
{
    private readonly List<KenoBet> _bets = new();

    public Guid Id { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IList<int> DrawnNumbers { get; private set; } = Array.Empty<int>();
    public IReadOnlyCollection<KenoBet> Bets => _bets.AsReadOnly();

    private KenoGame()
    {
    }

    public static KenoGame Create(Guid id, DateTimeOffset startTime) =>
        new KenoGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };

    public void AddBet(KenoBet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue) throw new InvalidOperationException("Game completed.");
        if (bet.GameId != Id) throw new InvalidOperationException("GameId mismatch.");
        _bets.Add(bet);
    }

    public IReadOnlyDictionary<Guid, Money> Complete(
        IEnumerable<int> drawnNumbers,
        Func<KenoBet, IList<int>, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        var draw = drawnNumbers?.ToList() ?? throw new ArgumentNullException(nameof(drawnNumbers));
        if (draw.Count != 20 || draw.Any(n => n < 1 || n > 80))
            throw new ArgumentException("Must draw 20 unique numbers between 1–80.");
        if (completedAt < StartTime) throw new InvalidOperationException("Complete before start.");

        DrawnNumbers = draw.AsReadOnly();
        CompletedAt = completedAt;

        var results = new Dictionary<Guid, Money>();
        foreach (var bet in _bets)
        {
            var mult = payoutProvider(bet, DrawnNumbers);
            if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");
            results[bet.Id] = new Money(bet.Amount.Amount * (decimal)mult);
        }

        return results;
    }
}

/// <summary>
/// Represents a single Blackjack game between one player and dealer.
/// </summary>
public sealed class BlackjackGame
{
    private readonly List<string> _playerHand = new();
    private readonly List<string> _dealerHand = new();

    public Guid Id { get; private set; }
    public UserId UserId { get; private set; }
    public Money BetAmount { get; private set; }
    public DateTimeOffset PlacedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IList<string> PlayerHand => _playerHand.AsReadOnly();
    public IList<string> DealerHand => _dealerHand.AsReadOnly();
    public Money Payout { get; private set; }

    private BlackjackGame()
    {
    }

    public static BlackjackGame Create(Guid id, UserId userId, Money bet, DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (bet.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(bet));

        return new BlackjackGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            BetAmount = bet,
            PlacedAt = placedAt
        };
    }

    public void DealInitial(IEnumerable<string> player, IEnumerable<string> dealer)
    {
        if (CompletedAt.HasValue) throw new InvalidOperationException("Already completed.");
        _playerHand.Clear();
        _playerHand.AddRange(player);
        _dealerHand.Clear();
        _dealerHand.AddRange(dealer);
    }

    public void Complete(
        Func<IList<string>, IList<string>, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue) throw new InvalidOperationException("Already completed.");
        if (payoutProvider == null) throw new ArgumentNullException(nameof(payoutProvider));

        var mult = payoutProvider(PlayerHand, DealerHand);
        if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");

        Payout = new Money(BetAmount.Amount * (decimal)mult);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a multiplayer blackjack room where many players play together.
/// </summary>
public sealed class MultiplayerBlackjackRoom
{
    private readonly List<BlackjackGame> _games = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<BlackjackGame> Games => _games.AsReadOnly();

    private MultiplayerBlackjackRoom()
    {
    }

    public static MultiplayerBlackjackRoom Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        return new MultiplayerBlackjackRoom
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Name = name
        };
    }

    public void AddGame(BlackjackGame game)
    {
        if (game == null) throw new ArgumentNullException(nameof(game));
        if (_games.Any(g => g.Id == game.Id)) throw new InvalidOperationException("Duplicate game");
        _games.Add(game);
    }
}

/// <summary>
/// Represents an American Roulette round (38 pockets).
/// </summary>
public sealed class AmericanRouletteRound
{
    private readonly List<RouletteBet> _bets = new();

    public Guid Id { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset? EndTime { get; private set; }
    public int? WinningNumber { get; private set; }
    public IReadOnlyCollection<RouletteBet> Bets => _bets.AsReadOnly();

    private AmericanRouletteRound()
    {
    }

    public static AmericanRouletteRound Create(Guid id, DateTimeOffset startTime) =>
        new AmericanRouletteRound
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime
        };

    public void AddBet(RouletteBet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (EndTime.HasValue) throw new InvalidOperationException("Completed.");
        if (bet.RoundId != Id) throw new InvalidOperationException("RoundId mismatch.");
        _bets.Add(bet);
    }

    public IReadOnlyDictionary<Guid, Money> Complete(
        int winningNumber,
        Func<RouletteBet, int, double> payoutProvider,
        DateTimeOffset endTime)
    {
        if (EndTime.HasValue) throw new InvalidOperationException("Already completed.");
        if (winningNumber < 0 || winningNumber > 37) throw new ArgumentOutOfRangeException(nameof(winningNumber));
        if (endTime < StartTime) throw new InvalidOperationException("End before start.");

        WinningNumber = winningNumber;
        EndTime = endTime;

        var results = new Dictionary<Guid, Money>();
        foreach (var bet in _bets)
        {
            var mult = payoutProvider(bet, winningNumber);
            if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");
            results[bet.Id] = new Money(bet.Amount.Amount * (decimal)mult);
        }

        return results;
    }
}

/// <summary>
/// Represents a single Video Poker game instance.
/// </summary>
public sealed class VideoPokerGame
{
    private readonly List<string> _initialHand = new();
    private readonly List<string> _finalHand = new();

    public Guid Id { get; private set; }
    public UserId UserId { get; private set; }
    public Money BetAmount { get; private set; }
    public DateTimeOffset PlacedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IList<string> InitialHand => _initialHand.AsReadOnly();
    public IList<string> FinalHand => _finalHand.AsReadOnly();
    public Money Payout { get; private set; }

    private VideoPokerGame()
    {
    }

    public static VideoPokerGame Create(Guid id, UserId userId, Money bet, DateTimeOffset placedAt)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (bet.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(bet));

        return new VideoPokerGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            UserId = userId,
            BetAmount = bet,
            PlacedAt = placedAt
        };
    }

    public void DealInitial(IEnumerable<string> cards)
    {
        _initialHand.Clear();
        _initialHand.AddRange(cards);
    }

    public void Complete(
        IEnumerable<string> finalCards,
        Func<IList<string>, IList<string>, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue) throw new InvalidOperationException("Already completed.");
        _finalHand.Clear();
        _finalHand.AddRange(finalCards);

        var mult = payoutProvider(InitialHand, FinalHand);
        if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");

        Payout = new Money(BetAmount.Amount * (decimal)mult);
        CompletedAt = completedAt;
    }
}

/// <summary>
/// Represents a single Crypto Prediction game, where users bet on whether
/// a crypto price will be above or below a target at a given time.
/// </summary>
public sealed class CryptoPredictionGame
{
    private readonly List<CryptoPredictionBet> _bets = new();

    public Guid Id { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public double TargetPrice { get; private set; }
    public CryptoPredictionOutcome? Outcome { get; private set; }
    public IReadOnlyCollection<CryptoPredictionBet> Bets => _bets.AsReadOnly();

    private CryptoPredictionGame()
    {
    }

    public static CryptoPredictionGame Create(Guid id, DateTimeOffset startTime, double targetPrice)
    {
        if (targetPrice <= 0) throw new ArgumentOutOfRangeException(nameof(targetPrice));
        return new CryptoPredictionGame
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            StartTime = startTime,
            TargetPrice = targetPrice
        };
    }

    public void AddBet(CryptoPredictionBet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue) throw new InvalidOperationException("Completed.");
        _bets.Add(bet);
    }

    public IReadOnlyDictionary<Guid, Money> Complete(
        double closingPrice,
        Func<CryptoPredictionBet, CryptoPredictionOutcome, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        if (CompletedAt.HasValue) throw new InvalidOperationException("Already completed.");
        if (completedAt < StartTime) throw new InvalidOperationException("Complete before start.");

        Outcome = closingPrice >= TargetPrice
            ? CryptoPredictionOutcome.Above
            : CryptoPredictionOutcome.Below;
        CompletedAt = completedAt;

        var results = new Dictionary<Guid, Money>();
        foreach (var bet in _bets)
        {
            var mult = payoutProvider(bet, Outcome.Value);
            if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");
            results[bet.Id] = new Money(bet.Amount.Amount * (decimal)mult);
        }

        return results;
    }
}

/// <summary>
/// Represents a single Horse Racing event, with a set of participants and bets.
/// </summary>
public sealed class HorseRacingRace
{
    private readonly List<HorseBet> _bets = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public IList<HorsePosition> Results { get; private set; } = Array.Empty<HorsePosition>();
    public IReadOnlyCollection<HorseBet> Bets => _bets.AsReadOnly();

    private HorseRacingRace()
    {
    }

    public static HorseRacingRace Create(Guid id, string name, DateTimeOffset startTime)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        return new HorseRacingRace
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Name = name,
            StartTime = startTime
        };
    }

    public void AddBet(HorseBet bet)
    {
        if (bet == null) throw new ArgumentNullException(nameof(bet));
        if (CompletedAt.HasValue) throw new InvalidOperationException("Completed.");
        _bets.Add(bet);
    }

    public IReadOnlyDictionary<Guid, Money> Complete(
        IEnumerable<HorsePosition> results,
        Func<HorseBet, HorsePosition, double> payoutProvider,
        DateTimeOffset completedAt)
    {
        var list = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
        if (list.Count < 1) throw new ArgumentException("At least one position must be provided.");
        if (completedAt < StartTime) throw new InvalidOperationException("Complete before start.");

        Results = list.AsReadOnly();
        CompletedAt = completedAt;

        var payouts = new Dictionary<Guid, Money>();
        foreach (var bet in _bets)
        {
            var mult = payoutProvider(bet, Results[(int)bet.PickOrder - 1]);
            if (mult < 0) throw new InvalidOperationException("Invalid multiplier.");
            payouts[bet.Id] = new Money(bet.Amount.Amount * (decimal)mult);
        }

        return payouts;
    }
}

/// <summary>
/// Represents a single bet in a Keno game.
/// </summary>
public sealed class KenoBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the Keno game this bet belongs to.
    /// </summary>
    public Guid GameId { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// The set of numbers (1–80) chosen by the player.
    /// </summary>
    public IList<int> Numbers { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private KenoBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="KenoBet"/>.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="gameId">The Keno game ID.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="numbers">The chosen numbers; must be 1–10 unique numbers from 1–80.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="KenoBet"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/>, <paramref name="numbers"/>, or <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentException">If numbers count is not between 1 and 10, contains duplicates, or values out of 1–80.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="amount"/>.Amount ≤ 0.</exception>
    public static KenoBet Create(
        Guid id,
        Guid gameId,
        UserId userId,
        IEnumerable<int> numbers,
        Money amount)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (numbers == null) throw new ArgumentNullException(nameof(numbers));
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        var list = numbers.ToList();
        if (list.Count < 1 || list.Count > 10)
            throw new ArgumentException("You must pick between 1 and 10 numbers.", nameof(numbers));
        if (list.Distinct().Count() != list.Count)
            throw new ArgumentException("Numbers must be unique.", nameof(numbers));
        if (list.Any(n => n < 1 || n > 80))
            throw new ArgumentException("Numbers must be between 1 and 80.", nameof(numbers));

        return new KenoBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            GameId = gameId,
            UserId = userId,
            Numbers = list.AsReadOnly(),
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a single bet in a Crypto Prediction game.
/// </summary>
public sealed class CryptoPredictionBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the Crypto Prediction game this bet belongs to.
    /// </summary>
    public Guid GameId { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// The predicted outcome (Above or Below).
    /// </summary>
    public CryptoPredictionOutcome Prediction { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private CryptoPredictionBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="CryptoPredictionBet"/>.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="gameId">Crypto Prediction game ID.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="prediction">The predicted outcome.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="CryptoPredictionBet"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="amount"/>.Amount ≤ 0.</exception>
    public static CryptoPredictionBet Create(
        Guid id,
        Guid gameId,
        UserId userId,
        CryptoPredictionOutcome prediction,
        Money amount)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");

        return new CryptoPredictionBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            GameId = gameId,
            UserId = userId,
            Prediction = prediction,
            Amount = amount
        };
    }
}

/// <summary>
/// Represents a single bet in a Horse Racing race.
/// </summary>
public sealed class HorseBet
{
    /// <summary>
    /// Unique identifier of this bet.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identifier of the Horse Racing race this bet belongs to.
    /// </summary>
    public Guid RaceId { get; private set; }

    /// <summary>
    /// Identifier of the user placing the bet.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// The position (1=first, 2=second, etc.) the user bets on.
    /// </summary>
    public HorsePosition PickOrder { get; private set; }

    /// <summary>
    /// The stake amount for this bet.
    /// </summary>
    public Money Amount { get; private set; }

    private HorseBet()
    {
    }

    /// <summary>
    /// Creates a new <see cref="HorseBet"/>.
    /// </summary>
    /// <param name="id">Bet ID; Guid.Empty to generate new.</param>
    /// <param name="raceId">Horse Racing race ID.</param>
    /// <param name="userId">The user placing the bet.</param>
    /// <param name="pickOrder">The finishing position being bet on.</param>
    /// <param name="amount">Stake amount; must be positive.</param>
    /// <returns>A new <see cref="HorseBet"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="userId"/> or <paramref name="amount"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="amount"/>.Amount ≤ 0, 
    /// or if <paramref name="pickOrder"/> is less than First or greater than Unplaced.
    /// </exception>
    public static HorseBet Create(
        Guid id,
        Guid raceId,
        UserId userId,
        HorsePosition pickOrder,
        Money amount)
    {
        if (userId.IsEmpty)
            throw new Exception($"{nameof(userId)} cannot be empty.");
        if (amount == null) throw new ArgumentNullException(nameof(amount));
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");
        if (!Enum.IsDefined(typeof(HorsePosition), pickOrder))
            throw new ArgumentOutOfRangeException(nameof(pickOrder), "Invalid horse position.");

        return new HorseBet
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            RaceId = raceId,
            UserId = userId,
            PickOrder = pickOrder,
            Amount = amount
        };
    }
}