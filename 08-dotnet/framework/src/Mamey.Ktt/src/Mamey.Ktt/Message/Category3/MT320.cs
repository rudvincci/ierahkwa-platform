using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT320
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22A")]
    [RegularExpression(@"^(LOAN|DEPOSIT)$")]
    public string OperationCode { get; set; }

    [FieldTag("30T")]
    [RegularExpression(@"^\d{6}$")]
    public string TradeDate { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CurrencyPrincipalAmount { get; set; }

    [FieldTag("33E")]
    [RegularExpression(@"^\d{6}$")]
    public string MaturityDate { get; set; }

    [FieldTag("34A")]
    [RegularExpression(@"^[0-9]{1,5}\.[0-9]{1,2}$")]
    public string InterestRate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}