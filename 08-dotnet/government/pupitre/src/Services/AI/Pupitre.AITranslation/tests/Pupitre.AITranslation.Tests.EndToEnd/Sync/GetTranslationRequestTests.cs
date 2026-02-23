using System.Net;
using Pupitre.AITranslation.Api;
using Pupitre.AITranslation.Application.DTO;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using Pupitre.AITranslation.Tests.Shared.Factories;
using Pupitre.AITranslation.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.AITranslation.Tests.EndToEnd.Sync;

public class GetTranslationRequestTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aitranslation/{_translationrequestId}");

    [Fact]
    public async Task get_translationrequest_endpoint_should_return_not_found_status_code_if_translationrequest_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_translationrequest_endpoint_should_return_dto_with_correct_data()
    {
        await InsertTranslationRequestAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<TranslationRequestDto>(content);
        
        dto.Id.ShouldBe(_translationrequestId);
    }
    
    #region Arrange

    private readonly Guid _translationrequestId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<TranslationRequestDocument, Guid> _mongoDbFixture;

    private Task InsertTranslationRequestAsync()
        => _mongoDbFixture.InsertAsync(new TranslationRequestDocument
        {
            Id = _translationrequestId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetTranslationRequestTests(MameyApplicationFactory<Program> factory)
    {
        _translationrequestId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<TranslationRequestDocument, Guid>("aitranslation");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}