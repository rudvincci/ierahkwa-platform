using Mamey.CQRS.Commands;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Commands;

public class CreateRole : ICommand
{
    public RoleId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
}
public class UpdateRole : ICommand
{
    public RoleId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
}
public class DeleteRole : ICommand
{
    public RoleId Id { get; set; }
    
}
