using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category3;

public class MT341
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    public string RelatedReference { get; set; }

    [FieldTag("30")]
    [RegularExpression(@"^\d{6}$")]
    public string SettlementDate { get; set; }

    [FieldTag("32B")]
    [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
    public string CurrencyAmount { get; set; }

    [FieldTag("34A")]
    public string InterestRate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}