using Common.Application.DTOs;
using Librarian.Application.DTOs;
using Librarian.Domain.Entities;

namespace Librarian.Application.Interfaces;

public interface IBookService
{
    Task<BookDto?> GetByIdAsync(int id);
    Task<BookDto?> GetByISBNAsync(string isbn);
    Task<PagedResult<BookDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<BookDto>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    Task<IEnumerable<BookDto>> SearchAsync(string searchTerm);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<BookDto> UpdateAsync(int id, CreateBookDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateQuantityAsync(int id, int quantity);
}

public interface IBookCategoryService
{
    Task<BookCategoryDto?> GetByIdAsync(int id);
    Task<PagedResult<BookCategoryDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<BookCategoryDto>> GetTreeAsync();
    Task<BookCategoryDto> CreateAsync(CreateBookCategoryDto dto);
    Task<BookCategoryDto> UpdateAsync(int id, CreateBookCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ILibraryMemberService
{
    Task<LibraryMemberDto?> GetByIdAsync(int id);
    Task<LibraryMemberDto?> GetByMemberNumberAsync(string memberNumber);
    Task<LibraryMemberDto?> GetByStudentIdAsync(int studentId);
    Task<LibraryMemberDto?> GetByTeacherIdAsync(int teacherId);
    Task<PagedResult<LibraryMemberDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<LibraryMemberDto>> GetByTypeAsync(MemberType type);
    Task<LibraryMemberDto> CreateAsync(CreateLibraryMemberDto dto);
    Task<LibraryMemberDto> UpdateAsync(int id, CreateLibraryMemberDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> RenewMembershipAsync(int id, DateTime newExpiryDate);
}

public interface IBorrowService
{
    Task<BorrowTransactionDto?> GetByIdAsync(int id);
    Task<BorrowTransactionDto?> GetByTransactionNumberAsync(string transactionNumber);
    Task<PagedResult<BorrowTransactionDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<BorrowTransactionDto>> GetByMemberAsync(int memberId);
    Task<IEnumerable<BorrowTransactionDto>> GetByBookAsync(int bookId);
    Task<IEnumerable<BorrowTransactionDto>> GetActiveAsync();
    Task<IEnumerable<BorrowTransactionDto>> GetOverdueAsync();
    Task<BorrowTransactionDto> BorrowAsync(int issuedBy, CreateBorrowDto dto);
    Task<BorrowTransactionDto> ReturnAsync(int returnedTo, ReturnBookDto dto);
    Task<BorrowTransactionDto> RenewAsync(RenewBookDto dto);
    Task<bool> MarkAsLostAsync(int transactionId, decimal fineAmount);
    Task<LibraryDashboardDto> GetDashboardAsync();
}
