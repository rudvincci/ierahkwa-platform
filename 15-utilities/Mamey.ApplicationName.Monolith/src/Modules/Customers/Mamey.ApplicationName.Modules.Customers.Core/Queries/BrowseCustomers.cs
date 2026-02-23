using Mamey.ApplicationName.Modules.Customers.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Customers.Core.Queries;

internal class BrowseCustomers : PagedQueryBase, IQuery<PagedResult<CustomerDto>>
{
    public string State { get; set; }
}
