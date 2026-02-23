using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Government.Identity.Tests.Shared.Factories;
using Mamey.Government.Identity.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Mamey.Government.Identity.Tests.Integration.Async;

public class AddUserTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddUser command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_user_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddUser(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<UserAdded, UserDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "identity";
    private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddUserTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("identity");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}