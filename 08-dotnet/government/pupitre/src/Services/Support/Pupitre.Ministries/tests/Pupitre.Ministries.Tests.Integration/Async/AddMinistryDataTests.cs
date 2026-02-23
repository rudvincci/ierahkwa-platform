using Pupitre.Ministries.Application.Commands;
using Pupitre.Ministries.Application.Events;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using Pupitre.Ministries.Tests.Shared.Factories;
using Pupitre.Ministries.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Ministries.Tests.Integration.Async;

public class AddMinistryDataTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddMinistryData command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_ministrydata_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddMinistryData(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<MinistryDataAdded, MinistryDataDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "ministries";
    private readonly MongoDbFixture<MinistryDataDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddMinistryDataTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<MinistryDataDocument, Guid>("ministries");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}