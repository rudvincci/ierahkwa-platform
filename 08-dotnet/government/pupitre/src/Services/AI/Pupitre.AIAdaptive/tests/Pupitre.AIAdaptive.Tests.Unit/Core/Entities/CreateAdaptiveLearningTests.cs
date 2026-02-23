using System.Collections.Generic;
using System.Linq;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Domain.Events;
using Pupitre.AIAdaptive.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AIAdaptive.Tests.Unit.Core.Entities;

public class CreateAdaptiveLearningTests
{
    private AdaptiveLearning Act(EntityId id, string name, IEnumerable<string> tags) => AdaptiveLearning.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_adaptivelearning_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var adaptivelearning = Act(id, name, tags);
        
        // Assert
        adaptivelearning.ShouldNotBeNull();
        adaptivelearning.Id.ToString().ShouldBe(id.ToString());
        adaptivelearning.Tags.ShouldBe(tags);
        adaptivelearning.Events.Count().ShouldBe(1);

        var @event = adaptivelearning.Events.Single();
        @event.ShouldBeOfType<AdaptiveLearningCreated>();
    }

    [Fact]
    public void given_empty_tags_adaptivelearning_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAdaptiveLearningTagsException>();
    }
}