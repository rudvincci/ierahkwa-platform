using System;
using System.Threading.Tasks;
using Mamey.Government.Modules.Passports.Core.Clients.DTO;

namespace Mamey.Government.Modules.Passports.Core.Clients;

/// <summary>
/// Client interface for calling the Citizens module.
/// </summary>
internal interface ICitizensClient
{
    Task<CitizenDto?> GetAsync(Guid citizenId);
    // Task<bool> ExistsAsync(Guid citizenId);
}
