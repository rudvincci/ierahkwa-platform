using System.Net;
using System.Text;
using Pupitre.AIBehavior.Api;
using Pupitre.AIBehavior.Application.Commands;
using Pupitre.AIBehavior.Contracts.Commands;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using Pupitre.AIBehavior.Tests.Shared.Factories;
using Pupitre.AIBehavior.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIBehavior.Tests.EndToEnd.Sync
{
    public class AddBehaviorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddBehavior command) 
            => _httpClient.PostAsync("aibehavior", GetContent(command));

        [Fact]
        public async Task add_behavior_endpoint_should_return_http_status_code_created()
        {
            var command = new AddBehavior(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_behavior_endpoint_should_return_location_header_with_correct_behavior_id()
        {
            var command = new AddBehavior(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aibehavior/{command.Id}");
        }

        [Fact]
        public async Task add_behavior_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddBehavior(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<BehaviorDocument, Guid> _mongoDbFixture;
        
        public AddBehaviorTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<BehaviorDocument, Guid>("aibehavior");
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