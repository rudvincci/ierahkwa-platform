using System.Net;
using System.Text;
using Pupitre.Accessibility.Api;
using Pupitre.Accessibility.Application.Commands;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using Pupitre.Accessibility.Tests.Shared.Factories;
using Pupitre.Accessibility.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Accessibility.Tests.EndToEnd.Sync
{
    public class AddAccessProfileTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAccessProfile command) 
            => _httpClient.PostAsync("accessibility", GetContent(command));

        [Fact]
        public async Task add_accessprofile_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAccessProfile(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_accessprofile_endpoint_should_return_location_header_with_correct_accessprofile_id()
        {
            var command = new AddAccessProfile(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"accessibility/{command.Id}");
        }

        [Fact]
        public async Task add_accessprofile_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAccessProfile(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AccessProfileDocument, Guid> _mongoDbFixture;
        
        public AddAccessProfileTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AccessProfileDocument, Guid>("accessibility");
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