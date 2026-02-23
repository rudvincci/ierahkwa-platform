using System.Collections.Generic;
using System.Linq;
using Mamey.ServiceName.Domain.Entities;
using Mamey.ServiceName.Domain.Events;
using Mamey.ServiceName.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Mamey.ServiceName.Tests.Unit.Core.Entities;

public class CreateEntityNameTests
{
    private EntityName Act(EntityId id, string name, IEnumerable<string> tags) => EntityName.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_entityname_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var entityname = Act(id, name, tags);
        
        // Assert
        entityname.ShouldNotBeNull();
        entityname.Id.ToString().ShouldBe(id.ToString());
        entityname.Tags.ShouldBe(tags);
        entityname.Events.Count().ShouldBe(1);

        var @event = entityname.Events.Single();
        @event.ShouldBeOfType<EntityNameCreated>();
    }

    [Fact]
    public void given_empty_tags_entityname_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingEntityNameTagsException>();
    }
}