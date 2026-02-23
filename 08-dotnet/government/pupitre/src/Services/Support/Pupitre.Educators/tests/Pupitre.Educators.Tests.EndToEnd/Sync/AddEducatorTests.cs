using System.Net;
using System.Text;
using Pupitre.Educators.Api;
using Pupitre.Educators.Application.Commands;
using Pupitre.Educators.Contracts.Commands;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using Pupitre.Educators.Tests.Shared.Factories;
using Pupitre.Educators.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Educators.Tests.EndToEnd.Sync
{
    public class AddEducatorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddEducator command) 
            => _httpClient.PostAsync("educators", GetContent(command));

        [Fact]
        public async Task add_educator_endpoint_should_return_http_status_code_created()
        {
            var command = new AddEducator(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_educator_endpoint_should_return_location_header_with_correct_educator_id()
        {
            var command = new AddEducator(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"educators/{command.Id}");
        }

        [Fact]
        public async Task add_educator_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddEducator(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<EducatorDocument, Guid> _mongoDbFixture;
        
        public AddEducatorTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<EducatorDocument, Guid>("educators");
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