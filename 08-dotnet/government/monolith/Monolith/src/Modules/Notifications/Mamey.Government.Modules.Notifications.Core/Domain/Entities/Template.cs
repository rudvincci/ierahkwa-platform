using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Entities;

internal class Template(TemplateId id, string name, NotificationType type, string body, int version = 0) 
    : AggregateRoot<TemplateId>(id, version)
{
    public string Name { get; private set; } = name;
    public NotificationType Type { get; private set; } = type;
    public string Body { get; set; } = body;

}