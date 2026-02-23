using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Application.Events;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using Pupitre.AISpeech.Tests.Shared.Factories;
using Pupitre.AISpeech.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AISpeech.Tests.Integration.Async;

public class AddSpeechRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddSpeechRequest command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_speechrequest_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddSpeechRequest(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<SpeechRequestAdded, SpeechRequestDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aispeech";
    private readonly MongoDbFixture<SpeechRequestDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddSpeechRequestTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<SpeechRequestDocument, Guid>("aispeech");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}