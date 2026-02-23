namespace Mamey.ISO20022.FIN.ServiceMessages;

/// <summary>
/// This message is sent by the system to notify the user that it has aborted both the General Purpose Application session belonging to the logical terminal identified in the basic header, and the open FIN session controlled by the aborted General Purpose Application.
/// </summary>
public class RemoveLogicalTerminalSystemRequest : ServiceMessage
{
    public const string Code = "14";

    public RemoveLogicalTerminalSystemRequest() : base(Code)
    {
    }
}

