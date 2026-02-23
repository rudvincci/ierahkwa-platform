using System.Collections.Generic;
using System.Linq;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Domain.Events;
using Pupitre.GLEs.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.GLEs.Tests.Unit.Core.Entities;

public class CreateGLETests
{
    private GLE Act(EntityId id, string name, IEnumerable<string> tags) => GLE.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_gle_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var gle = Act(id, name, tags);
        
        // Assert
        gle.ShouldNotBeNull();
        gle.Id.ToString().ShouldBe(id.ToString());
        gle.Tags.ShouldBe(tags);
        gle.Events.Count().ShouldBe(1);

        var @event = gle.Events.Single();
        @event.ShouldBeOfType<GLECreated>();
    }

    [Fact]
    public void given_empty_tags_gle_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingGLETagsException>();
    }
}