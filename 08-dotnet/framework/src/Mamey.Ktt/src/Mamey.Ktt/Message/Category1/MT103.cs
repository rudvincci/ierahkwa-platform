using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category1;

/// <summary>
/// Represents an MT103 message: Single Customer Credit Transfer.
/// </summary>
public class MT103
{
    [FieldTag("20")]
    [FieldName("Transaction Reference Number")]
    [RegularExpression(@"^[A-Z0-9]{16}$", ErrorMessage = "Field 20 must be 16 alphanumeric characters.")]
    public string TransactionReference { get; set; }

    [FieldTag("23B")]
    [FieldName("Bank Operation Code")]
    [RegularExpression(@"^(CRED)$", ErrorMessage = "Field 23B must be 'CRED'.")]
    public string BankOperationCode { get; set; }

    [FieldTag("32A")]
    [FieldName("Value Date, Currency, Amount")]
    [RegularExpression(@"^\d{6}[A-Z]{3}[0-9,]{1,15}$", ErrorMessage = "Field 32A format: YYMMDDCCCAmount.")]
    public string ValueDateCurrencyAmount { get; set; }

    [FieldTag("50A")]
    [FieldName("Ordering Customer (SWIFT/BIC)")]
    public string OrderingCustomerBIC { get; set; }

    [FieldTag("50K")]
    [FieldName("Ordering Customer (Details)")]
    public string OrderingCustomerDetails { get; set; }

    [FieldTag("59")]
    [FieldName("Beneficiary Customer")]
    public string BeneficiaryCustomer { get; set; }

    [FieldTag("70")]
    [FieldName("Remittance Information")]
    [RegularExpression(@"^.{0,35}$", ErrorMessage = "Field 70 must be up to 35 characters.")]
    public string RemittanceInformation { get; set; }

    [FieldTag("71A")]
    [FieldName("Details of Charges")]
    [RegularExpression(@"^(BEN|OUR|SHA)$", ErrorMessage = "Field 71A must be 'BEN', 'OUR', or 'SHA'.")]
    public string DetailsOfCharges { get; set; }

    [FieldTag("72")]
    [FieldName("Sender to Receiver Information")]
    public string SenderToReceiverInfo { get; set; }
}