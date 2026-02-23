using System.Collections.Generic;
using System.Linq;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Events;
using Pupitre.AIAssessments.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AIAssessments.Tests.Unit.Core.Entities;

public class CreateAIAssessmentTests
{
    private AIAssessment Act(EntityId id, string name, IEnumerable<string> tags) => AIAssessment.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_aiassessment_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var aiassessment = Act(id, name, tags);
        
        // Assert
        aiassessment.ShouldNotBeNull();
        aiassessment.Id.ToString().ShouldBe(id.ToString());
        aiassessment.Tags.ShouldBe(tags);
        aiassessment.Events.Count().ShouldBe(1);

        var @event = aiassessment.Events.Single();
        @event.ShouldBeOfType<AIAssessmentCreated>();
    }

    [Fact]
    public void given_empty_tags_aiassessment_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAIAssessmentTagsException>();
    }
}