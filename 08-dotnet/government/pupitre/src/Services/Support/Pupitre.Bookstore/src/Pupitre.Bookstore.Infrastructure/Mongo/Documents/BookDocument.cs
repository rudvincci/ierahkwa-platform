using Pupitre.Bookstore.Application.DTO;
using Pupitre.Bookstore.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.Integration.Async")]
namespace Pupitre.Bookstore.Infrastructure.Mongo.Documents;

internal class BookDocument : IIdentifiable<Guid>
{
    public BookDocument()
    {

    }

    public BookDocument(Book book)
    {
        if (book is null)
        {
            throw new NullReferenceException();
        }

        Id = book.Id.Value;
        Name = book.Name;
        CreatedAt = book.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = book.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = book.Tags;
        Version = book.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public Book AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public BookDto AsDto()
        => new BookDto(Id, Name, Tags);
    public BookDetailsDto AsDetailsDto()
        => new BookDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}

