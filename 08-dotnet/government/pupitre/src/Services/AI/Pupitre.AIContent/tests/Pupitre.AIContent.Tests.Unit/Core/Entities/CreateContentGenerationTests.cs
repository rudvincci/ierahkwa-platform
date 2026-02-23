using System.Collections.Generic;
using System.Linq;
using Pupitre.AIContent.Domain.Entities;
using Pupitre.AIContent.Domain.Events;
using Pupitre.AIContent.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AIContent.Tests.Unit.Core.Entities;

public class CreateContentGenerationTests
{
    private ContentGeneration Act(EntityId id, string name, IEnumerable<string> tags) => ContentGeneration.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_contentgeneration_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var contentgeneration = Act(id, name, tags);
        
        // Assert
        contentgeneration.ShouldNotBeNull();
        contentgeneration.Id.ToString().ShouldBe(id.ToString());
        contentgeneration.Tags.ShouldBe(tags);
        contentgeneration.Events.Count().ShouldBe(1);

        var @event = contentgeneration.Events.Single();
        @event.ShouldBeOfType<ContentGenerationCreated>();
    }

    [Fact]
    public void given_empty_tags_contentgeneration_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingContentGenerationTagsException>();
    }
}