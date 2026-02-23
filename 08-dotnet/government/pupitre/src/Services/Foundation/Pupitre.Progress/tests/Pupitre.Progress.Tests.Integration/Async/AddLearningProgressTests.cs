using Pupitre.Progress.Application.Commands;
using Pupitre.Progress.Application.Events;
using Pupitre.Progress.Contracts.Commands;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using Pupitre.Progress.Tests.Shared.Factories;
using Pupitre.Progress.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Progress.Tests.Integration.Async;

public class AddLearningProgressTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddLearningProgress command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_learningprogress_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddLearningProgress(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<LearningProgressAdded, LearningProgressDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "progress";
    private readonly MongoDbFixture<LearningProgressDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddLearningProgressTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<LearningProgressDocument, Guid>("progress");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}