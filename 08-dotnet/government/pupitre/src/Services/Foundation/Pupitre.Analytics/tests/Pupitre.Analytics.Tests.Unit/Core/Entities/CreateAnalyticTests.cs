using System.Collections.Generic;
using System.Linq;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Domain.Events;
using Pupitre.Analytics.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Analytics.Tests.Unit.Core.Entities;

public class CreateAnalyticTests
{
    private Analytic Act(EntityId id, string name, IEnumerable<string> tags) => Analytic.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_analytic_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var analytic = Act(id, name, tags);
        
        // Assert
        analytic.ShouldNotBeNull();
        analytic.Id.ToString().ShouldBe(id.ToString());
        analytic.Tags.ShouldBe(tags);
        analytic.Events.Count().ShouldBe(1);

        var @event = analytic.Events.Single();
        @event.ShouldBeOfType<AnalyticCreated>();
    }

    [Fact]
    public void given_empty_tags_analytic_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAnalyticTagsException>();
    }
}