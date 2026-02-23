using System.Net;
using System.Text;
using Pupitre.Analytics.Api;
using Pupitre.Analytics.Application.Commands;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using Pupitre.Analytics.Tests.Shared.Factories;
using Pupitre.Analytics.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Analytics.Tests.EndToEnd.Sync
{
    public class AddAnalyticTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAnalytic command) 
            => _httpClient.PostAsync("analytics", GetContent(command));

        [Fact]
        public async Task add_analytic_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAnalytic(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_analytic_endpoint_should_return_location_header_with_correct_analytic_id()
        {
            var command = new AddAnalytic(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"analytics/{command.Id}");
        }

        [Fact]
        public async Task add_analytic_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAnalytic(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AnalyticDocument, Guid> _mongoDbFixture;
        
        public AddAnalyticTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AnalyticDocument, Guid>("analytics");
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