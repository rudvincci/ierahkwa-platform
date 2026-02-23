using Pupitre.AITranslation.Application.Commands;
using Pupitre.AITranslation.Application.Events;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using Pupitre.AITranslation.Tests.Shared.Factories;
using Pupitre.AITranslation.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AITranslation.Tests.Integration.Async;

public class AddTranslationRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddTranslationRequest command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_translationrequest_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddTranslationRequest(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<TranslationRequestAdded, TranslationRequestDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aitranslation";
    private readonly MongoDbFixture<TranslationRequestDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddTranslationRequestTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<TranslationRequestDocument, Guid>("aitranslation");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}