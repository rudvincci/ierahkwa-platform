using System.Collections.Generic;
using System.Linq;
using Pupitre.Lessons.Domain.Entities;
using Pupitre.Lessons.Domain.Events;
using Pupitre.Lessons.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Lessons.Tests.Unit.Core.Entities;

public class CreateLessonTests
{
    private Lesson Act(EntityId id, string name, IEnumerable<string> tags) => Lesson.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_lesson_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var lesson = Act(id, name, tags);
        
        // Assert
        lesson.ShouldNotBeNull();
        lesson.Id.ToString().ShouldBe(id.ToString());
        lesson.Tags.ShouldBe(tags);
        lesson.Events.Count().ShouldBe(1);

        var @event = lesson.Events.Single();
        @event.ShouldBeOfType<LessonCreated>();
    }

    [Fact]
    public void given_empty_tags_lesson_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingLessonTagsException>();
    }
}