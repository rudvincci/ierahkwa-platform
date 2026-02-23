using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

public class Settings
{
    public Guid Id { get; set; }
    public UserId? UserId { get; private set; }
    
}