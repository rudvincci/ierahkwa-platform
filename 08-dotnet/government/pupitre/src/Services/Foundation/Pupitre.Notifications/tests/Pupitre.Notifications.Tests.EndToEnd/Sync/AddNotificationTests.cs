using System.Net;
using System.Text;
using Pupitre.Notifications.Api;
using Pupitre.Notifications.Application.Commands;
using Pupitre.Notifications.Contracts.Commands;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using Pupitre.Notifications.Tests.Shared.Factories;
using Pupitre.Notifications.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Notifications.Tests.EndToEnd.Sync
{
    public class AddNotificationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddNotification command) 
            => _httpClient.PostAsync("notifications", GetContent(command));

        [Fact]
        public async Task add_notification_endpoint_should_return_http_status_code_created()
        {
            var command = new AddNotification(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_notification_endpoint_should_return_location_header_with_correct_notification_id()
        {
            var command = new AddNotification(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"notifications/{command.Id}");
        }

        [Fact]
        public async Task add_notification_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddNotification(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<NotificationDocument, Guid> _mongoDbFixture;
        
        public AddNotificationTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<NotificationDocument, Guid>("notifications");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }
        
        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }
        
        private static StringContent GetContent(object value) 
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
        
        #endregion
    }
}