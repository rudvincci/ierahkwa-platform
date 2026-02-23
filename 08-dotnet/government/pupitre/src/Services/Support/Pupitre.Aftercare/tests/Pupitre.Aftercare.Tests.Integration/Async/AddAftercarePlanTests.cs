using Pupitre.Aftercare.Application.Commands;
using Pupitre.Aftercare.Application.Events;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using Pupitre.Aftercare.Tests.Shared.Factories;
using Pupitre.Aftercare.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Aftercare.Tests.Integration.Async;

public class AddAftercarePlanTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddAftercarePlan command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_aftercareplan_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddAftercarePlan(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<AftercarePlanAdded, AftercarePlanDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "aftercare";
    private readonly MongoDbFixture<AftercarePlanDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddAftercarePlanTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<AftercarePlanDocument, Guid>("aftercare");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}