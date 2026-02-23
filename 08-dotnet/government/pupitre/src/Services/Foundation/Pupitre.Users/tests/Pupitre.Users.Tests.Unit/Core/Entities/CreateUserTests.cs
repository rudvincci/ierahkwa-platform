using System.Collections.Generic;
using System.Linq;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Domain.Events;
using Pupitre.Users.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Users.Tests.Unit.Core.Entities;

public class CreateUserTests
{
    private User Act(EntityId id, string name, IEnumerable<string> tags) => User.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_user_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var user = Act(id, name, tags);
        
        // Assert
        user.ShouldNotBeNull();
        user.Id.ToString().ShouldBe(id.ToString());
        user.Tags.ShouldBe(tags);
        user.Events.Count().ShouldBe(1);

        var @event = user.Events.Single();
        @event.ShouldBeOfType<UserCreated>();
    }

    [Fact]
    public void given_empty_tags_user_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingUserTagsException>();
    }
}