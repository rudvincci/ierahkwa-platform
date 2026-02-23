using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category2;

public class MT205COV
{
    /// <summary>
    /// Field 20: Transaction Reference Number (mandatory)
    /// A unique identifier assigned by the sender.
    /// Example: "COVREF1234567890"
    /// </summary>
    [FieldTag("20")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    /// <summary>
    /// Field 21: Related Reference (optional)
    /// The reference from the original MT103 message.
    /// Example: "MT103REF9876543210"
    /// </summary>
    [FieldTag("21")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string RelatedReference { get; set; }

    /// <summary>
    /// Field 32A: Value Date, Currency, and Amount (mandatory)
    /// Specifies the value date, currency, and amount.
    /// Example: "241117USD1000000,00"
    /// </summary>
    [FieldTag("32A")]
    [RegularExpression(@"^\d{6}[A-Z]{3}[0-9,]{1,15}$")]
    public string ValueDateCurrencyAmount { get; set; }

    /// <summary>
    /// Field 53A: Sender's Correspondent (optional)
    /// The correspondent bank managing the funds.
    /// Example: "ABC Bank, New York"
    /// </summary>
    [FieldTag("53A")]
    public string SendersCorrespondent { get; set; }

    /// <summary>
    /// Field 56A: Intermediary Institution (mandatory)
    /// The intermediary bank involved in the transaction.
    /// Example: "DEF Bank, London"
    /// </summary>
    [FieldTag("56A")]
    public string IntermediaryInstitution { get; set; }

    /// <summary>
    /// Field 57A: Account With Institution (mandatory)
    /// The bank where the beneficiary holds an account.
    /// Example: "GHI Bank, Tokyo"
    /// </summary>
    [FieldTag("57A")]
    public string AccountWithInstitution { get; set; }

    /// <summary>
    /// Field 72: Sender to Receiver Information (optional)
    /// Additional instructions for the receiver.
    /// Example: "Payment for invoice #7890"
    /// </summary>
    [FieldTag("72")]
    public string SenderToReceiverInfo { get; set; }
}