using Pupitre.GLEs.Application.Commands;
using Pupitre.GLEs.Application.Events;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using Pupitre.GLEs.Tests.Shared.Factories;
using Pupitre.GLEs.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.GLEs.Tests.Integration.Async;

public class AddGLETests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddGLE command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_gle_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddGLE(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<GLEAdded, GLEDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "gles";
    private readonly MongoDbFixture<GLEDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddGLETests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<GLEDocument, Guid>("gles");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}