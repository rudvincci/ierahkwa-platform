using System.Net;
using Pupitre.Progress.Api;
using Pupitre.Progress.Application.DTO;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using Pupitre.Progress.Tests.Shared.Factories;
using Pupitre.Progress.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Progress.Tests.EndToEnd.Sync;

public class GetLearningProgressTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"progress/{_learningprogressId}");

    [Fact]
    public async Task get_learningprogress_endpoint_should_return_not_found_status_code_if_learningprogress_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_learningprogress_endpoint_should_return_dto_with_correct_data()
    {
        await InsertLearningProgressAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<LearningProgressDto>(content);
        
        dto.Id.ShouldBe(_learningprogressId);
    }
    
    #region Arrange

    private readonly Guid _learningprogressId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<LearningProgressDocument, Guid> _mongoDbFixture;

    private Task InsertLearningProgressAsync()
        => _mongoDbFixture.InsertAsync(new LearningProgressDocument
        {
            Id = _learningprogressId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetLearningProgressTests(MameyApplicationFactory<Program> factory)
    {
        _learningprogressId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<LearningProgressDocument, Guid>("progress");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}