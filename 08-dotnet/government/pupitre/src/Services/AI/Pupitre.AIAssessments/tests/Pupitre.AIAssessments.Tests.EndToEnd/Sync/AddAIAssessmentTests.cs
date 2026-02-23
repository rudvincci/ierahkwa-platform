using System.Net;
using System.Text;
using Pupitre.AIAssessments.Api;
using Pupitre.AIAssessments.Application.Commands;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using Pupitre.AIAssessments.Tests.Shared.Factories;
using Pupitre.AIAssessments.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIAssessments.Tests.EndToEnd.Sync
{
    public class AddAIAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAIAssessment command) 
            => _httpClient.PostAsync("aiassessments", GetContent(command));

        [Fact]
        public async Task add_aiassessment_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAIAssessment(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_aiassessment_endpoint_should_return_location_header_with_correct_aiassessment_id()
        {
            var command = new AddAIAssessment(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aiassessments/{command.Id}");
        }

        [Fact]
        public async Task add_aiassessment_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAIAssessment(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AIAssessmentDocument, Guid> _mongoDbFixture;
        
        public AddAIAssessmentTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AIAssessmentDocument, Guid>("aiassessments");
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