using System.Net;
using System.Text;
using Pupitre.Progress.Api;
using Pupitre.Progress.Application.Commands;
using Pupitre.Progress.Contracts.Commands;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using Pupitre.Progress.Tests.Shared.Factories;
using Pupitre.Progress.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Progress.Tests.EndToEnd.Sync
{
    public class AddLearningProgressTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddLearningProgress command) 
            => _httpClient.PostAsync("progress", GetContent(command));

        [Fact]
        public async Task add_learningprogress_endpoint_should_return_http_status_code_created()
        {
            var command = new AddLearningProgress(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_learningprogress_endpoint_should_return_location_header_with_correct_learningprogress_id()
        {
            var command = new AddLearningProgress(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"progress/{command.Id}");
        }

        [Fact]
        public async Task add_learningprogress_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddLearningProgress(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<LearningProgressDocument, Guid> _mongoDbFixture;
        
        public AddLearningProgressTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<LearningProgressDocument, Guid>("progress");
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