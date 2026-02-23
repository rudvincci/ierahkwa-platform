using System.Net;
using Pupitre.AISpeech.Api;
using Pupitre.AISpeech.Application.DTO;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using Pupitre.AISpeech.Tests.Shared.Factories;
using Pupitre.AISpeech.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AISpeech.Tests.EndToEnd.Sync;

public class GetSpeechRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aispeech/{_speechrequestId}");

    [Fact]
    public async Task get_speechrequest_endpoint_should_return_not_found_status_code_if_speechrequest_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_speechrequest_endpoint_should_return_dto_with_correct_data()
    {
        await InsertSpeechRequestAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<SpeechRequestDto>(content);
        
        dto.Id.ShouldBe(_speechrequestId);
    }
    
    #region Arrange

    private readonly Guid _speechrequestId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<SpeechRequestDocument, Guid> _mongoDbFixture;

    private Task InsertSpeechRequestAsync()
        => _mongoDbFixture.InsertAsync(new SpeechRequestDocument
        {
            Id = _speechrequestId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetSpeechRequestTests(MameyApplicationFactory<Program> factory)
    {
        _speechrequestId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<SpeechRequestDocument, Guid>("aispeech");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}