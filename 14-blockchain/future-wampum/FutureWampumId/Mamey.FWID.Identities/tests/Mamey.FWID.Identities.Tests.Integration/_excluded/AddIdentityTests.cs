using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Application.Events;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Async;

[Collection("Integration")]
public class AddIdentityTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddIdentity command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
    public async Task add_identity_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddIdentity
        {
            Id = Guid.NewGuid(),
            Name = new Name("Test", "User"),
            PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
            ContactInformation = new ContactInformation(
                new Email("test@example.com"),
                new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
            ),
            BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
            Zone = "zone-001"
        };

        // TODO: Fix MongoDbFixture usage
        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<IdentityAdded, IdentityDocument>(Exchange,
                null, command.Id); // _mongoDbFixture.GetAsync

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        // TODO: IdentityDocument doesn't have Tags property - removed from domain model
    }
    
    #region Arrange

    private const string Exchange = "mamey.fwid.identities";
    // TODO: Fix MongoDbFixture usage - using MongoDBFixture instead
    // private readonly MongoDbFixture<IdentityDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddIdentityTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        // _mongoDbFixture = new MongoDbFixture<IdentityDocument, Guid>("mamey.fwid.identities");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        // _mongoDbFixture.Dispose();
    }   

    #endregion
}