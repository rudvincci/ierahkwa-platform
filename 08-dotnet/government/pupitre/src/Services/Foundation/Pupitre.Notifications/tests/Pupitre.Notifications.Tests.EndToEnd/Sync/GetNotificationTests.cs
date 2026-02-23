using System.Net;
using Pupitre.Notifications.Api;
using Pupitre.Notifications.Application.DTO;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using Pupitre.Notifications.Tests.Shared.Factories;
using Pupitre.Notifications.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Notifications.Tests.EndToEnd.Sync;

public class GetNotificationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"notifications/{_notificationId}");

    [Fact]
    public async Task get_notification_endpoint_should_return_not_found_status_code_if_notification_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_notification_endpoint_should_return_dto_with_correct_data()
    {
        await InsertNotificationAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<NotificationDto>(content);
        
        dto.Id.ShouldBe(_notificationId);
    }
    
    #region Arrange

    private readonly Guid _notificationId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<NotificationDocument, Guid> _mongoDbFixture;

    private Task InsertNotificationAsync()
        => _mongoDbFixture.InsertAsync(new NotificationDocument
        {
            Id = _notificationId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetNotificationTests(MameyApplicationFactory<Program> factory)
    {
        _notificationId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<NotificationDocument, Guid>("notifications");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}