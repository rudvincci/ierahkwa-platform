using System.Collections.Generic;
using System.Linq;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Domain.Events;
using Pupitre.AIVision.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AIVision.Tests.Unit.Core.Entities;

public class CreateVisionAnalysisTests
{
    private VisionAnalysis Act(EntityId id, string name, IEnumerable<string> tags) => VisionAnalysis.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_visionanalysis_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var visionanalysis = Act(id, name, tags);
        
        // Assert
        visionanalysis.ShouldNotBeNull();
        visionanalysis.Id.ToString().ShouldBe(id.ToString());
        visionanalysis.Tags.ShouldBe(tags);
        visionanalysis.Events.Count().ShouldBe(1);

        var @event = visionanalysis.Events.Single();
        @event.ShouldBeOfType<VisionAnalysisCreated>();
    }

    [Fact]
    public void given_empty_tags_visionanalysis_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingVisionAnalysisTagsException>();
    }
}