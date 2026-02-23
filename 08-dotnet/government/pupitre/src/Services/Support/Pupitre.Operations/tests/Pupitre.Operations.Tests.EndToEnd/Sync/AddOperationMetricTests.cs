using System.Net;
using System.Text;
using Pupitre.Operations.Api;
using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using Pupitre.Operations.Tests.Shared.Factories;
using Pupitre.Operations.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Operations.Tests.EndToEnd.Sync
{
    public class AddOperationMetricTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddOperationMetric command) 
            => _httpClient.PostAsync("operations", GetContent(command));

        [Fact]
        public async Task add_operationmetric_endpoint_should_return_http_status_code_created()
        {
            var command = new AddOperationMetric(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_operationmetric_endpoint_should_return_location_header_with_correct_operationmetric_id()
        {
            var command = new AddOperationMetric(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"operations/{command.Id}");
        }

        [Fact]
        public async Task add_operationmetric_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddOperationMetric(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<OperationMetricDocument, Guid> _mongoDbFixture;
        
        public AddOperationMetricTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<OperationMetricDocument, Guid>("operations");
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