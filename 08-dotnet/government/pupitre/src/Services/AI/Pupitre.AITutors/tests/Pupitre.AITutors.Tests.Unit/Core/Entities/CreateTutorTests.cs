using System.Collections.Generic;
using System.Linq;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Domain.Events;
using Pupitre.AITutors.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AITutors.Tests.Unit.Core.Entities;

public class CreateTutorTests
{
    private Tutor Act(EntityId id, string name, IEnumerable<string> tags) => Tutor.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_tutor_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var tutor = Act(id, name, tags);
        
        // Assert
        tutor.ShouldNotBeNull();
        tutor.Id.ToString().ShouldBe(id.ToString());
        tutor.Tags.ShouldBe(tags);
        tutor.Events.Count().ShouldBe(1);

        var @event = tutor.Events.Single();
        @event.ShouldBeOfType<TutorCreated>();
    }

    [Fact]
    public void given_empty_tags_tutor_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingTutorTagsException>();
    }
}