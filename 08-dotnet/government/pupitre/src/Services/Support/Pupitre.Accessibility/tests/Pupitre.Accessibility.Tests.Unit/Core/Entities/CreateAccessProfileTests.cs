using System.Collections.Generic;
using System.Linq;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Domain.Events;
using Pupitre.Accessibility.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Accessibility.Tests.Unit.Core.Entities;

public class CreateAccessProfileTests
{
    private AccessProfile Act(EntityId id, string name, IEnumerable<string> tags) => AccessProfile.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_accessprofile_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var accessprofile = Act(id, name, tags);
        
        // Assert
        accessprofile.ShouldNotBeNull();
        accessprofile.Id.ToString().ShouldBe(id.ToString());
        accessprofile.Tags.ShouldBe(tags);
        accessprofile.Events.Count().ShouldBe(1);

        var @event = accessprofile.Events.Single();
        @event.ShouldBeOfType<AccessProfileCreated>();
    }

    [Fact]
    public void given_empty_tags_accessprofile_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAccessProfileTagsException>();
    }
}