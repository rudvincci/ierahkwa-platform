using System.Net;
using Pupitre.AIRecommendations.Api;
using Pupitre.AIRecommendations.Application.DTO;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;
using Pupitre.AIRecommendations.Tests.Shared.Factories;
using Pupitre.AIRecommendations.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIRecommendations.Tests.EndToEnd.Sync;

public class GetAIRecommendationTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"airecommendations/{_airecommendationId}");

    [Fact]
    public async Task get_airecommendation_endpoint_should_return_not_found_status_code_if_airecommendation_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_airecommendation_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAIRecommendationAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AIRecommendationDto>(content);
        
        dto.Id.ShouldBe(_airecommendationId);
    }
    
    #region Arrange

    private readonly Guid _airecommendationId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AIRecommendationDocument, Guid> _mongoDbFixture;

    private Task InsertAIRecommendationAsync()
        => _mongoDbFixture.InsertAsync(new AIRecommendationDocument
        {
            Id = _airecommendationId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAIRecommendationTests(MameyApplicationFactory<Program> factory)
    {
        _airecommendationId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AIRecommendationDocument, Guid>("airecommendations");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}