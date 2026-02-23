using Pupitre.Educators.Application.Commands;
using Pupitre.Educators.Application.Events;
using Pupitre.Educators.Contracts.Commands;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using Pupitre.Educators.Tests.Shared.Factories;
using Pupitre.Educators.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Educators.Tests.Integration.Async;

public class AddEducatorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddEducator command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_educator_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddEducator(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<EducatorAdded, EducatorDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "educators";
    private readonly MongoDbFixture<EducatorDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddEducatorTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<EducatorDocument, Guid>("educators");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}