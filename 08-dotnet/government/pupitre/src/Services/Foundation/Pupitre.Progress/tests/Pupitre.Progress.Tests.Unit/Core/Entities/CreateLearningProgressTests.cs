using System.Collections.Generic;
using System.Linq;
using Pupitre.Progress.Domain.Entities;
using Pupitre.Progress.Domain.Events;
using Pupitre.Progress.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Progress.Tests.Unit.Core.Entities;

public class CreateLearningProgressTests
{
    private LearningProgress Act(EntityId id, string name, IEnumerable<string> tags) => LearningProgress.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_learningprogress_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var learningprogress = Act(id, name, tags);
        
        // Assert
        learningprogress.ShouldNotBeNull();
        learningprogress.Id.ToString().ShouldBe(id.ToString());
        learningprogress.Tags.ShouldBe(tags);
        learningprogress.Events.Count().ShouldBe(1);

        var @event = learningprogress.Events.Single();
        @event.ShouldBeOfType<LearningProgressCreated>();
    }

    [Fact]
    public void given_empty_tags_learningprogress_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingLearningProgressTagsException>();
    }
}