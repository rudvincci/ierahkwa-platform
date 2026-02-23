using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mamey.ApplicationName.Modules.Raffles.Domain.Domain;

internal class Payment
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Participant paying for the ticket(s).
    /// </summary>
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; } = null!;

    /// <summary>
    /// Payment date/time
    /// </summary>
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The total amount paid in this transaction (covers 1 or more tickets).
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment status: e.g., "Pending", "Completed", "Failed", "Refunded".
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Payment method or gateway reference, e.g., "CreditCard", "PayPal".
    /// </summary>
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "CreditCard";

    /// <summary>
    /// External transaction ID from the payment provider.
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// A collection of tickets associated with this payment.
    /// (One payment could buy multiple tickets in a single transaction.)
    /// </summary>
    public List<Ticket> Tickets { get; set; } = new();
}