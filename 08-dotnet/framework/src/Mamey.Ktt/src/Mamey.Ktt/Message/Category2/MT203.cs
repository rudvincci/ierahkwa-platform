using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category2;

public class MT203
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("32A")]
    [RegularExpression(@"^\d{6}[A-Z]{3}[0-9,]{1,15}$")]
    public string ValueDateCurrencyAmount { get; set; }

    [FieldTag("52A")]
    public string OrderingInstitution { get; set; }

    [FieldTag("53A")]
    public string SendersCorrespondent { get; set; }

    [FieldTag("58A")]
    public string BeneficiaryInstitution { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}