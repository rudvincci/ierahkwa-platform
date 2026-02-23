using System.Net;
using Pupitre.Ministries.Api;
using Pupitre.Ministries.Application.DTO;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using Pupitre.Ministries.Tests.Shared.Factories;
using Pupitre.Ministries.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Ministries.Tests.EndToEnd.Sync;

public class GetMinistryDataTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"ministries/{_ministrydataId}");

    [Fact]
    public async Task get_ministrydata_endpoint_should_return_not_found_status_code_if_ministrydata_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_ministrydata_endpoint_should_return_dto_with_correct_data()
    {
        await InsertMinistryDataAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<MinistryDataDto>(content);
        
        dto.Id.ShouldBe(_ministrydataId);
    }
    
    #region Arrange

    private readonly Guid _ministrydataId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<MinistryDataDocument, Guid> _mongoDbFixture;

    private Task InsertMinistryDataAsync()
        => _mongoDbFixture.InsertAsync(new MinistryDataDocument
        {
            Id = _ministrydataId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetMinistryDataTests(MameyApplicationFactory<Program> factory)
    {
        _ministrydataId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<MinistryDataDocument, Guid>("ministries");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}