using System.Collections.Generic;
using System.Linq;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Events;
using Pupitre.AISafety.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AISafety.Tests.Unit.Core.Entities;

public class CreateSafetyCheckTests
{
    private SafetyCheck Act(EntityId id, string name, IEnumerable<string> tags) => SafetyCheck.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_safetycheck_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var safetycheck = Act(id, name, tags);
        
        // Assert
        safetycheck.ShouldNotBeNull();
        safetycheck.Id.ToString().ShouldBe(id.ToString());
        safetycheck.Tags.ShouldBe(tags);
        safetycheck.Events.Count().ShouldBe(1);

        var @event = safetycheck.Events.Single();
        @event.ShouldBeOfType<SafetyCheckCreated>();
    }

    [Fact]
    public void given_empty_tags_safetycheck_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingSafetyCheckTagsException>();
    }
}