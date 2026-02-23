using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class RafflePrize
{
    [Key] public int Id { get; set; }

    public int RaffleId { get; set; }
    public Raffle Raffle { get; set; } = null!;

    /// <summary>
    /// Example: "Grand Prize", "Second Prize", "Jackpot", "Bonus Prize", etc.
    /// </summary>
    [Required, MaxLength(100)]
    public string PrizeName { get; set; } = "Grand Prize";

    /// <summary>
    /// A short description of the prize, or monetary value if it's a cash prize.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// If you want the system to be aware of the ranking or tier order.
    /// Lower rank means it's drawn first or is highest priority.
    /// </summary>
    public int RankOrder { get; set; } = 1;

    /// <summary>
    /// True if this prize has been awarded (when the draw happens).
    /// Could be updated once the draw is done.
    /// </summary>
    public bool IsAwarded { get; set; } = false;
}