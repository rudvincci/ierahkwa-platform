using System.Net;
using System.Text;
using Pupitre.AIContent.Api;
using Pupitre.AIContent.Application.Commands;
using Pupitre.AIContent.Contracts.Commands;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using Pupitre.AIContent.Tests.Shared.Factories;
using Pupitre.AIContent.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIContent.Tests.EndToEnd.Sync
{
    public class AddContentGenerationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddContentGeneration command) 
            => _httpClient.PostAsync("aicontent", GetContent(command));

        [Fact]
        public async Task add_contentgeneration_endpoint_should_return_http_status_code_created()
        {
            var command = new AddContentGeneration(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_contentgeneration_endpoint_should_return_location_header_with_correct_contentgeneration_id()
        {
            var command = new AddContentGeneration(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aicontent/{command.Id}");
        }

        [Fact]
        public async Task add_contentgeneration_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddContentGeneration(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ContentGenerationDocument, Guid> _mongoDbFixture;
        
        public AddContentGenerationTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ContentGenerationDocument, Guid>("aicontent");
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