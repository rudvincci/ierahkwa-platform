using System.Net;
using Pupitre.Accessibility.Api;
using Pupitre.Accessibility.Application.DTO;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using Pupitre.Accessibility.Tests.Shared.Factories;
using Pupitre.Accessibility.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Accessibility.Tests.EndToEnd.Sync;

public class GetAccessProfileTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"accessibility/{_accessprofileId}");

    [Fact]
    public async Task get_accessprofile_endpoint_should_return_not_found_status_code_if_accessprofile_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_accessprofile_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAccessProfileAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AccessProfileDto>(content);
        
        dto.Id.ShouldBe(_accessprofileId);
    }
    
    #region Arrange

    private readonly Guid _accessprofileId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AccessProfileDocument, Guid> _mongoDbFixture;

    private Task InsertAccessProfileAsync()
        => _mongoDbFixture.InsertAsync(new AccessProfileDocument
        {
            Id = _accessprofileId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAccessProfileTests(MameyApplicationFactory<Program> factory)
    {
        _accessprofileId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AccessProfileDocument, Guid>("accessibility");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}