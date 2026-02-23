using System.Collections.Generic;
using System.Linq;
using Pupitre.AITranslation.Domain.Entities;
using Pupitre.AITranslation.Domain.Events;
using Pupitre.AITranslation.Domain.Exceptions;
using Mamey.Types;
using Shouldly;
using Xunit;

namespace Pupitre.AITranslation.Tests.Unit.Core.Entities;

public class CreateTranslationRequestTests
{
    private TranslationRequest Act(EntityId id, string name, IEnumerable<string> tags) => TranslationRequest.Create(id, name, tags);

    [Fact]
    public void given_valid_id_and_tags_translationrequest_should_be_created()
    {
        // Arrange
        var id = new EntityId(Guid.NewGuid());
        var name = "name";
        var tags = new[] {"tag"};

        // Act
        var translationrequest = Act(id, name, tags);
        
        // Assert
        translationrequest.ShouldNotBeNull();
        translationrequest.Id.ToString().ShouldBe(id.ToString());
        translationrequest.Tags.ShouldBe(tags);
        translationrequest.Events.Count().ShouldBe(1);

        var @event = translationrequest.Events.Single();
        @event.ShouldBeOfType<TranslationRequestCreated>();
    }

    [Fact]
    public void given_empty_tags_translationrequest_should_throw_an_exception()
    {
        var id = new EntityId(Guid.NewGuid());
        var tags = Enumerable.Empty<string>();
        var name = "name";

        var exception = Record.Exception(() => Act(id, name, tags));
        
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<MissingTranslationRequestTagsException>();
    }
}