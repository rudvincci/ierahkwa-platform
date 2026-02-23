namespace Mamey.Casino.Domain.Enums;

// <summary>
/// All supported game types in the system.
/// </summary>
public enum GameType
{
    Crash,
    EuropeanRoulette,
    AmericanRoulette,
    MultiplayerRoulette,
    Dice,
    Dice3D,
    Keno,
    Slots,
    Slots3D,
    CaribbeanPoker,
    CasinoHoldem,
    Blackjack,
    MultiplayerBlackjack,
    HeadsOrTails,
    SicBo,
    VideoPoker,
    Raffle,
    CryptoPrediction,
    HorseRacing,
    LuckyWheel,
    Plinko
}

/// <summary>
/// States of a Crash game round.
/// </summary>
public enum CrashState
{
    WaitingForBets,
    InProgress,
    Crashed
}

/// <summary>
/// Pocket colors in Roulette.
/// </summary>
public enum RouletteColor
{
    Green,
    Red,
    Black
}

/// <summary>
/// Bet types available in 3D Dice.
/// </summary>
public enum Dice3DType
{
    SpecificTriple,
    AnyTriple,
    Big, // sum 11–17
    Small, // sum 4–10
    Sum, // exact sum
    Single, // specific face appears
    Double, // any double appears
    TotalOdd,
    TotalEven
}

/// <summary>
/// Result of a single die roll.
/// </summary>
public enum DiceResult
{
    One = 1,
    Two,
    Three,
    Four,
    Five,
    Six
}

/// <summary>
/// Number of spots in Keno.
/// </summary>
public enum KenoCategory
{
    Spot1 = 1,
    Spot2,
    Spot3,
    Spot4,
    Spot5,
    Spot6,
    Spot7,
    Spot8,
    Spot9,
    Spot10
}

/// <summary>
/// Variations of classic Slots (defines reels, paylines).
/// </summary>
public enum SlotVariation
{
    ThreeReel,
    FiveReel,
    MultiPayline,
    Progressive
}

/// <summary>
/// Move types in poker games.
/// </summary>
public enum PokerMoveType
{
    Fold,
    Check,
    Call,
    Bet,
    Raise
}

/// <summary>
/// Player phases/actions in Blackjack.
/// </summary>
public enum BlackjackMoveType
{
    PlayerTurn,
    DealerTurn,
    Completed
}

/// <summary>
/// Side choice in Heads-Or-Tails.
/// </summary>
public enum HeadsOrTailsSide
{
    Heads,
    Tails
}

/// <summary>
/// Bet types in Sic Bo.
/// </summary>
public enum SicBoBetType
{
    Big,
    Small,
    SpecificTriple,
    AnyTriple,
    SpecificDouble,
    AnyDouble,
    ThreeDiceSum,
    TwoDiceCombination,
    SingleNumber
}

/// <summary>
/// Hand combinations in Video Poker.
/// </summary>
public enum VideoPokerCombination
{
    NoPair,
    JacksOrBetter,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

/// <summary>
/// Status of a Raffle.
/// </summary>
public enum RaffleStatus
{
    Open,
    Closed,
    Drawn
}

/// <summary>
/// Outcome of a Crypto Prediction bet.
/// </summary>
public enum CryptoPredictionOutcome
{
    Above,
    Below
}

/// <summary>
/// Finishing positions in Horse Racing.
/// </summary>
public enum HorsePosition
{
    First = 1,
    Second,
    Third,
    Fourth,
    Fifth,
    Unplaced
}

/// <summary>
/// Sections on a Lucky Wheel.
/// </summary>
public enum LuckyWheelSection
{
    Section1,
    Section2,
    Section3,
    Section4,
    Section5,
    Bonus
}

/// <summary>
/// Slots at bottom of a Plinko board.
/// </summary>
public enum PlinkoSlot
{
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
    Jackpot
}