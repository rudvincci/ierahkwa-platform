using System.Collections.Generic;
using System.Linq;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Domain.Events;
using Pupitre.Operations.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Operations.Tests.Unit.Core.Entities;

public class CreateOperationMetricTests
{
    private OperationMetric Act(EntityId id, string name, IEnumerable<string> tags) => OperationMetric.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_operationmetric_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var operationmetric = Act(id, name, tags);
        
        // Assert
        operationmetric.ShouldNotBeNull();
        operationmetric.Id.ToString().ShouldBe(id.ToString());
        operationmetric.Tags.ShouldBe(tags);
        operationmetric.Events.Count().ShouldBe(1);

        var @event = operationmetric.Events.Single();
        @event.ShouldBeOfType<OperationMetricCreated>();
    }

    [Fact]
    public void given_empty_tags_operationmetric_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingOperationMetricTagsException>();
    }
}