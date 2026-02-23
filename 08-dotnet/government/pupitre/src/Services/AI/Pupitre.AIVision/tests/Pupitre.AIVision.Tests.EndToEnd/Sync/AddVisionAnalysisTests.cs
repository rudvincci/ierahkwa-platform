using System.Net;
using System.Text;
using Pupitre.AIVision.Api;
using Pupitre.AIVision.Application.Commands;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using Pupitre.AIVision.Tests.Shared.Factories;
using Pupitre.AIVision.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIVision.Tests.EndToEnd.Sync
{
    public class AddVisionAnalysisTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddVisionAnalysis command) 
            => _httpClient.PostAsync("aivision", GetContent(command));

        [Fact]
        public async Task add_visionanalysis_endpoint_should_return_http_status_code_created()
        {
            var command = new AddVisionAnalysis(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_visionanalysis_endpoint_should_return_location_header_with_correct_visionanalysis_id()
        {
            var command = new AddVisionAnalysis(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aivision/{command.Id}");
        }

        [Fact]
        public async Task add_visionanalysis_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddVisionAnalysis(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<VisionAnalysisDocument, Guid> _mongoDbFixture;
        
        public AddVisionAnalysisTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<VisionAnalysisDocument, Guid>("aivision");
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