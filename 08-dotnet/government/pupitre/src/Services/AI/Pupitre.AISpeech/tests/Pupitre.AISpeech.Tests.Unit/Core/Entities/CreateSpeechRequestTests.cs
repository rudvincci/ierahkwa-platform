using System.Collections.Generic;
using System.Linq;
using Pupitre.AISpeech.Domain.Entities;
using Pupitre.AISpeech.Domain.Events;
using Pupitre.AISpeech.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AISpeech.Tests.Unit.Core.Entities;

public class CreateSpeechRequestTests
{
    private SpeechRequest Act(EntityId id, string name, IEnumerable<string> tags) => SpeechRequest.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_speechrequest_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var speechrequest = Act(id, name, tags);
        
        // Assert
        speechrequest.ShouldNotBeNull();
        speechrequest.Id.ToString().ShouldBe(id.ToString());
        speechrequest.Tags.ShouldBe(tags);
        speechrequest.Events.Count().ShouldBe(1);

        var @event = speechrequest.Events.Single();
        @event.ShouldBeOfType<SpeechRequestCreated>();
    }

    [Fact]
    public void given_empty_tags_speechrequest_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingSpeechRequestTagsException>();
    }
}