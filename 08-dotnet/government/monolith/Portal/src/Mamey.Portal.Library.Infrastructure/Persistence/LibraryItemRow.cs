using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mamey.Portal.Library.Application.Models;

namespace Mamey.Portal.Library.Infrastructure.Persistence;

[Table("library_items")]
public sealed class LibraryItemRow
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string TenantId { get; set; } = default!;

    [Required]
    [MaxLength(128)]
    public string Category { get; set; } = default!;

    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = default!;

    [MaxLength(2048)]
    public string? Summary { get; set; }

    [Required]
    [MaxLength(32)]
    public LibraryVisibility Visibility { get; set; }

    [Required]
    [MaxLength(32)]
    public LibraryContentStatus Status { get; set; }

    [Required]
    [MaxLength(256)]
    public string FileName { get; set; } = default!;

    [Required]
    [MaxLength(128)]
    public string ContentType { get; set; } = default!;

    public long Size { get; set; }

    [Required]
    [MaxLength(128)]
    public string StorageBucket { get; set; } = default!;

    [Required]
    [MaxLength(512)]
    public string StorageKey { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [MaxLength(256)]
    public string? CreatedBy { get; set; }

    [MaxLength(256)]
    public string? UpdatedBy { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }
}




