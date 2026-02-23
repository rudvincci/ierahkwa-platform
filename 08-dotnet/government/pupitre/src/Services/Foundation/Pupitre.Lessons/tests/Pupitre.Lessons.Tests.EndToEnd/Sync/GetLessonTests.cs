using System.Net;
using Pupitre.Lessons.Api;
using Pupitre.Lessons.Application.DTO;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;
using Pupitre.Lessons.Tests.Shared.Factories;
using Pupitre.Lessons.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Lessons.Tests.EndToEnd.Sync;

public class GetLessonTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"lessons/{_lessonId}");

    [Fact]
    public async Task get_lesson_endpoint_should_return_not_found_status_code_if_lesson_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_lesson_endpoint_should_return_dto_with_correct_data()
    {
        await InsertLessonAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<LessonDto>(content);
        
        dto.Id.ShouldBe(_lessonId);
    }
    
    #region Arrange

    private readonly Guid _lessonId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<LessonDocument, Guid> _mongoDbFixture;

    private Task InsertLessonAsync()
        => _mongoDbFixture.InsertAsync(new LessonDocument
        {
            Id = _lessonId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetLessonTests(MameyApplicationFactory<Program> factory)
    {
        _lessonId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<LessonDocument, Guid>("lessons");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}