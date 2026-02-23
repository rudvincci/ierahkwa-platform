namespace Mamey.ISO20022;

public class MTMessageHeader
{
    public MTMessageHeader(string messageId, DateTime creationDate, string sender, string receiver)
    {
        MessageId = messageId;
        CreationDate = creationDate;
        Sender = sender;
        Receiver = receiver;
    }

    public string MessageId { get; set; }
    public DateTime CreationDate { get; set; }
    public string Sender { get; set; }
    public string Receiver { get; set; }

}