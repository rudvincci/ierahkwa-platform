using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Bookstore.Application.DTO;

internal class BookDetailsDto : BookDto
{
    public BookDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public BookDetailsDto(BookDto bookDto, DateTime createdAt, DateTime? modifiedAt)
        : base(bookDto.Id, bookDto.Name, bookDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
