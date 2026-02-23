using System.Net;
using System.Text;
using Pupitre.Aftercare.Api;
using Pupitre.Aftercare.Application.Commands;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using Pupitre.Aftercare.Tests.Shared.Factories;
using Pupitre.Aftercare.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Aftercare.Tests.EndToEnd.Sync
{
    public class AddAftercarePlanTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddAftercarePlan command) 
            => _httpClient.PostAsync("aftercare", GetContent(command));

        [Fact]
        public async Task add_aftercareplan_endpoint_should_return_http_status_code_created()
        {
            var command = new AddAftercarePlan(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_aftercareplan_endpoint_should_return_location_header_with_correct_aftercareplan_id()
        {
            var command = new AddAftercarePlan(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"aftercare/{command.Id}");
        }

        [Fact]
        public async Task add_aftercareplan_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddAftercarePlan(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<AftercarePlanDocument, Guid> _mongoDbFixture;
        
        public AddAftercarePlanTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<AftercarePlanDocument, Guid>("aftercare");
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