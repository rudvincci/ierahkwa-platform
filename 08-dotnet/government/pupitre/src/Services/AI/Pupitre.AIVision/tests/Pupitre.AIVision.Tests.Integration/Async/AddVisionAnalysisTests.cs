using Pupitre.AIVision.Application.Commands;
using Pupitre.AIVision.Application.Events;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using Pupitre.AIVision.Tests.Shared.Factories;
using Pupitre.AIVision.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIVision.Tests.Integration.Async;

public class AddVisionAnalysisTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddVisionAnalysis command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_visionanalysis_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddVisionAnalysis(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<VisionAnalysisAdded, VisionAnalysisDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aivision";
    private readonly MongoDbFixture<VisionAnalysisDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddVisionAnalysisTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<VisionAnalysisDocument, Guid>("aivision");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}