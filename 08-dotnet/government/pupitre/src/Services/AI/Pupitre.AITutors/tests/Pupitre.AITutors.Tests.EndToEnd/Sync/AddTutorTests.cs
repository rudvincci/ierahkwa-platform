using System.Net;
using System.Text;
using Pupitre.AITutors.Api;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using Pupitre.AITutors.Tests.Shared.Factories;
using Pupitre.AITutors.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AITutors.Tests.EndToEnd.Sync
{
    public class AddTutorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddTutor command) 
            => _httpClient.PostAsync("aitutors", GetContent(command));

        [Fact]
        public async Task add_tutor_endpoint_should_return_http_status_code_created()
        {
            var command = new AddTutor(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_tutor_endpoint_should_return_location_header_with_correct_tutor_id()
        {
            var command = new AddTutor(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aitutors/{command.Id}");
        }

        [Fact]
        public async Task add_tutor_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddTutor(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<TutorDocument, Guid> _mongoDbFixture;
        
        public AddTutorTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<TutorDocument, Guid>("aitutors");
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