using System.Collections.Generic;
using System.Linq;
using Pupitre.Rewards.Domain.Entities;
using Pupitre.Rewards.Domain.Events;
using Pupitre.Rewards.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Rewards.Tests.Unit.Core.Entities;

public class CreateRewardTests
{
    private Reward Act(EntityId id, string name, IEnumerable<string> tags) => Reward.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_reward_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var reward = Act(id, name, tags);
        
        // Assert
        reward.ShouldNotBeNull();
        reward.Id.ToString().ShouldBe(id.ToString());
        reward.Tags.ShouldBe(tags);
        reward.Events.Count().ShouldBe(1);

        var @event = reward.Events.Single();
        @event.ShouldBeOfType<RewardCreated>();
    }

    [Fact]
    public void given_empty_tags_reward_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingRewardTagsException>();
    }
}