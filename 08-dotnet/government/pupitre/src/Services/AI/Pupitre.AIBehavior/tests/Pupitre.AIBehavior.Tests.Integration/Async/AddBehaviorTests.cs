using Pupitre.AIBehavior.Application.Commands;
using Pupitre.AIBehavior.Application.Events;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using Pupitre.AIBehavior.Tests.Shared.Factories;
using Pupitre.AIBehavior.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIBehavior.Tests.Integration.Async;

public class AddBehaviorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddBehavior command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_behavior_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddBehavior(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<BehaviorAdded, BehaviorDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aibehavior";
    private readonly MongoDbFixture<BehaviorDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddBehaviorTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<BehaviorDocument, Guid>("aibehavior");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}