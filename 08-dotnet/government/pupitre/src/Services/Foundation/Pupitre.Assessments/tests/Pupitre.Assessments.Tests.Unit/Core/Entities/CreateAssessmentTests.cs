using System.Collections.Generic;
using System.Linq;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Events;
using Pupitre.Assessments.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.Assessments.Tests.Unit.Core.Entities;

public class CreateAssessmentTests
{
    private Assessment Act(EntityId id, string name, IEnumerable<string> tags) => Assessment.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_assessment_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var assessment = Act(id, name, tags);
        
        // Assert
        assessment.ShouldNotBeNull();
        assessment.Id.ToString().ShouldBe(id.ToString());
        assessment.Tags.ShouldBe(tags);
        assessment.Events.Count().ShouldBe(1);

        var @event = assessment.Events.Single();
        @event.ShouldBeOfType<AssessmentCreated>();
    }

    [Fact]
    public void given_empty_tags_assessment_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingAssessmentTagsException>();
    }
}