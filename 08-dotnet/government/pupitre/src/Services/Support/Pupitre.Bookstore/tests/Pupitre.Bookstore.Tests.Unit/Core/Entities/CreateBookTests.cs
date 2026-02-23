using System.Collections.Generic;
using System.Linq;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Domain.Events;
using Pupitre.Bookstore.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Bookstore.Tests.Unit.Core.Entities;

public class CreateBookTests
{
    private Book Act(EntityId id, string name, IEnumerable<string> tags) => Book.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_book_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var book = Act(id, name, tags);
        
        // Assert
        book.ShouldNotBeNull();
        book.Id.ToString().ShouldBe(id.ToString());
        book.Tags.ShouldBe(tags);
        book.Events.Count().ShouldBe(1);

        var @event = book.Events.Single();
        @event.ShouldBeOfType<BookCreated>();
    }

    [Fact]
    public void given_empty_tags_book_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingBookTagsException>();
    }
}