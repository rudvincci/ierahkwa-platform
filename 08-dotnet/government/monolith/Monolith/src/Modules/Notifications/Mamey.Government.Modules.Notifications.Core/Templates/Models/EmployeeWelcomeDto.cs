using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Templates.Models;

public class EmployeeWelcomeDto
{
    public Name Name { get; set;  }
    public string CreatePasswordLink { get; set; }
}