using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class RaffleRound
{
    [Key] public int Id { get; set; }

    public int RaffleId { get; set; }
    public Raffle Raffle { get; set; } = null!;

    /// <summary>
    /// For progressive or recurring raffles, increment each time a draw is held.
    /// E.g., round 1, round 2, round 3...
    /// </summary>
    public int RoundNumber { get; set; }

    /// <summary>
    /// The date/time this round was drawn or is scheduled to be drawn.
    /// </summary>
    public DateTime RoundDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// If there's a rolling jackpot, store it here. For 50/50, store pot info, etc.
    /// </summary>
    public decimal JackpotAmount { get; set; } = 0m;

    /// <summary>
    /// Indicates if a winner was determined in this round.
    /// If not, it might roll over to the next round.
    /// </summary>
    public bool IsClaimed { get; set; } = false;
}