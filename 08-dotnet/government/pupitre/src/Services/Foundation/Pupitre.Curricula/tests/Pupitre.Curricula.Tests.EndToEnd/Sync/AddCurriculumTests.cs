using System.Net;
using System.Text;
using Pupitre.Curricula.Api;
using Pupitre.Curricula.Application.Commands;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using Pupitre.Curricula.Tests.Shared.Factories;
using Pupitre.Curricula.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Curricula.Tests.EndToEnd.Sync
{
    public class AddCurriculumTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddCurriculum command) 
            => _httpClient.PostAsync("curricula", GetContent(command));

        [Fact]
        public async Task add_curriculum_endpoint_should_return_http_status_code_created()
        {
            var command = new AddCurriculum(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_curriculum_endpoint_should_return_location_header_with_correct_curriculum_id()
        {
            var command = new AddCurriculum(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"curricula/{command.Id}");
        }

        [Fact]
        public async Task add_curriculum_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddCurriculum(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<CurriculumDocument, Guid> _mongoDbFixture;
        
        public AddCurriculumTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<CurriculumDocument, Guid>("curricula");
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