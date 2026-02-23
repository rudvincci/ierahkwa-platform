using System.Net;
using Pupitre.AIBehavior.Api;
using Pupitre.AIBehavior.Application.DTO;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using Pupitre.AIBehavior.Tests.Shared.Factories;
using Pupitre.AIBehavior.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AIBehavior.Tests.EndToEnd.Sync;

public class GetBehaviorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aibehavior/{_behaviorId}");

    [Fact]
    public async Task get_behavior_endpoint_should_return_not_found_status_code_if_behavior_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_behavior_endpoint_should_return_dto_with_correct_data()
    {
        await InsertBehaviorAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<BehaviorDto>(content);
        
        dto.Id.ShouldBe(_behaviorId);
    }
    
    #region Arrange

    private readonly Guid _behaviorId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<BehaviorDocument, Guid> _mongoDbFixture;

    private Task InsertBehaviorAsync()
        => _mongoDbFixture.InsertAsync(new BehaviorDocument
        {
            Id = _behaviorId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetBehaviorTests(MameyApplicationFactory<Program> factory)
    {
        _behaviorId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<BehaviorDocument, Guid>("aibehavior");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}