using Librarian.Domain.Entities;

namespace Librarian.Application.DTOs;

// Book DTOs
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int? PublishYear { get; set; }
    public string? Edition { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Subject { get; set; }
    public string? RackNumber { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public bool IsActive { get; set; }
}

public class CreateBookDto
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
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}

// Book Category DTOs
public class BookCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public int BookCount { get; set; }
    public IEnumerable<BookCategoryDto> Children { get; set; } = new List<BookCategoryDto>();
}

public class CreateBookCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}

// Library Member DTOs
public class LibraryMemberDto
{
    public int Id { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public MemberType Type { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public int? StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime JoinDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxBooksAllowed { get; set; }
    public int CurrentBorrowCount { get; set; }
    public bool IsActive { get; set; }
}

public class CreateLibraryMemberDto
{
    public MemberType Type { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public int? StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxBooksAllowed { get; set; } = 3;
}

// Borrow Transaction DTOs
public class BorrowTransactionDto
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? MemberNumber { get; set; }
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
    public string? ISBN { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public TransactionStatus Status { get; set; }
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; }
    public string? Notes { get; set; }
    public int? IssuedBy { get; set; }
    public string? IssuedByName { get; set; }
    public int DaysOverdue { get; set; }
}

public class CreateBorrowDto
{
    public int MemberId { get; set; }
    public int BookId { get; set; }
    public int BorrowDays { get; set; } = 14;
    public string? Notes { get; set; }
}

public class ReturnBookDto
{
    public int TransactionId { get; set; }
    public decimal? FineAmount { get; set; }
    public bool FinePaid { get; set; }
    public string? Notes { get; set; }
}

public class RenewBookDto
{
    public int TransactionId { get; set; }
    public int ExtendDays { get; set; } = 7;
}

public class LibraryDashboardDto
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBooks { get; set; }
    public decimal TotalFines { get; set; }
    public decimal CollectedFines { get; set; }
    public IEnumerable<BorrowTransactionDto> RecentTransactions { get; set; } = new List<BorrowTransactionDto>();
    public IEnumerable<BorrowTransactionDto> OverdueTransactions { get; set; } = new List<BorrowTransactionDto>();
}
