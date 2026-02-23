using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Bookstore.Domain.Events;
using Pupitre.Bookstore.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.Unit.Core.Entities")]
namespace Pupitre.Bookstore.Domain.Entities;


internal class Book : AggregateRoot<BookId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Book(BookId id, string name, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null, int version = 0)
        : base(id, version)
    {
        Name = name;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Tags = tags ?? Enumerable.Empty<string>();
    }

    #region Properties

    /// <summary>
    /// A name for the book.
    /// </summary>
    [Description("The book's name")]
    public string Name { get; private set; }

    /// <summary>
    /// Date and time the record was created.
    /// </summary>
    [Description("Date and time the record was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the record was modified.
    /// </summary>
    [Description("Date and time the record was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Collection of Book tags.
    /// </summary>
    [Description("Collection of Book tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static Book Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingBookNameException();
        }

        var book = new Book(id, name, DateTime.UtcNow, tags: tags);
        book.AddEvent(new BookCreated(book));
        return book;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new BookModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidBookTagsException();
        }
    }
}

