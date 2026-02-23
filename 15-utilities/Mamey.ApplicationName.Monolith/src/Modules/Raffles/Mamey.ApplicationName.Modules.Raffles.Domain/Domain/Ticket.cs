using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class Ticket
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// A unique alphanumeric identifier for the ticket. 
    /// Typically displayed or emailed to the participant.
    /// </summary>
    [Required, MaxLength(50)]
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// The associated raffle. 
    /// </summary>
    public int RaffleId { get; set; }
    public Raffle Raffle { get; set; } = null!;

    /// <summary>
    /// The participant who purchased or received this ticket.
    /// </summary>
    public int OwnerId { get; set; }
    public Participant Owner { get; set; } = null!;

    /// <summary>
    /// Price of a single ticket or share. 
    /// (Might be 0 for free or door-prize raffles.)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } = 0m;

    /// <summary>
    /// If you record a Payment entity for online transactions,
    /// store the PaymentId reference here.
    /// </summary>
    public int? PaymentId { get; set; }
    public Payment? Payment { get; set; }

    /// <summary>
    /// Indicates if this ticket has been drawn as a winner.
    /// </summary>
    public bool IsWinner { get; set; } = false;

    /// <summary>
    /// If multi-tier, store which prize was won. E.g. "FirstPrize", "Jackpot", "DoorPrize", etc.
    /// </summary>
    public string? PrizeAwarded { get; set; }
}