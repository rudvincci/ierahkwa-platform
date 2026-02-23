using Pupitre.Assessments.Application.Commands;
using Pupitre.Assessments.Application.Events;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using Pupitre.Assessments.Tests.Shared.Factories;
using Pupitre.Assessments.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Assessments.Tests.Integration.Async;

public class AddAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAssessment command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_assessment_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAssessment(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AssessmentAdded, AssessmentDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "assessments";
    private readonly MongoDbFixture<AssessmentDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAssessmentTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AssessmentDocument, Guid>("assessments");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}