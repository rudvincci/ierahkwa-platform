using Pupitre.Parents.Application.Commands;
using Pupitre.Parents.Application.Events;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using Pupitre.Parents.Tests.Shared.Factories;
using Pupitre.Parents.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Parents.Tests.Integration.Async;

public class AddParentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddParent command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_parent_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddParent(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<ParentAdded, ParentDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "parents";
    private readonly MongoDbFixture<ParentDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddParentTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<ParentDocument, Guid>("parents");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}