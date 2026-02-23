using System.Collections.Generic;
using System.Linq;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Domain.Events;
using Pupitre.Curricula.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Curricula.Tests.Unit.Core.Entities;

public class CreateCurriculumTests
{
    private Curriculum Act(EntityId id, string name, IEnumerable<string> tags) => Curriculum.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_curriculum_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var curriculum = Act(id, name, tags);
        
        // Assert
        curriculum.ShouldNotBeNull();
        curriculum.Id.ToString().ShouldBe(id.ToString());
        curriculum.Tags.ShouldBe(tags);
        curriculum.Events.Count().ShouldBe(1);

        var @event = curriculum.Events.Single();
        @event.ShouldBeOfType<CurriculumCreated>();
    }

    [Fact]
    public void given_empty_tags_curriculum_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingCurriculumTagsException>();
    }
}