using System.ComponentModel.DataAnnotations;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class Raffle
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// For example, "Summer BBQ Raffle"
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A more detailed description of the raffle.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Categorize the raffle (e.g., "Charity", "Sports", "Festival").
    /// </summary>
    public int? CategoryId { get; set; }
    public RaffleCategory? Category { get; set; }

    /// <summary>
    /// Indicates which raffle variant logic to apply (e.g., "Standard", "MultiTier", etc.).
    /// This ties into the IRaffleVariant classes via a factory or strategy pattern.
    /// </summary>
    [Required, MaxLength(50)]
    public string VariantType { get; set; } = "Standard";

    /// <summary>
    /// Additional configuration or JSON-serialized settings that the variant might need.
    /// For example: prize tiers, progressive rules, etc.
    /// </summary>
    public string? VariantConfiguration { get; set; }

    /// <summary>
    /// When does the raffle start accepting tickets?
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When does the raffle stop accepting tickets?
    /// </summary>
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// When is the drawing scheduled?
    /// </summary>
    public DateTime DrawingDate { get; set; } = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// Current status of the raffle (e.g., Draft, Active, Completed, Canceled).
    /// </summary>
    public RaffleStatus Status { get; set; } = RaffleStatus.Draft;

    /// <summary>
    /// Navigation property for all tickets sold/issued for this raffle.
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();

    /// <summary>
    /// Navigation property for prizes if it is a multi-prize or tiered raffle.
    /// This can define how many prizes exist, their rank order, etc.
    /// </summary>
    public List<RafflePrize> Prizes { get; set; } = new();

    /// <summary>
    /// Optional: For progressive/rolling jackpots or repeated draws, you may keep a collection of rounds.
    /// Each round records a partial draw or jackpot carryover.
    /// </summary>
    public List<RaffleRound> Rounds { get; set; } = new();
}