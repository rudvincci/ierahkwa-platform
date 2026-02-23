using System.Collections.Generic;
using System.Linq;
using Pupitre.Fundraising.Domain.Entities;
using Pupitre.Fundraising.Domain.Events;
using Pupitre.Fundraising.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Fundraising.Tests.Unit.Core.Entities;

public class CreateCampaignTests
{
    private Campaign Act(EntityId id, string name, IEnumerable<string> tags) => Campaign.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_campaign_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var campaign = Act(id, name, tags);
        
        // Assert
        campaign.ShouldNotBeNull();
        campaign.Id.ToString().ShouldBe(id.ToString());
        campaign.Tags.ShouldBe(tags);
        campaign.Events.Count().ShouldBe(1);

        var @event = campaign.Events.Single();
        @event.ShouldBeOfType<CampaignCreated>();
    }

    [Fact]
    public void given_empty_tags_campaign_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingCampaignTagsException>();
    }
}