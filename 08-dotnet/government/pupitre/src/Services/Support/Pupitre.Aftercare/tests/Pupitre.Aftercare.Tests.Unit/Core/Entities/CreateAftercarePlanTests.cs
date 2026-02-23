using System.Collections.Generic;
using System.Linq;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Domain.Events;
using Pupitre.Aftercare.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Aftercare.Tests.Unit.Core.Entities;

public class CreateAftercarePlanTests
{
    private AftercarePlan Act(EntityId id, string name, IEnumerable<string> tags) => AftercarePlan.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_aftercareplan_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var aftercareplan = Act(id, name, tags);
        
        // Assert
        aftercareplan.ShouldNotBeNull();
        aftercareplan.Id.ToString().ShouldBe(id.ToString());
        aftercareplan.Tags.ShouldBe(tags);
        aftercareplan.Events.Count().ShouldBe(1);

        var @event = aftercareplan.Events.Single();
        @event.ShouldBeOfType<AftercarePlanCreated>();
    }

    [Fact]
    public void given_empty_tags_aftercareplan_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAftercarePlanTagsException>();
    }
}