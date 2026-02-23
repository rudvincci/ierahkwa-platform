using System.Net;
using System.Text;
using Pupitre.AITranslation.Api;
using Pupitre.AITranslation.Application.Commands;
using Pupitre.AITranslation.Contracts.Commands;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using Pupitre.AITranslation.Tests.Shared.Factories;
using Pupitre.AITranslation.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AITranslation.Tests.EndToEnd.Sync
{
    public class AddTranslationRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddTranslationRequest command) 
            => _httpClient.PostAsync("aitranslation", GetContent(command));

        [Fact]
        public async Task add_translationrequest_endpoint_should_return_http_status_code_created()
        {
            var command = new AddTranslationRequest(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_translationrequest_endpoint_should_return_location_header_with_correct_translationrequest_id()
        {
            var command = new AddTranslationRequest(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aitranslation/{command.Id}");
        }

        [Fact]
        public async Task add_translationrequest_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddTranslationRequest(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<TranslationRequestDocument, Guid> _mongoDbFixture;
        
        public AddTranslationRequestTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<TranslationRequestDocument, Guid>("aitranslation");
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