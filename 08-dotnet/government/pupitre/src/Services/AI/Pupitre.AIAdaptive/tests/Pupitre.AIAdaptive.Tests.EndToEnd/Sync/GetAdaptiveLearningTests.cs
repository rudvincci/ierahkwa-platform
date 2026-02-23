using System.Net;
using Pupitre.AIAdaptive.Api;
using Pupitre.AIAdaptive.Application.DTO;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using Pupitre.AIAdaptive.Tests.Shared.Factories;
using Pupitre.AIAdaptive.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIAdaptive.Tests.EndToEnd.Sync;

public class GetAdaptiveLearningTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aiadaptive/{_adaptivelearningId}");

    [Fact]
    public async Task get_adaptivelearning_endpoint_should_return_not_found_status_code_if_adaptivelearning_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_adaptivelearning_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAdaptiveLearningAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AdaptiveLearningDto>(content);
        
        dto.Id.ShouldBe(_adaptivelearningId);
    }
    
    #region Arrange

    private readonly Guid _adaptivelearningId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AdaptiveLearningDocument, Guid> _mongoDbFixture;

    private Task InsertAdaptiveLearningAsync()
        => _mongoDbFixture.InsertAsync(new AdaptiveLearningDocument
        {
            Id = _adaptivelearningId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAdaptiveLearningTests(MameyApplicationFactory<Program> factory)
    {
        _adaptivelearningId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AdaptiveLearningDocument, Guid>("aiadaptive");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}