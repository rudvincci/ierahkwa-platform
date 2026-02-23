using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Customers.Core.DTO;

public class CustomerDetailsDto : CustomerDto
{
    public Address Address { get; set; }
    public IdentityDto Identity { get; set; }
    public string Notes { get; set; }
}
