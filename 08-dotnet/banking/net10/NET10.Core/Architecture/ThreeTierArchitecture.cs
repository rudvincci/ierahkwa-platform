// ═══════════════════════════════════════════════════════════════════════════════
// THREE-TIER ARCHITECTURE - NET10 WEB ERP
// ═══════════════════════════════════════════════════════════════════════════════
// Layer 1: Presentation Layer (UI) - NET10.API / wwwroot
// Layer 2: Business Logic Layer (BLL) - NET10.Core
// Layer 3: Data Access Layer (DAL) - NET10.Infrastructure
// ═══════════════════════════════════════════════════════════════════════════════

namespace NET10.Core.Architecture;

/// <summary>
/// Base Repository Interface - Data Access Layer Contract
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<int> CountAsync();
}

/// <summary>
/// Unit of Work Pattern - Transaction Management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

/// <summary>
/// Base Entity for all domain models
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Audit Entity with full tracking
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public List<AuditLog> AuditLogs { get; set; } = [];
}

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

/// <summary>
/// Result wrapper for business operations
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public int StatusCode { get; set; } = 200;
    
    public static Result<T> Ok(T data, string message = "Success") => new()
    {
        Success = true,
        Data = data,
        Message = message,
        StatusCode = 200
    };
    
    public static Result<T> Fail(string error, int statusCode = 400) => new()
    {
        Success = false,
        Message = error,
        Errors = [error],
        StatusCode = statusCode
    };
    
    public static Result<T> Fail(List<string> errors, int statusCode = 400) => new()
    {
        Success = false,
        Errors = errors,
        Message = string.Join(", ", errors),
        StatusCode = statusCode
    };
}

/// <summary>
/// Pagination support
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public string? SearchTerm { get; set; }
}
