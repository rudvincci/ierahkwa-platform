using System.Net;
using Pupitre.Assessments.Api;
using Pupitre.Assessments.Application.DTO;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using Pupitre.Assessments.Tests.Shared.Factories;
using Pupitre.Assessments.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Assessments.Tests.EndToEnd.Sync;

public class GetAssessmentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"assessments/{_assessmentId}");

    [Fact]
    public async Task get_assessment_endpoint_should_return_not_found_status_code_if_assessment_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_assessment_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAssessmentAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AssessmentDto>(content);
        
        dto.Id.ShouldBe(_assessmentId);
    }
    
    #region Arrange

    private readonly Guid _assessmentId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AssessmentDocument, Guid> _mongoDbFixture;

    private Task InsertAssessmentAsync()
        => _mongoDbFixture.InsertAsync(new AssessmentDocument
        {
            Id = _assessmentId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAssessmentTests(MameyApplicationFactory<Program> factory)
    {
        _assessmentId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AssessmentDocument, Guid>("assessments");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}