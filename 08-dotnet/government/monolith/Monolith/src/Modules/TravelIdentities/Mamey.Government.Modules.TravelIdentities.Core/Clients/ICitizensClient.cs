using System;
using System.Threading.Tasks;
using Mamey.Government.Modules.TravelIdentities.Core.Clients.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Clients;

internal interface ICitizensClient
{
    Task<CitizenDto?> GetAsync(Guid citizenId);
    // Task<bool> ExistsAsync(Guid citizenId);
}
