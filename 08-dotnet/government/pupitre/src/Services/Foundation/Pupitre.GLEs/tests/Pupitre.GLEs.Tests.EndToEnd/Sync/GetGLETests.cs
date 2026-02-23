using System.Net;
using Pupitre.GLEs.Api;
using Pupitre.GLEs.Application.DTO;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using Pupitre.GLEs.Tests.Shared.Factories;
using Pupitre.GLEs.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.GLEs.Tests.EndToEnd.Sync;

public class GetGLETests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"gles/{_gleId}");

    [Fact]
    public async Task get_gle_endpoint_should_return_not_found_status_code_if_gle_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_gle_endpoint_should_return_dto_with_correct_data()
    {
        await InsertGLEAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<GLEDto>(content);
        
        dto.Id.ShouldBe(_gleId);
    }
    
    #region Arrange

    private readonly Guid _gleId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<GLEDocument, Guid> _mongoDbFixture;

    private Task InsertGLEAsync()
        => _mongoDbFixture.InsertAsync(new GLEDocument
        {
            Id = _gleId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetGLETests(MameyApplicationFactory<Program> factory)
    {
        _gleId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<GLEDocument, Guid>("gles");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}