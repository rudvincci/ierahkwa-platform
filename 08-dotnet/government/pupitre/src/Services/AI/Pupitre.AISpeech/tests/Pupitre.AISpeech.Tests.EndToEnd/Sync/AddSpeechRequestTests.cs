using System.Net;
using System.Text;
using Pupitre.AISpeech.Api;
using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Contracts.Commands;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using Pupitre.AISpeech.Tests.Shared.Factories;
using Pupitre.AISpeech.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AISpeech.Tests.EndToEnd.Sync
{
    public class AddSpeechRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddSpeechRequest command) 
            => _httpClient.PostAsync("aispeech", GetContent(command));

        [Fact]
        public async Task add_speechrequest_endpoint_should_return_http_status_code_created()
        {
            var command = new AddSpeechRequest(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_speechrequest_endpoint_should_return_location_header_with_correct_speechrequest_id()
        {
            var command = new AddSpeechRequest(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aispeech/{command.Id}");
        }

        [Fact]
        public async Task add_speechrequest_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddSpeechRequest(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<SpeechRequestDocument, Guid> _mongoDbFixture;
        
        public AddSpeechRequestTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<SpeechRequestDocument, Guid>("aispeech");
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