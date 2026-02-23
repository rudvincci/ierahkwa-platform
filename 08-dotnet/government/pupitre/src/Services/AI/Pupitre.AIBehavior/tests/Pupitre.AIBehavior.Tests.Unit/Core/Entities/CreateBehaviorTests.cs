using System.Collections.Generic;
using System.Linq;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Domain.Events;
using Pupitre.AIBehavior.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AIBehavior.Tests.Unit.Core.Entities;

public class CreateBehaviorTests
{
    private Behavior Act(EntityId id, string name, IEnumerable<string> tags) => Behavior.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_behavior_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var behavior = Act(id, name, tags);
        
        // Assert
        behavior.ShouldNotBeNull();
        behavior.Id.ToString().ShouldBe(id.ToString());
        behavior.Tags.ShouldBe(tags);
        behavior.Events.Count().ShouldBe(1);

        var @event = behavior.Events.Single();
        @event.ShouldBeOfType<BehaviorCreated>();
    }

    [Fact]
    public void given_empty_tags_behavior_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingBehaviorTagsException>();
    }
}