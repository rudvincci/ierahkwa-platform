using Common.Domain.Entities;

namespace Librarian.Domain.Entities;

public class BorrowTransaction : TenantEntity
{
    public string TransactionNumber { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public int BookId { get; set; }
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Borrowed;
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; } = false;
    public string? Notes { get; set; }
    public int? IssuedBy { get; set; }
    public int? ReturnedTo { get; set; }
    
    public virtual LibraryMember? Member { get; set; }
    public virtual Book? Book { get; set; }
}

public enum TransactionStatus
{
    Borrowed,
    Returned,
    Overdue,
    Lost,
    Renewed
}
