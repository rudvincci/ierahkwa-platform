using System.Net;
using System.Text;
using Pupitre.AISafety.Api;
using Pupitre.AISafety.Application.Commands;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using Pupitre.AISafety.Tests.Shared.Factories;
using Pupitre.AISafety.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AISafety.Tests.EndToEnd.Sync
{
    public class AddSafetyCheckTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddSafetyCheck command) 
            => _httpClient.PostAsync("aisafety", GetContent(command));

        [Fact]
        public async Task add_safetycheck_endpoint_should_return_http_status_code_created()
        {
            var command = new AddSafetyCheck(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_safetycheck_endpoint_should_return_location_header_with_correct_safetycheck_id()
        {
            var command = new AddSafetyCheck(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aisafety/{command.Id}");
        }

        [Fact]
        public async Task add_safetycheck_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddSafetyCheck(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<SafetyCheckDocument, Guid> _mongoDbFixture;
        
        public AddSafetyCheckTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<SafetyCheckDocument, Guid>("aisafety");
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