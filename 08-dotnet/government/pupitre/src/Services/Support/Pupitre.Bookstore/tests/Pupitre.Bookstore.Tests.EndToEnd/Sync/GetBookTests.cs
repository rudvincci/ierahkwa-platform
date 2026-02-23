using System.Net;
using Pupitre.Bookstore.Api;
using Pupitre.Bookstore.Application.DTO;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using Pupitre.Bookstore.Tests.Shared.Factories;
using Pupitre.Bookstore.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Bookstore.Tests.EndToEnd.Sync;

public class GetBookTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"bookstore/{_bookId}");

    [Fact]
    public async Task get_book_endpoint_should_return_not_found_status_code_if_book_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_book_endpoint_should_return_dto_with_correct_data()
    {
        await InsertBookAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<BookDto>(content);
        
        dto.Id.ShouldBe(_bookId);
    }
    
    #region Arrange

    private readonly Guid _bookId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<BookDocument, Guid> _mongoDbFixture;

    private Task InsertBookAsync()
        => _mongoDbFixture.InsertAsync(new BookDocument
        {
            Id = _bookId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetBookTests(MameyApplicationFactory<Program> factory)
    {
        _bookId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<BookDocument, Guid>("bookstore");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}