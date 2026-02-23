using System.Net;
using System.Text;
using Pupitre.Rewards.Api;
using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Contracts.Commands;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using Pupitre.Rewards.Tests.Shared.Factories;
using Pupitre.Rewards.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Rewards.Tests.EndToEnd.Sync
{
    public class AddRewardTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddReward command) 
            => _httpClient.PostAsync("rewards", GetContent(command));

        [Fact]
        public async Task add_reward_endpoint_should_return_http_status_code_created()
        {
            var command = new AddReward(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_reward_endpoint_should_return_location_header_with_correct_reward_id()
        {
            var command = new AddReward(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"rewards/{command.Id}");
        }

        [Fact]
        public async Task add_reward_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddReward(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<RewardDocument, Guid> _mongoDbFixture;
        
        public AddRewardTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<RewardDocument, Guid>("rewards");
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