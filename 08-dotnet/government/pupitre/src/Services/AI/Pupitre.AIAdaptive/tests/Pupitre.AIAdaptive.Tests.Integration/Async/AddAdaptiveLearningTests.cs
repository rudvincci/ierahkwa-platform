using Pupitre.AIAdaptive.Application.Commands;
using Pupitre.AIAdaptive.Application.Events;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using Pupitre.AIAdaptive.Tests.Shared.Factories;
using Pupitre.AIAdaptive.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIAdaptive.Tests.Integration.Async;

public class AddAdaptiveLearningTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAdaptiveLearning command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_adaptivelearning_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAdaptiveLearning(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AdaptiveLearningAdded, AdaptiveLearningDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aiadaptive";
    private readonly MongoDbFixture<AdaptiveLearningDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAdaptiveLearningTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AdaptiveLearningDocument, Guid>("aiadaptive");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}