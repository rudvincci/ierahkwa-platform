using System.Net;
using Pupitre.Analytics.Api;
using Pupitre.Analytics.Application.DTO;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using Pupitre.Analytics.Tests.Shared.Factories;
using Pupitre.Analytics.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Analytics.Tests.EndToEnd.Sync;

public class GetAnalyticTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"analytics/{_analyticId}");

    [Fact]
    public async Task get_analytic_endpoint_should_return_not_found_status_code_if_analytic_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_analytic_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAnalyticAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AnalyticDto>(content);
        
        dto.Id.ShouldBe(_analyticId);
    }
    
    #region Arrange

    private readonly Guid _analyticId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AnalyticDocument, Guid> _mongoDbFixture;

    private Task InsertAnalyticAsync()
        => _mongoDbFixture.InsertAsync(new AnalyticDocument
        {
            Id = _analyticId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAnalyticTests(MameyApplicationFactory<Program> factory)
    {
        _analyticId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AnalyticDocument, Guid>("analytics");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}