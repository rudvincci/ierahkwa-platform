using Pupitre.AIContent.Application.Commands;
using Pupitre.AIContent.Application.Events;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using Pupitre.AIContent.Tests.Shared.Factories;
using Pupitre.AIContent.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIContent.Tests.Integration.Async;

public class AddContentGenerationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddContentGeneration command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_contentgeneration_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddContentGeneration(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<ContentGenerationAdded, ContentGenerationDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aicontent";
    private readonly MongoDbFixture<ContentGenerationDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddContentGenerationTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<ContentGenerationDocument, Guid>("aicontent");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}