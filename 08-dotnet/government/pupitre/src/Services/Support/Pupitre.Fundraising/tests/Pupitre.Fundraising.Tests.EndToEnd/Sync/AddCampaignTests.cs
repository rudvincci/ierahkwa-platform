using System.Net;
using System.Text;
using Pupitre.Fundraising.Api;
using Pupitre.Fundraising.Application.Commands;
using Pupitre.Fundraising.Contracts.Commands;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using Pupitre.Fundraising.Tests.Shared.Factories;
using Pupitre.Fundraising.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Fundraising.Tests.EndToEnd.Sync
{
    public class AddCampaignTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddCampaign command) 
            => _httpClient.PostAsync("fundraising", GetContent(command));

        [Fact]
        public async Task add_campaign_endpoint_should_return_http_status_code_created()
        {
            var command = new AddCampaign(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_campaign_endpoint_should_return_location_header_with_correct_campaign_id()
        {
            var command = new AddCampaign(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"fundraising/{command.Id}");
        }

        [Fact]
        public async Task add_campaign_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddCampaign(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<CampaignDocument, Guid> _mongoDbFixture;
        
        public AddCampaignTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<CampaignDocument, Guid>("fundraising");
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