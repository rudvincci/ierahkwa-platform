using System.Net;
using System.Text;
using Pupitre.Bookstore.Api;
using Pupitre.Bookstore.Application.Commands;
using Pupitre.Bookstore.Contracts.Commands;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using Pupitre.Bookstore.Tests.Shared.Factories;
using Pupitre.Bookstore.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Bookstore.Tests.EndToEnd.Sync
{
    public class AddBookTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddBook command) 
            => _httpClient.PostAsync("bookstore", GetContent(command));

        [Fact]
        public async Task add_book_endpoint_should_return_http_status_code_created()
        {
            var command = new AddBook(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task add_book_endpoint_should_return_location_header_with_correct_book_id()
        {
            var command = new AddBook(Guid.NewGuid(), "name", new []{"tag"});

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"bookstore/{command.Id}");
        }

        [Fact]
        public async Task add_book_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddBook(Guid.NewGuid(), "name", new []{"tag"});

            await Act(command);

            var document = await _mongoDbFixture.GetAsync(command.Id);
            
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.Id);
            document.Tags.ShouldBe(command.Tags);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<BookDocument, Guid> _mongoDbFixture;
        
        public AddBookTests(MameyApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<BookDocument, Guid>("bookstore");
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