using System.Net;
using Pupitre.AIContent.Api;
using Pupitre.AIContent.Application.DTO;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using Pupitre.AIContent.Tests.Shared.Factories;
using Pupitre.AIContent.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIContent.Tests.EndToEnd.Sync;

public class GetContentGenerationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aicontent/{_contentgenerationId}");

    [Fact]
    public async Task get_contentgeneration_endpoint_should_return_not_found_status_code_if_contentgeneration_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_contentgeneration_endpoint_should_return_dto_with_correct_data()
    {
        await InsertContentGenerationAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<ContentGenerationDto>(content);
        
        dto.Id.ShouldBe(_contentgenerationId);
    }
    
    #region Arrange

    private readonly Guid _contentgenerationId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<ContentGenerationDocument, Guid> _mongoDbFixture;

    private Task InsertContentGenerationAsync()
        => _mongoDbFixture.InsertAsync(new ContentGenerationDocument
        {
            Id = _contentgenerationId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetContentGenerationTests(MameyApplicationFactory<Program> factory)
    {
        _contentgenerationId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<ContentGenerationDocument, Guid>("aicontent");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}