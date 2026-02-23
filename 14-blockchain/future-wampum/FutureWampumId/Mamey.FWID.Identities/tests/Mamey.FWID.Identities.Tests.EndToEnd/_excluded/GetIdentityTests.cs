using System.Net;
using Mamey.FWID.Identities.Api;
using Mamey.FWID.Identities.Application.DTO;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Sync;

[Collection("EndToEnd")]
public class GetIdentityTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
{
    private Task<HttpResponseMessage> Act() => _httpClient.GetAsync($"/api/identities/{_identityId}");

    [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
    public async Task get_identity_endpoint_should_return_not_found_status_code_if_identity_document_does_not_exist()
    {
        var response = await Act();
        
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
    public async Task get_identity_endpoint_should_return_dto_with_correct_data()
    {
        await InsertIdentityAsync();
        var response = await Act();
        
        response.ShouldNotBeNull();
        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<IdentityDto>(content);
        
        dto.Id.ShouldBe(_identityId);
    }
    
    #region Arrange

    private readonly Guid _identityId; 
    private readonly HttpClient _httpClient;
    // TODO: Fix MongoDbFixture usage - using MongoDBFixture instead
    // private readonly MongoDbFixture<IdentityDocument, Guid> _mongoDbFixture;

    private Task InsertIdentityAsync()
    {
        // TODO: Fix MongoDbFixture usage
        return Task.CompletedTask;
        // => _mongoDbFixture.InsertAsync(new IdentityDocument
        // {
        //     Id = _identityId,
        //     Name = "name",
        //     Tags = new[] {"tag"}
        // });
    }
    
    public GetIdentityTests(MameyApplicationFactory<Program> factory)
    {
        _identityId = Guid.NewGuid();
        // _mongoDbFixture = new MongoDbFixture<IdentityDocument, Guid>("mamey.fwid.identities");
        factory.Server.AllowSynchronousIO = true;
        _httpClient = factory.CreateClient();
    }
    
    public void Dispose()
    {
        // _mongoDbFixture.Dispose();
    }
    
    #endregion
}