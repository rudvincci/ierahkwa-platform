using Pupitre.AIAssessments.Application.Commands;
using Pupitre.AIAssessments.Application.Events;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using Pupitre.AIAssessments.Tests.Shared.Factories;
using Pupitre.AIAssessments.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.AIAssessments.Tests.Integration.Async;

public class AddAIAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAIAssessment command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_aiassessment_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAIAssessment(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AIAssessmentAdded, AIAssessmentDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aiassessments";
    private readonly MongoDbFixture<AIAssessmentDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAIAssessmentTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AIAssessmentDocument, Guid>("aiassessments");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}