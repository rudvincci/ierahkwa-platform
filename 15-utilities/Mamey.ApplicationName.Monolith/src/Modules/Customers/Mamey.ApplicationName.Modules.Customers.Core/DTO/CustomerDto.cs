using System;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Customers.Core.DTO;
public class CustomerDto
{
    public Guid CustomerId { get; set; }
    public string State { get; set; }
    public string Email { get; set; }
    public Name Name { get; set; }
    public string FullName { get; set; }
    public string Nationality { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
}
