using System;
using System.Threading.Tasks;
using Mamey.Government.Modules.TravelIdentities.Core.Clients.DTO;
using Mamey.MicroMonolith.Abstractions.Modules;

namespace Mamey.Government.Modules.TravelIdentities.Core.Clients;

internal class CitizensClient : ICitizensClient
{
    private readonly IModuleClient _moduleClient;

    public CitizensClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }

    public Task<CitizenDto?> GetAsync(Guid citizenId)
        => _moduleClient.SendAsync<CitizenDto?>("citizens/get", new { CitizenId = citizenId });
    //
    // public Task<bool> ExistsAsync(Guid citizenId)
    //     => _moduleClient.SendAsync<bool>("citizens/exists", new { CitizenId = citizenId });
}
