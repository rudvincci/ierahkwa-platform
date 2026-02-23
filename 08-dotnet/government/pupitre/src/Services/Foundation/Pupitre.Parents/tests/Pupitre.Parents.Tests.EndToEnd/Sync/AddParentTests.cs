using System.Net;
using System.Text;
using Pupitre.Parents.Api;
using Pupitre.Parents.Application.Commands;
using Pupitre.Parents.Contracts.Commands;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using Pupitre.Parents.Tests.Shared.Factories;
using Pupitre.Parents.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Parents.Tests.EndToEnd.Sync
{
    public class AddParentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddParent command) 
            => _httpClient.PostAsync("parents", GetContent(command));

        [Fact]
        public async Task add_parent_endpoint_should_return_http_status_code_created()
        {
            var command = new AddParent(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_parent_endpoint_should_return_location_header_with_correct_parent_id()
        {
            var command = new AddParent(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"parents/{command.Id}");
        }

        [Fact]
        public async Task add_parent_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddParent(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ParentDocument, Guid> _mongoDbFixture;
        
        public AddParentTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ParentDocument, Guid>("parents");
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