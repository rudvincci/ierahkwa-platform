using Pupitre.Notifications.Application.Commands;
using Pupitre.Notifications.Application.Events;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using Pupitre.Notifications.Tests.Shared.Factories;
using Pupitre.Notifications.Tests.Shared.Fixtures;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shouldly;
using Xunit;

namespace Pupitre.Notifications.Tests.Integration.Async;

public class AddNotificationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task Act(AddNotification command) => _rabbitMqFixture.PublishAsync(command, Exchange);

    [Fact]
    public async Task add_notification_command_should_add_document_with_given_id_to_database()
    {
        var command = new AddNotification(Guid.NewGuid(), "name", new[] {"tag"});

        var tcs = await _rabbitMqFixture
            .SubscribeAndGetAsync<NotificationAdded, NotificationDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.Id);

        await Act(command);

        var document = await tcs.Task;
        
        document.ShouldNotBeNull();
        document.Id.ShouldBe(command.Id);
        document.Tags.ShouldBe(command.Tags);
    }
    
    #region Arrange

    private const string Exchange = "notifications";
    private readonly MongoDbFixture<NotificationDocument, Guid> _mongoDbFixture;
    private readonly RabbitMqFixture _rabbitMqFixture;
    
    public AddNotificationTests(MameyApplicationFactory<Program> factory)
    {
        _rabbitMqFixture = new RabbitMqFixture();
        _mongoDbFixture = new MongoDbFixture<NotificationDocument, Guid>("notifications");
        factory.Server.AllowSynchronousIO = true;
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }   

    #endregion
}