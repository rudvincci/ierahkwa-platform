using System.Net;
using System.Text;
using Mamey.Government.Identity.Api;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Government.Identity.Tests.Shared.Factories;
using Mamey.Government.Identity.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mamey.Government.Identity.Tests.EndToEnd.Sync
{
    public class AddUserTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddUser command) 
            => _httpClient.PostAsync("identity", GetContent(command));

        [Fact]
        public async Task add_user_endpoint_should_return_http_status_code_created()
        {
            var command = new AddUser(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_user_endpoint_should_return_location_header_with_correct_user_id()
        {
            var command = new AddUser(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"identity/{command.Id}");
        }

        [Fact]
        public async Task add_user_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddUser(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
        
        public AddUserTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("identity");
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