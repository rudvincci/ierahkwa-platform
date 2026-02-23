using System.Net;
using Pupitre.AIAssessments.Api;
using Pupitre.AIAssessments.Application.DTO;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using Pupitre.AIAssessments.Tests.Shared.Factories;
using Pupitre.AIAssessments.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIAssessments.Tests.EndToEnd.Sync;

public class GetAIAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aiassessments/{_aiassessmentId}");

    [Fact]
    public async Task get_aiassessment_endpoint_should_return_not_found_status_code_if_aiassessment_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_aiassessment_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAIAssessmentAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AIAssessmentDto>(content);
        
        dto.Id.ShouldBe(_aiassessmentId);
    }
    
    #region Arrange

    private readonly Guid _aiassessmentId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AIAssessmentDocument, Guid> _mongoDbFixture;

    private Task InsertAIAssessmentAsync()
        => _mongoDbFixture.InsertAsync(new AIAssessmentDocument
        {
            Id = _aiassessmentId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAIAssessmentTests(MameyApplicationFactory<Program> factory)
    {
        _aiassessmentId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AIAssessmentDocument, Guid>("aiassessments");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}