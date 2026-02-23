using Common.Domain.Entities;

namespace Librarian.Domain.Entities;

public class BookCategory : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual BookCategory? Parent { get; set; }
    public virtual ICollection<BookCategory> Children { get; set; } = new List<BookCategory>();
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
