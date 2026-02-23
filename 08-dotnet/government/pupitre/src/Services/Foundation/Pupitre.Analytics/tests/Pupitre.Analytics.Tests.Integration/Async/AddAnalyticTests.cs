using Pupitre.Analytics.Application.Commands;
using Pupitre.Analytics.Application.Events;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using Pupitre.Analytics.Tests.Shared.Factories;
using Pupitre.Analytics.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Analytics.Tests.Integration.Async;

public class AddAnalyticTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAnalytic command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_analytic_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAnalytic(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AnalyticAdded, AnalyticDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "analytics";
    private readonly MongoDbFixture<AnalyticDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAnalyticTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AnalyticDocument, Guid>("analytics");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}