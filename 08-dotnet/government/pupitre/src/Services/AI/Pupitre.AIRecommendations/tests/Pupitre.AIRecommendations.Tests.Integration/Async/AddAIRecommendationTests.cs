using Pupitre.AIRecommendations.Application.Commands;
using Pupitre.AIRecommendations.Application.Events;
using Pupitre.AIRecommendations.Contracts.Commands;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;
using Pupitre.AIRecommendations.Tests.Shared.Factories;
using Pupitre.AIRecommendations.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIRecommendations.Tests.Integration.Async;

public class AddAIRecommendationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAIRecommendation command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_airecommendation_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAIRecommendation(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AIRecommendationAdded, AIRecommendationDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "airecommendations";
    private readonly MongoDbFixture<AIRecommendationDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAIRecommendationTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AIRecommendationDocument, Guid>("airecommendations");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}