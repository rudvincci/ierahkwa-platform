using Pupitre.Curricula.Application.Commands;
using Pupitre.Curricula.Application.Events;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using Pupitre.Curricula.Tests.Shared.Factories;
using Pupitre.Curricula.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Curricula.Tests.Integration.Async;

public class AddCurriculumTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddCurriculum command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_curriculum_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddCurriculum(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<CurriculumAdded, CurriculumDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "curricula";
    private readonly MongoDbFixture<CurriculumDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddCurriculumTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<CurriculumDocument, Guid>("curricula");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}