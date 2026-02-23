using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Application.Events;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using Pupitre.AITutors.Tests.Shared.Factories;
using Pupitre.AITutors.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AITutors.Tests.Integration.Async;

public class AddTutorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddTutor command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_tutor_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddTutor(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<TutorAdded, TutorDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aitutors";
    private readonly MongoDbFixture<TutorDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddTutorTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<TutorDocument, Guid>("aitutors");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}