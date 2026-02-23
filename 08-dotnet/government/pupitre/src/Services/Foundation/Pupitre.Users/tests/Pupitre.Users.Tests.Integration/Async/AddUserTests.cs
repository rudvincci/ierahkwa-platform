using Pupitre.Users.Application.Commands;
using Pupitre.Users.Application.Events;
using Pupitre.Users.Contracts.Commands;
using Pupitre.Users.Infrastructure.Mongo.Documents;
using Pupitre.Users.Tests.Shared.Factories;
using Pupitre.Users.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Users.Tests.Integration.Async;

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

    private const string Exchange = "users";
    private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddUserTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}