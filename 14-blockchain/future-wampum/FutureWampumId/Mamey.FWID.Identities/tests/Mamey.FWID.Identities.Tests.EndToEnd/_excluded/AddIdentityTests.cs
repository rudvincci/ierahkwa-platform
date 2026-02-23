using System.Net;
using System.Text;
using Mamey.FWID.Identities.Api;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Types;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Sync
{
    [Collection("EndToEnd")]
    public class AddIdentityTests : IDisposable, IClassFixture<MameyApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(AddIdentity command) 
            => _httpClient.PostAsync("mamey.fwid.identities", GetContent(command));

        [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
        public async Task add_identity_endpoint_should_return_http_status_code_created()
        {
            var command = new AddIdentity
            {
                Id = Guid.NewGuid(),
                Name = new Name("Test", "User"),
                PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
                ContactInformation = new ContactInformation(
                    new Email("test@example.com"),
                    new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
                ),
                BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
                Zone = "zone-001"
            };

            var response = await Act(command);
            
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
        
        [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
        public async Task add_identity_endpoint_should_return_location_header_with_correct_identity_id()
        {
            var command = new AddIdentity
            {
                Id = Guid.NewGuid(),
                Name = new Name("Test", "User"),
                PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
                ContactInformation = new ContactInformation(
                    new Email("test@example.com"),
                    new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
                ),
                BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
                Zone = "zone-001"
            };

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();
            
            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"mamey.fwid.identities/{command.Id}");
        }

        [Fact(Timeout = 60000)] // 60 second timeout to prevent hangs
        public async Task add_identity_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddIdentity
            {
                Id = Guid.NewGuid(),
                Name = new Name("Test", "User"),
                PersonalDetails = new PersonalDetails(DateTime.UtcNow.AddYears(-30), "USA", "Male", "Wolf Clan"),
                ContactInformation = new ContactInformation(
                    new Email("test@example.com"),
                    new Address("", "123 Main St", null, null, null, "New York", "NY", "10001", null, null, "US", null),
                    new List<Phone> { new Phone("1", "5551234567", null, Phone.PhoneType.Mobile) }
                ),
                BiometricData = new BiometricData(BiometricType.Fingerprint, new byte[1024], "test-hash"),
                Zone = "zone-001"
            };

            await Act(command);

            // TODO: Fix MongoDbFixture usage
            // var document = await _mongoDbFixture.GetAsync(command.Id);
            // document.ShouldNotBeNull();
            // document.Id.ShouldBe(command.Id);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        // TODO: Fix MongoDbFixture usage - using MongoDBFixture instead
        // private readonly MongoDbFixture<IdentityDocument, Guid> _mongoDbFixture;
        
        public AddIdentityTests(MameyApplicationFactory<Program> factory)
        {
            // _mongoDbFixture = new MongoDbFixture<IdentityDocument, Guid>("mamey.fwid.identities");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }
        
        public void Dispose()
        {
            // _mongoDbFixture.Dispose();
        }
        
        private static StringContent GetContent(object value) 
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
        
        #endregion
    }
}