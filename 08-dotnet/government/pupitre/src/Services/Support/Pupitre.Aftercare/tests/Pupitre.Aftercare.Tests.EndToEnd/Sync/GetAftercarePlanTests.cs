using System.Net;
using Pupitre.Aftercare.Api;
using Pupitre.Aftercare.Application.DTO;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using Pupitre.Aftercare.Tests.Shared.Factories;
using Pupitre.Aftercare.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Aftercare.Tests.EndToEnd.Sync;

public class GetAftercarePlanTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"aftercare/{_aftercareplanId}");

    [Fact]
    public async Task get_aftercareplan_endpoint_should_return_not_found_status_code_if_aftercareplan_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_aftercareplan_endpoint_should_return_dto_with_correct_data()
    {
        await InsertAftercarePlanAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AftercarePlanDto>(content);
        
        dto.Id.ShouldBe(_aftercareplanId);
    }
    
    #region Arrange

    private readonly Guid _aftercareplanId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<AftercarePlanDocument, Guid> _mongoDbFixture;

    private Task InsertAftercarePlanAsync()
        => _mongoDbFixture.InsertAsync(new AftercarePlanDocument
        {
            Id = _aftercareplanId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetAftercarePlanTests(MameyApplicationFactory<Program> factory)
    {
        _aftercareplanId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<AftercarePlanDocument, Guid>("aftercare");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}