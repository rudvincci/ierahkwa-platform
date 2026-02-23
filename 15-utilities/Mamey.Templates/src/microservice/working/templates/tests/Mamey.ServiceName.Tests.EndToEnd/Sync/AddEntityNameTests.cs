using System.Net;
using System.Text;
using Mamey.ServiceName.Api;
using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Contracts.Commands;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using Mamey.ServiceName.Tests.Shared.Factories;
using Mamey.ServiceName.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mamey.ServiceName.Tests.EndToEnd.Sync
{
    public class AddEntityNameTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddEntityName command) 
            => _httpClient.PostAsync("servicename", GetContent(command));

        [Fact]
        public async Task add_entityname_endpoint_should_return_http_status_code_created()
        {
            var command = new AddEntityName(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_entityname_endpoint_should_return_location_header_with_correct_entityname_id()
        {
            var command = new AddEntityName(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"servicename/{command.Id}");
        }

        [Fact]
        public async Task add_entityname_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddEntityName(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<EntityNameDocument, Guid> _mongoDbFixture;
        
        public AddEntityNameTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<EntityNameDocument, Guid>("servicename");
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