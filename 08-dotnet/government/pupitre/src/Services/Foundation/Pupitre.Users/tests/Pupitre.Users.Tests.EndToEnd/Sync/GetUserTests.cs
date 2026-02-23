using System.Net;
using Pupitre.Users.Api;
using Pupitre.Users.Application.DTO;
using Pupitre.Users.Infrastructure.Mongo.Documents;
using Pupitre.Users.Tests.Shared.Factories;
using Pupitre.Users.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Users.Tests.EndToEnd.Sync;

public class GetUserTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"users/{_userId}");

    [Fact]
    public async Task get_user_endpoint_should_return_not_found_status_code_if_user_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_user_endpoint_should_return_dto_with_correct_data()
    {
        await InsertUserAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<UserDto>(content);
        
        dto.Id.ShouldBe(_userId);
    }
    
    #region Arrange

    private readonly Guid _userId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;

    private Task InsertUserAsync()
        => _mongoDbFixture.InsertAsync(new UserDocument
        {
            Id = _userId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetUserTests(MameyApplicationFactory<Program> factory)
    {
        _userId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}