using System.Net;
using Pupitre.AITutors.Api;
using Pupitre.AITutors.Application.DTO;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using Pupitre.AITutors.Tests.Shared.Factories;
using Pupitre.AITutors.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AITutors.Tests.EndToEnd.Sync;

public class GetTutorTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aitutors/{_tutorId}");

    [Fact]
    public async Task get_tutor_endpoint_should_return_not_found_status_code_if_tutor_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_tutor_endpoint_should_return_dto_with_correct_data()
    {
        await InsertTutorAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<TutorDto>(content);
        
        dto.Id.ShouldBe(_tutorId);
    }
    
    #region Arrange

    private readonly Guid _tutorId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<TutorDocument, Guid> _mongoDbFixture;

    private Task InsertTutorAsync()
        => _mongoDbFixture.InsertAsync(new TutorDocument
        {
            Id = _tutorId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetTutorTests(MameyApplicationFactory<Program> factory)
    {
        _tutorId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<TutorDocument, Guid>("aitutors");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}