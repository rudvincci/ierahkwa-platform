using System.Net;
using System.Text;
using Pupitre.Ministries.Api;
using Pupitre.Ministries.Application.Commands;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using Pupitre.Ministries.Tests.Shared.Factories;
using Pupitre.Ministries.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Ministries.Tests.EndToEnd.Sync
{
    public class AddMinistryDataTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddMinistryData command) 
            => _httpClient.PostAsync("ministries", GetContent(command));

        [Fact]
        public async Task add_ministrydata_endpoint_should_return_http_status_code_created()
        {
            var command = new AddMinistryData(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_ministrydata_endpoint_should_return_location_header_with_correct_ministrydata_id()
        {
            var command = new AddMinistryData(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"ministries/{command.Id}");
        }

        [Fact]
        public async Task add_ministrydata_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddMinistryData(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<MinistryDataDocument, Guid> _mongoDbFixture;
        
        public AddMinistryDataTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<MinistryDataDocument, Guid>("ministries");
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