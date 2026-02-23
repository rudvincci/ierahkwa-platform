using System.Net;
using System.Text;
using Pupitre.Compliance.Api;
using Pupitre.Compliance.Application.Commands;
using Pupitre.Compliance.Contracts.Commands;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using Pupitre.Compliance.Tests.Shared.Factories;
using Pupitre.Compliance.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Compliance.Tests.EndToEnd.Sync
{
    public class AddComplianceRecordTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddComplianceRecord command) 
            => _httpClient.PostAsync("compliance", GetContent(command));

        [Fact]
        public async Task add_compliancerecord_endpoint_should_return_http_status_code_created()
        {
            var command = new AddComplianceRecord(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_compliancerecord_endpoint_should_return_location_header_with_correct_compliancerecord_id()
        {
            var command = new AddComplianceRecord(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"compliance/{command.Id}");
        }

        [Fact]
        public async Task add_compliancerecord_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddComplianceRecord(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ComplianceRecordDocument, Guid> _mongoDbFixture;
        
        public AddComplianceRecordTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ComplianceRecordDocument, Guid>("compliance");
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