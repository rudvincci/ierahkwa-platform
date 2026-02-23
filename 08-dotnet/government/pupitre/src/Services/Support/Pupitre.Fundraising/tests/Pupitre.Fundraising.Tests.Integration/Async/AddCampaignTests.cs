using Pupitre.Fundraising.Application.Commands;
using Pupitre.Fundraising.Application.Events;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using Pupitre.Fundraising.Tests.Shared.Factories;
using Pupitre.Fundraising.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Fundraising.Tests.Integration.Async;

public class AddCampaignTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddCampaign command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_campaign_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddCampaign(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<CampaignAdded, CampaignDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "fundraising";
    private readonly MongoDbFixture<CampaignDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddCampaignTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<CampaignDocument, Guid>("fundraising");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}