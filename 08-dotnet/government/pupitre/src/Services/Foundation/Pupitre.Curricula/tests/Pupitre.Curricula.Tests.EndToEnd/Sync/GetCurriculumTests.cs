using System.Net;
using Pupitre.Curricula.Api;
using Pupitre.Curricula.Application.DTO;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using Pupitre.Curricula.Tests.Shared.Factories;
using Pupitre.Curricula.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Curricula.Tests.EndToEnd.Sync;

public class GetCurriculumTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"curricula/{_curriculumId}");

    [Fact]
    public async Task get_curriculum_endpoint_should_return_not_found_status_code_if_curriculum_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_curriculum_endpoint_should_return_dto_with_correct_data()
    {
        await InsertCurriculumAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<CurriculumDto>(content);
        
        dto.Id.ShouldBe(_curriculumId);
    }
    
    #region Arrange

    private readonly Guid _curriculumId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<CurriculumDocument, Guid> _mongoDbFixture;

    private Task InsertCurriculumAsync()
        => _mongoDbFixture.InsertAsync(new CurriculumDocument
        {
            Id = _curriculumId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetCurriculumTests(MameyApplicationFactory<Program> factory)
    {
        _curriculumId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<CurriculumDocument, Guid>("curricula");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}