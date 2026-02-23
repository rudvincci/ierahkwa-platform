using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Application.Events;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using Pupitre.Operations.Tests.Shared.Factories;
using Pupitre.Operations.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Operations.Tests.Integration.Async;

public class AddOperationMetricTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddOperationMetric command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_operationmetric_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddOperationMetric(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<OperationMetricAdded, OperationMetricDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "operations";
    private readonly MongoDbFixture<OperationMetricDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddOperationMetricTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<OperationMetricDocument, Guid>("operations");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}