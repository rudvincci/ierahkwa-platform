using System.Net;
using Pupitre.Fundraising.Api;
using Pupitre.Fundraising.Application.DTO;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using Pupitre.Fundraising.Tests.Shared.Factories;
using Pupitre.Fundraising.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Fundraising.Tests.EndToEnd.Sync;

public class GetCampaignTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"fundraising/{_campaignId}");

    [Fact]
    public async Task get_campaign_endpoint_should_return_not_found_status_code_if_campaign_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_campaign_endpoint_should_return_dto_with_correct_data()
    {
        await InsertCampaignAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<CampaignDto>(content);
        
        dto.Id.ShouldBe(_campaignId);
    }
    
    #region Arrange

    private readonly Guid _campaignId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<CampaignDocument, Guid> _mongoDbFixture;

    private Task InsertCampaignAsync()
        => _mongoDbFixture.InsertAsync(new CampaignDocument
        {
            Id = _campaignId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetCampaignTests(MameyApplicationFactory<Program> factory)
    {
        _campaignId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<CampaignDocument, Guid>("fundraising");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}