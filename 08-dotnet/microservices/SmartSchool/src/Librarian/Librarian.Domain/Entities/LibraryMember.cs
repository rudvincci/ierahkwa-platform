using Common.Domain.Entities;

namespace Librarian.Domain.Entities;

public class LibraryMember : TenantEntity
{
    public string MemberNumber { get; set; } = string.Empty;
    public MemberType Type { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public int? StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public int MaxBooksAllowed { get; set; } = 3;
    public int CurrentBorrowCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
}

public enum MemberType
{
    Student,
    Teacher,
    Staff,
    External
}
