namespace Mamey.ISO20022.FIN;

public interface IFINMessage
{
    
    IEnumerable<IFINMessageField> MessageFields { get; set; }
}

public interface IFINMessageField
{
    public string Reps { get; set; }
    public string Tag { get; set; }
    public string ContentComments { get; set; }
}
/// <summary>
///  (MT category 0) which relate to either the sending or receiving of messages
///  used to customise a user's FIN operating environment.
/// </summary>
public abstract class SystemMessage
{
    public SystemMessage(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        }
        Code = code;
        MaxOutputLength = 2000;

    }
    protected virtual int MaxOutputLength { get; }
    public string Code { get; }

}
/// <summary>
///  Enable users to perform financial transactions.
/// </summary>
public class UserMessage
{

}
/// <summary>
/// Service messages exist for the exchange of operational instructions between
/// the FIN interface operator and Swift, in order to mutually manage the General
/// Purpose Application and FIN sessions and related message exchange.
/// 
/// Relate either to system commands (for example, LOGIN) or to acknowledgements
/// (for example, positive acknowledgement, select negative acknowledgement,
/// positive user acknowledgement).
/// 
/// </summary>
public class ServiceMessage
{
    public ServiceMessage(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        }
        if (code.Count() != 2)
        {
            //throw new InvalidCodeFormat("message code is invalid. it must contain exactly 2 characters")
        }

    }
}