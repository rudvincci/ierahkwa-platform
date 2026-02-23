using System.Net;
using Pupitre.AIVision.Api;
using Pupitre.AIVision.Application.DTO;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using Pupitre.AIVision.Tests.Shared.Factories;
using Pupitre.AIVision.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIVision.Tests.EndToEnd.Sync;

public class GetVisionAnalysisTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aivision/{_visionanalysisId}");

    [Fact]
    public async Task get_visionanalysis_endpoint_should_return_not_found_status_code_if_visionanalysis_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_visionanalysis_endpoint_should_return_dto_with_correct_data()
    {
        await InsertVisionAnalysisAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<VisionAnalysisDto>(content);
        
        dto.Id.ShouldBe(_visionanalysisId);
    }
    
    #region Arrange

    private readonly Guid _visionanalysisId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<VisionAnalysisDocument, Guid> _mongoDbFixture;

    private Task InsertVisionAnalysisAsync()
        => _mongoDbFixture.InsertAsync(new VisionAnalysisDocument
        {
            Id = _visionanalysisId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetVisionAnalysisTests(MameyApplicationFactory<Program> factory)
    {
        _visionanalysisId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<VisionAnalysisDocument, Guid>("aivision");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}