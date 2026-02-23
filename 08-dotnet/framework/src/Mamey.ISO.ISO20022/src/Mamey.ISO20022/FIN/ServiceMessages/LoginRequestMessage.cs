namespace Mamey.ISO20022.FIN.ServiceMessages;

/// <summary>
/// This message allows the system to verify whether the sending logical terminal
/// is an authorised system user.
/// This message cannot be issued by a synonym.
/// From: User
/// To: General Purpose Application
/// </summary>
public class LoginRequestMessage : ServiceMessage
{
    public static string Code = "02";
    public LoginRequestMessage()
        : base(Code)
    {
    }
    public List<IFINMessageField> FINMessageFields { get; private set; }

    public string Build()
    {
        throw new NotImplementedException();
    }
    
}
/// <summary>
/// This message initiates a FIN session for the logical terminal.
/// </summary>
public class SelectCommand : ServiceMessage
{
    public const string Code = "03";

    public SelectCommand()
        : base(Code)
    {
    }
}
/// <summary>
/// This message causes the system to terminate the current FIN session.
/// </summary>
public class QuitCommand : ServiceMessage
{
    public const string Code = "05";

    public QuitCommand() : base(Code)
    {
    }
}
/// <summary>
/// This message is issued by the user to terminate the General Purpose Application session.
/// </summary>
public class LogoutCommand : ServiceMessage
{
    public const string Code = "06";

    public LogoutCommand()
        : base(Code)
    {
    }
}
/// <summary>
/// This message is sent from Swift to a logical terminal that is logged in with the graceful shutdown indication allowed field set to "Y" at the time of the start of a maintenance activity.
/// </summary>
public class LogoutSystemRequest : ServiceMessage
{
    public const string Code = "16";

    public LogoutSystemRequest(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// 21 Acknowledgement of General Purpose Application and FIN Messages
/// </summary>
public class GeneralPurposeApplicationAndFINMessagesAknowkedgement : ServiceMessage
{
    public const string Code = "21";

    public GeneralPurposeApplicationAndFINMessagesAknowkedgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is a response to a 02 Login Request Message. It is sent by the system to acknowledge the login request.
/// </summary>
public class LoginPositiveAcknoledgement : ServiceMessage
{
    public const string Code = "22";

    public LoginPositiveAcknoledgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is a positive acknowledgement of a 03 Select Command.
/// </summary>
public class SelectRequestAcknoledgement : ServiceMessage
{
    public const string Code = "23";

    public SelectRequestAcknoledgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is sent to the user acknowledging successful completion of a 05 Quit Command.
/// </summary>
public class QuitAcknoledgement : ServiceMessage
{
    public const string Code = "25";

    public QuitAcknoledgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is sent to the user on successful completion of a 06 Logout Command.
/// </summary>
public class LogoutAcknoledgement : ServiceMessage
{
    public const string Code = "26";

    public LogoutAcknoledgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is sent to the user by the system to refuse a 02 Login Request
/// Message. The system does not create a General Purpose Application session for this logical terminal.
/// </summary>
public class LoginNegativeAcknoledgement : ServiceMessage
{
    public const string Code = "42";

    public LoginNegativeAcknoledgement(string code)
        : base(Code)
    {
    }
}
/// <summary>
/// This message is sent to the user by the system to refuse a 03 Select Command.
/// </summary>
public class SelectNegativeAcknoledgement : ServiceMessage
{
    public const string Code = "43";

    public SelectNegativeAcknoledgement(string code)
        : base(Code)
    {
    }
}

