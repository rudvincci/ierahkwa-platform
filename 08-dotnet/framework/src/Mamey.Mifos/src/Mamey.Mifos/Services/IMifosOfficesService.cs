using Mamey.Mifos.Commands.Organizization;
using Mamey.Mifos.Entities;
using Mamey.Mifos.Results;
using Newtonsoft.Json;

namespace Mamey.Mifos.Services
{
    public interface IMifosOfficesService
    {
        Task<IReadOnlyList<Office>> RetrieveOfficesAsync();
        Task<Office?> RetrieveOfficeAsync(int officeId);
        Task<ResourceResponse> CreateOfficeAsync(CreateOffice command);
        Task UpdateOfficeAsync(Office office);
    }

    public class MifosOfficesService : IMifosOfficesService
    {
        private string _url;
        private readonly IMifosApiClient _mifosApiClient;

        public MifosOfficesService(MifosOptions options, IMifosApiClient mifosApiClient)
        {
            _url = $"{options.HostUrl}/api/v1/offices";
            _mifosApiClient = mifosApiClient;
        }

        public async Task<IReadOnlyList<Office>> RetrieveOfficesAsync()
        {
            //var response = await _mifosApiClient.SendAsync<CreateOffice, ResourceResponse>(new MifosRequest<CreateOffice>(_url, command));
            var exampleJsonResponse = "[{\"id\": 1,\"name\": \"Head Office\",\"nameDecorated\": \"Head Office\",\"externalId\": \"1\",\"openingDate\": [2009,1,1],\"hierarchy\": \".\"}," +
                "{\"id\": 2,\"name\": \"Mamey BDET Bank Main Branch\",\"nameDecorated\": \"Mamey BDET Bank Main Branch\",\"externalId\": \"91271f55-9476-4bd9-9f12-2ac9441aaa4d\",\"openingDate\": [2022,1,1],\"hierarchy\": \".2.\"}]";
            return await Task.FromResult(JsonConvert.DeserializeObject<IReadOnlyList<Office>>(exampleJsonResponse));
        }

        public Task<Office?> RetrieveOfficeAsync(int officeId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResourceResponse> CreateOfficeAsync(CreateOffice command)
        {
            //var response = await _mifosApiClient.SendAsync<CreateOffice, ResourceResponse>(new MifosRequest<CreateOffice>(_url, command));
            var response = await Task.FromResult(new ResourceResponse { OfficeId = 2, ResourceId = 1 });
            return response;
        }

        public Task UpdateOfficeAsync(Office office)
        {
            throw new NotImplementedException();
        }
    }
}

