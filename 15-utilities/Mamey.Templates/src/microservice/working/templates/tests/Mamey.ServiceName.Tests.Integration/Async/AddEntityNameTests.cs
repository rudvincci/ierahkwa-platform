using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Application.Events;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using Mamey.ServiceName.Tests.Shared.Factories;
using Mamey.ServiceName.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Mamey.ServiceName.Tests.Integration.Async;

public class AddEntityNameTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddEntityName command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_entityname_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddEntityName(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<EntityNameAdded, EntityNameDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "servicename";
    private readonly MongoDbFixture<EntityNameDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddEntityNameTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<EntityNameDocument, Guid>("servicename");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}