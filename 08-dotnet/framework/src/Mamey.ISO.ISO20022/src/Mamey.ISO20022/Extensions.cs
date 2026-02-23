namespace Mamey.ISO20022;

public static class Extensions
{
    
}
public interface IMTMessageBuilder
{
    IMTMessageBuilder AddHeader(string messageId, DateTime creationDate, string sender, string receiver);
    IMTMessageBuilder AddHeader(MTMessageHeader messageHeader);
    IMTMessageBuilder AddTransaction(string transactionId, decimal amount, string currency, string debtor, string creditor);
    IMTMessageBuilder AddTransaction(MTTransaction transaction);
    IMTMessageBuilder AddAdditionalInfo(string info);
    string Build();
}
public class MTMTMessageBuilder
{

}
public class MTTransaction
{
    public MTTransaction(string transactionId, decimal amount, string currency, string debtor, string creditor)
    {
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        Debtor = debtor;
        Creditor = creditor;
    }

    public string TransactionId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string Debtor { get; }
    public string Creditor { get; }
}

