using Pupitre.Compliance.Application.Commands;
using Pupitre.Compliance.Application.Events;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using Pupitre.Compliance.Tests.Shared.Factories;
using Pupitre.Compliance.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Compliance.Tests.Integration.Async;

public class AddComplianceRecordTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddComplianceRecord command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_compliancerecord_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddComplianceRecord(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<ComplianceRecordAdded, ComplianceRecordDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "compliance";
    private readonly MongoDbFixture<ComplianceRecordDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddComplianceRecordTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<ComplianceRecordDocument, Guid>("compliance");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}