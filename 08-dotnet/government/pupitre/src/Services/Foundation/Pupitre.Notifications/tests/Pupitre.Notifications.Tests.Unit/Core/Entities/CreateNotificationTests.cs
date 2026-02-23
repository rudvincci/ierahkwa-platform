using System.Collections.Generic;
using System.Linq;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Domain.Events;
using Pupitre.Notifications.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Notifications.Tests.Unit.Core.Entities;

public class CreateNotificationTests
{
    private Notification Act(EntityId id, string name, IEnumerable<string> tags) => Notification.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_notification_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var notification = Act(id, name, tags);
        
        // Assert
        notification.ShouldNotBeNull();
        notification.Id.ToString().ShouldBe(id.ToString());
        notification.Tags.ShouldBe(tags);
        notification.Events.Count().ShouldBe(1);

        var @event = notification.Events.Single();
        @event.ShouldBeOfType<NotificationCreated>();
    }

    [Fact]
    public void given_empty_tags_notification_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingNotificationTagsException>();
    }
}