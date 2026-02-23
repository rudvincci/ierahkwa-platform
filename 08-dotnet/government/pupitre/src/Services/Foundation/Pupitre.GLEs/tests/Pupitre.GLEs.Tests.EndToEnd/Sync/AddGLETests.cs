using System.Net;
using System.Text;
using Pupitre.GLEs.Api;
using Pupitre.GLEs.Application.Commands;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using Pupitre.GLEs.Tests.Shared.Factories;
using Pupitre.GLEs.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.GLEs.Tests.EndToEnd.Sync
{
    public class AddGLETests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddGLE command) 
            => _httpClient.PostAsync("gles", GetContent(command));

        [Fact]
        public async Task add_gle_endpoint_should_return_http_status_code_created()
        {
            var command = new AddGLE(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_gle_endpoint_should_return_location_header_with_correct_gle_id()
        {
            var command = new AddGLE(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"gles/{command.Id}");
        }

        [Fact]
        public async Task add_gle_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddGLE(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<GLEDocument, Guid> _mongoDbFixture;
        
        public AddGLETests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<GLEDocument, Guid>("gles");
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