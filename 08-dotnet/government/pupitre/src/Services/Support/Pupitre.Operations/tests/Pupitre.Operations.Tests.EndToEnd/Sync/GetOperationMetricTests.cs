using System.Net;
using Pupitre.Operations.Api;
using Pupitre.Operations.Application.DTO;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using Pupitre.Operations.Tests.Shared.Factories;
using Pupitre.Operations.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Pupitre.Operations.Tests.EndToEnd.Sync;

public class GetOperationMetricTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"operations/{_operationmetricId}");

    [Fact]
    public async Task get_operationmetric_endpoint_should_return_not_found_status_code_if_operationmetric_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task get_operationmetric_endpoint_should_return_dto_with_correct_data()
    {
        await InsertOperationMetricAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<OperationMetricDto>(content);
        
        dto.Id.ShouldBe(_operationmetricId);
    }
    
    #region Arrange

    private readonly Guid _operationmetricId; 
    private readonly HttpClient _httpClient;
    private readonly MongoDbFixture<OperationMetricDocument, Guid> _mongoDbFixture;

    private Task InsertOperationMetricAsync()
        => _mongoDbFixture.InsertAsync(new OperationMetricDocument
        {
            Id = _operationmetricId,
            Name = "name",
            Tags = new[] {"tag"}
        });
    
    public GetOperationMetricTests(MameyApplicationFactory<Program> factory)
    {
        _operationmetricId = Guid.NewGuid();
        _mongoDbFixture = new MongoDbFixture<OperationMetricDocument, Guid>("operations");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        _mongoDbFixture.Dispose();
    }
    
    #endregion
}