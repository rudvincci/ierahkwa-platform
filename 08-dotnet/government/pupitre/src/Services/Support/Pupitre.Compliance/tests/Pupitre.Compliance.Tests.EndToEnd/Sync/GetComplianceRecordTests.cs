using System.Net;
using Pupitre.Compliance.Api;
using Pupitre.Compliance.Application.DTO;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using Pupitre.Compliance.Tests.Shared.Factories;
using Pupitre.Compliance.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Compliance.Tests.EndToEnd.Sync;

public class GetComplianceRecordTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"compliance/{_compliancerecordId}");

    [Fact]
    public async Task get_compliancerecord_endpoint_should_return_not_found_status_code_if_compliancerecord_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_compliancerecord_endpoint_should_return_dto_with_correct_data()
    {
        await InsertComplianceRecordAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<ComplianceRecordDto>(content);
        
        dto.Id.ShouldBe(_compliancerecordId);
    }
    
    #region Arrange

    private readonly Guid _compliancerecordId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<ComplianceRecordDocument, Guid> _mongoDbFixture;

    private Task InsertComplianceRecordAsync()
        => _mongoDbFixture.InsertAsync(new ComplianceRecordDocument
        {
            Id = _compliancerecordId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetComplianceRecordTests(MameyApplicationFactory<Program> factory)
    {
        _compliancerecordId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<ComplianceRecordDocument, Guid>("compliance");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}