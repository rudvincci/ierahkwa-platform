using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category1;

/// <summary>
/// Represents an MT102 message: Multiple Customer Credit Transfer.
/// Used to instruct a bank to transfer funds to multiple beneficiaries in a single message.
/// </summary>
public class MT102
{
    /// <summary>
    /// Field 20: Transaction Reference Number (mandatory)
    /// A unique reference assigned by the sender to identify the transaction.
    /// Format: 16 alphanumeric characters.
    /// Example: "REF12345678901234"
    /// </summary>
    [FieldTag("20")]
    [FieldName("Transaction Reference Number")]
    [RegularExpression(@"^[A-Z0-9]{16}$")]
    public string TransactionReference { get; set; }

    /// <summary>
    /// Field 21: Related Reference (optional)
    /// Used to refer to a related transaction if applicable.
    /// Format: Up to 16 alphanumeric characters.
    /// Example: "PREVREF12345678"
    /// </summary>
    [FieldTag("21")]
    [FieldName("Related Reference")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string RelatedReference { get; set; }

    /// <summary>
    /// Field 19: Ordering Customer's Reference (optional)
    /// Used to identify the ordering customer's transaction reference.
    /// Example: "ORD1234567890"
    /// </summary>
    [FieldTag("19")]
    [FieldName("Ordering Customer's Reference")]
    [RegularExpression(@"^[A-Z0-9]{0,16}$")]
    public string OrderingCustomerReference { get; set; }

    /// <summary>
    /// Field 32A: Value Date, Currency, and Amount (mandatory)
    /// Specifies the value date, currency, and total amount for all transfers.
    /// Format: YYMMDD followed by currency and amount.
    /// Example: "241117USD10000,00"
    /// </summary>
    [FieldTag("32A")]
    [FieldName("Value Date, Currency, and Amount")]
    [RegularExpression(@"^\d{6}[A-Z]{3}[0-9,]{1,15}$")]
    public string ValueDateCurrencyAmount { get; set; }

    /// <summary>
    /// Field 50A: Ordering Customer (mandatory)
    /// Provides details of the customer initiating the transfer.
    /// Example: "John Doe, Account 0011223344"
    /// </summary>
    [FieldTag("50A")]
    [FieldName("Ordering Customer")]
    public string OrderingCustomer { get; set; }

    /// <summary>
    /// Field 59: List of Beneficiary Customers (mandatory)
    /// Specifies the account to which the funds should be credited for each beneficiary.
    /// Example: "Jane Smith, Account 5566778899"
    /// </summary>
    [FieldTag("59")]
    [FieldName("Beneficiary Customers")]
    public List<string> BeneficiaryCustomers { get; set; }

    /// <summary>
    /// Field 70: Remittance Information (optional)
    /// Free text to provide additional payment details, such as invoice numbers.
    /// Example: "Payment for invoice #1234"
    /// </summary>
    [FieldTag("70")]
    [FieldName("Remittance Information")]
    [RegularExpression(@"^.{0,35}$")]
    public string RemittanceInformation { get; set; }

    /// <summary>
    /// Field 71A: Details of Charges (mandatory)
    /// Determines who bears the transaction charges.
    /// Options:
    /// - "BEN": Charges borne by the beneficiary.
    /// - "OUR": Charges borne by the sender.
    /// - "SHA": Charges shared between sender and beneficiary.
    /// </summary>
    [FieldTag("71A")]
    [FieldName("Details of Charges")]
    [RegularExpression(@"^(BEN|OUR|SHA)$")]
    public string DetailsOfCharges { get; set; }

    /// <summary>
    /// Field 72: Sender to Receiver Information (optional)
    /// Provides additional instructions for the receiver.
    /// Example: "Please process urgently."
    /// </summary>
    [FieldTag("72")]
    [FieldName("Sender to Receiver Information")]
    public string SenderToReceiverInfo { get; set; }
}