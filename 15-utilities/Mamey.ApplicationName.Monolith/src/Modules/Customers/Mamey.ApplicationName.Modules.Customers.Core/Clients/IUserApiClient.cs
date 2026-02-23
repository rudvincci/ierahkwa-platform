using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Clients.DTO;

namespace Mamey.ApplicationName.Modules.Customers.Core.Clients;

internal interface IUserApiClient
{
    Task<UserDto> GetAsync(string email);
}