using System.Net;
using Pupitre.AISafety.Api;
using Pupitre.AISafety.Application.DTO;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using Pupitre.AISafety.Tests.Shared.Factories;
using Pupitre.AISafety.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AISafety.Tests.EndToEnd.Sync;

public class GetSafetyCheckTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aisafety/{_safetycheckId}");

    [Fact]
    public async Task get_safetycheck_endpoint_should_return_not_found_status_code_if_safetycheck_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_safetycheck_endpoint_should_return_dto_with_correct_data()
    {
        await InsertSafetyCheckAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<SafetyCheckDto>(content);
        
        dto.Id.ShouldBe(_safetycheckId);
    }
    
    #region Arrange

    private readonly Guid _safetycheckId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<SafetyCheckDocument, Guid> _mongoDbFixture;

    private Task InsertSafetyCheckAsync()
        => _mongoDbFixture.InsertAsync(new SafetyCheckDocument
        {
            Id = _safetycheckId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetSafetyCheckTests(MameyApplicationFactory<Program> factory)
    {
        _safetycheckId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<SafetyCheckDocument, Guid>("aisafety");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}