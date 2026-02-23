using Pupitre.AISafety.Application.Commands;
using Pupitre.AISafety.Application.Events;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using Pupitre.AISafety.Tests.Shared.Factories;
using Pupitre.AISafety.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AISafety.Tests.Integration.Async;

public class AddSafetyCheckTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddSafetyCheck command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_safetycheck_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddSafetyCheck(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<SafetyCheckAdded, SafetyCheckDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aisafety";
    private readonly MongoDbFixture<SafetyCheckDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddSafetyCheckTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<SafetyCheckDocument, Guid>("aisafety");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}