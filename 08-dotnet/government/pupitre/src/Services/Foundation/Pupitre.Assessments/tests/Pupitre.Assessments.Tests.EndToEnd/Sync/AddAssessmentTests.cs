using System.Net;
using System.Text;
using Pupitre.Assessments.Api;
using Pupitre.Assessments.Application.Commands;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using Pupitre.Assessments.Tests.Shared.Factories;
using Pupitre.Assessments.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Assessments.Tests.EndToEnd.Sync
{
    public class AddAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAssessment command) 
            => _httpClient.PostAsync("assessments", GetContent(command));

        [Fact]
        public async Task add_assessment_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAssessment(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_assessment_endpoint_should_return_location_header_with_correct_assessment_id()
        {
            var command = new AddAssessment(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"assessments/{command.Id}");
        }

        [Fact]
        public async Task add_assessment_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAssessment(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AssessmentDocument, Guid> _mongoDbFixture;
        
        public AddAssessmentTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AssessmentDocument, Guid>("assessments");
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