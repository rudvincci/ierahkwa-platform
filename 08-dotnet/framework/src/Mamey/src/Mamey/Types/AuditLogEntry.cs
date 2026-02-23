namespace Mamey.Types;

public class AuditLogEntry
{
    public AuditLogEntry (DateTime timeStamp, string action, string description)
    {
        TimeStamp = timeStamp;
        Action = action;
        Description = description;
    }
    public DateTime TimeStamp { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
}
