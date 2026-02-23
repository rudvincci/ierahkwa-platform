using Pupitre.Accessibility.Application.Commands;
using Pupitre.Accessibility.Application.Events;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using Pupitre.Accessibility.Tests.Shared.Factories;
using Pupitre.Accessibility.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Accessibility.Tests.Integration.Async;

public class AddAccessProfileTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAccessProfile command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_accessprofile_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAccessProfile(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AccessProfileAdded, AccessProfileDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "accessibility";
    private readonly MongoDbFixture<AccessProfileDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAccessProfileTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AccessProfileDocument, Guid>("accessibility");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}