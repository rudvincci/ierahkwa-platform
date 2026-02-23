using Common.Domain.Entities;

namespace Librarian.Domain.Entities;

public class Book : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Edition { get; set; }
    public int? CategoryId { get; set; }
    public string? Subject { get; set; }
    public string? RackNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public int AvailableQuantity { get; set; } = 1;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual BookCategory? Category { get; set; }
    public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
}
