using System.Collections.Generic;
using System.Linq;
using Pupitre.Educators.Domain.Entities;
using Pupitre.Educators.Domain.Events;
using Pupitre.Educators.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Educators.Tests.Unit.Core.Entities;

public class CreateEducatorTests
{
    private Educator Act(EntityId id, string name, IEnumerable<string> tags) => Educator.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_educator_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var educator = Act(id, name, tags);
        
        // Assert
        educator.ShouldNotBeNull();
        educator.Id.ToString().ShouldBe(id.ToString());
        educator.Tags.ShouldBe(tags);
        educator.Events.Count().ShouldBe(1);

        var @event = educator.Events.Single();
        @event.ShouldBeOfType<EducatorCreated>();
    }

    [Fact]
    public void given_empty_tags_educator_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingEducatorTagsException>();
    }
}