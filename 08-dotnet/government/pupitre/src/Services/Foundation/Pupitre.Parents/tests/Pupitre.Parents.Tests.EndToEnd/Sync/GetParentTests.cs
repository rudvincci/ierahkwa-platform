using System.Net;
using Pupitre.Parents.Api;
using Pupitre.Parents.Application.DTO;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using Pupitre.Parents.Tests.Shared.Factories;
using Pupitre.Parents.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Parents.Tests.EndToEnd.Sync;

public class GetParentTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"parents/{_parentId}");

    [Fact]
    public async Task get_parent_endpoint_should_return_not_found_status_code_if_parent_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_parent_endpoint_should_return_dto_with_correct_data()
    {
        await InsertParentAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<ParentDto>(content);
        
        dto.Id.ShouldBe(_parentId);
    }
    
    #region Arrange

    private readonly Guid _parentId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<ParentDocument, Guid> _mongoDbFixture;

    private Task InsertParentAsync()
        => _mongoDbFixture.InsertAsync(new ParentDocument
        {
            Id = _parentId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetParentTests(MameyApplicationFactory<Program> factory)
    {
        _parentId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<ParentDocument, Guid>("parents");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}