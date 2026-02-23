using System.Net;
using Pupitre.Rewards.Api;
using Pupitre.Rewards.Application.DTO;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using Pupitre.Rewards.Tests.Shared.Factories;
using Pupitre.Rewards.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Rewards.Tests.EndToEnd.Sync;

public class GetRewardTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"rewards/{_rewardId}");

    [Fact]
    public async Task get_reward_endpoint_should_return_not_found_status_code_if_reward_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_reward_endpoint_should_return_dto_with_correct_data()
    {
        await InsertRewardAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<RewardDto>(content);
        
        dto.Id.ShouldBe(_rewardId);
    }
    
    #region Arrange

    private readonly Guid _rewardId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<RewardDocument, Guid> _mongoDbFixture;

    private Task InsertRewardAsync()
        => _mongoDbFixture.InsertAsync(new RewardDocument
        {
            Id = _rewardId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetRewardTests(MameyApplicationFactory<Program> factory)
    {
        _rewardId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<RewardDocument, Guid>("rewards");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}