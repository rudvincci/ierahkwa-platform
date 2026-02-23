using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category5;

public class MT500
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    [RegularExpression(@"^(REGISTER)$")]
    public string InstructionIndicator { get; set; }

    [FieldTag("94A")]
    public string PlaceOfRegistration { get; set; }

    [FieldTag("98A")]
    [RegularExpression(@"^\d{6}$")]
    public string EffectiveDate { get; set; }

    [FieldTag("36")]
    [RegularExpression(@"^[0-9]{1,15}$")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT502
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    [RegularExpression(@"^(BUY|SELL)$")]
    public string BuySellIndicator { get; set; }

    [FieldTag("35B")]
    public string ISINCode { get; set; }

    [FieldTag("36")]
    [RegularExpression(@"^[0-9]{1,15}$")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("90A")]
    [RegularExpression(@"^[0-9]{1,10}\.[0-9]{1,2}$")]
    public string Price { get; set; }

    [FieldTag("98A")]
    [RegularExpression(@"^\d{6}$")]
    public string TradeDate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT503
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string ClaimType { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    [RegularExpression(@"^[0-9,]{1,15}$")]
    public string ClaimQuantity { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT504
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string ProposalType { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    [RegularExpression(@"^[0-9,]{1,15}$")]
    public string ProposedQuantity { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT505
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string SubstitutionType { get; set; }

    [FieldTag("35B")]
    public string OriginalSecurity { get; set; }

    [FieldTag("36")]
    public string NewSecurityQuantity { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT506
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string StatementType { get; set; }

    [FieldTag("32B")]
    public string CurrencyAmount { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT507
{
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    public string RelatedReference { get; set; }

    [FieldTag("22F")]
    public string ResponseCode { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT508
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("97A")]
    public string SafekeepingAccount { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT509
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("21")]
    public string RelatedReference { get; set; }

    [FieldTag("23G")]
    public string StatusCode { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT510
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("97A")]
    public string SafekeepingAccount { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT515
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string TradeTypeIndicator { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("32B")]
    public string CurrencyAmount { get; set; }

    [FieldTag("98A")]
    public string SettlementDate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT516
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("32B")]
    public string CurrencyAmount { get; set; }

    [FieldTag("98A")]
    public string SettlementDate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}
public class MT517
{
    [FieldTag("20")]
    public string TransactionReference { get; set; }

    [FieldTag("22F")]
    public string TradeTypeIndicator { get; set; }

    [FieldTag("35B")]
    public string IdentificationOfSecurity { get; set; }

    [FieldTag("36")]
    public string QuantityOfSecurities { get; set; }

    [FieldTag("32B")]
    public string CurrencyAmount { get; set; }

    [FieldTag("98A")]
    public string SettlementDate { get; set; }

    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}

