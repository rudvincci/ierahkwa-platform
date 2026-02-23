using System.Net;
using Pupitre.Educators.Api;
using Pupitre.Educators.Application.DTO;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using Pupitre.Educators.Tests.Shared.Factories;
using Pupitre.Educators.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Educators.Tests.EndToEnd.Sync;

public class GetEducatorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"educators/{_educatorId}");

    [Fact]
    public async Task get_educator_endpoint_should_return_not_found_status_code_if_educator_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_educator_endpoint_should_return_dto_with_correct_data()
    {
        await InsertEducatorAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<EducatorDto>(content);
        
        dto.Id.ShouldBe(_educatorId);
    }
    
    #region Arrange

    private readonly Guid _educatorId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<EducatorDocument, Guid> _mongoDbFixture;

    private Task InsertEducatorAsync()
        => _mongoDbFixture.InsertAsync(new EducatorDocument
        {
            Id = _educatorId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetEducatorTests(MameyApplicationFactory<Program> factory)
    {
        _educatorId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<EducatorDocument, Guid>("educators");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}