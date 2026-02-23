using System.Net;
using System.Text;
using Pupitre.AIAdaptive.Api;
using Pupitre.AIAdaptive.Application.Commands;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using Pupitre.AIAdaptive.Tests.Shared.Factories;
using Pupitre.AIAdaptive.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIAdaptive.Tests.EndToEnd.Sync
{
    public class AddAdaptiveLearningTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAdaptiveLearning command) 
            => _httpClient.PostAsync("aiadaptive", GetContent(command));

        [Fact]
        public async Task add_adaptivelearning_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAdaptiveLearning(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_adaptivelearning_endpoint_should_return_location_header_with_correct_adaptivelearning_id()
        {
            var command = new AddAdaptiveLearning(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aiadaptive/{command.Id}");
        }

        [Fact]
        public async Task add_adaptivelearning_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAdaptiveLearning(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AdaptiveLearningDocument, Guid> _mongoDbFixture;
        
        public AddAdaptiveLearningTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AdaptiveLearningDocument, Guid>("aiadaptive");
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