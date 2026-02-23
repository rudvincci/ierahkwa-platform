using System.Collections.Generic;
using System.Linq;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Domain.Events;
using Pupitre.Ministries.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Ministries.Tests.Unit.Core.Entities;

public class CreateMinistryDataTests
{
    private MinistryData Act(EntityId id, string name, IEnumerable<string> tags) => MinistryData.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_ministrydata_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var ministrydata = Act(id, name, tags);
        
        // Assert
        ministrydata.ShouldNotBeNull();
        ministrydata.Id.ToString().ShouldBe(id.ToString());
        ministrydata.Tags.ShouldBe(tags);
        ministrydata.Events.Count().ShouldBe(1);

        var @event = ministrydata.Events.Single();
        @event.ShouldBeOfType<MinistryDataCreated>();
    }

    [Fact]
    public void given_empty_tags_ministrydata_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingMinistryDataTagsException>();
    }
}