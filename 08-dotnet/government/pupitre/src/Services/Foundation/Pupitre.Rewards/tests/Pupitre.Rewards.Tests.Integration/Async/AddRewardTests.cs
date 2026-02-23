using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Application.Events;
using Pupitre.Rewards.Contracts.Commands;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using Pupitre.Rewards.Tests.Shared.Factories;
using Pupitre.Rewards.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Rewards.Tests.Integration.Async;

public class AddRewardTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddReward command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_reward_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddReward(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<RewardAdded, RewardDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "rewards";
    private readonly MongoDbFixture<RewardDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddRewardTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<RewardDocument, Guid>("rewards");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}