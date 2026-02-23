using System.Net;
using Mamey.ServiceName.Api;
using Mamey.ServiceName.Application.DTO;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using Mamey.ServiceName.Tests.Shared.Factories;
using Mamey.ServiceName.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mamey.ServiceName.Tests.EndToEnd.Sync;

public class GetEntityNameTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"servicename/{_entitynameId}");

    [Fact]
    public async Task get_entityname_endpoint_should_return_not_found_status_code_if_entityname_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_entityname_endpoint_should_return_dto_with_correct_data()
    {
        await InsertEntityNameAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<EntityNameDto>(content);
        
        dto.Id.ShouldBe(_entitynameId);
    }
    
    #region Arrange

    private readonly Guid _entitynameId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<EntityNameDocument, Guid> _mongoDbFixture;

    private Task InsertEntityNameAsync()
        => _mongoDbFixture.InsertAsync(new EntityNameDocument
        {
            Id = _entitynameId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetEntityNameTests(MameyApplicationFactory<Program> factory)
    {
        _entitynameId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<EntityNameDocument, Guid>("servicename");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}