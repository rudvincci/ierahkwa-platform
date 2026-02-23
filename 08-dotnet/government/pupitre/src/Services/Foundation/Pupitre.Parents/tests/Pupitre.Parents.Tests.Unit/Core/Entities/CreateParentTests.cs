using System.Collections.Generic;
using System.Linq;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Domain.Events;
using Pupitre.Parents.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Parents.Tests.Unit.Core.Entities;

public class CreateParentTests
{
    private Parent Act(EntityId id, string name, IEnumerable<string> tags) => Parent.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_parent_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var parent = Act(id, name, tags);
        
        // Assert
        parent.ShouldNotBeNull();
        parent.Id.ToString().ShouldBe(id.ToString());
        parent.Tags.ShouldBe(tags);
        parent.Events.Count().ShouldBe(1);

        var @event = parent.Events.Single();
        @event.ShouldBeOfType<ParentCreated>();
    }

    [Fact]
    public void given_empty_tags_parent_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingParentTagsException>();
    }
}