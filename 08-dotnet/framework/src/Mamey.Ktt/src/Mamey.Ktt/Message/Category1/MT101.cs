using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category1;

/// <summary>
    /// Represents an MT101 message: Request for Transfer.
    /// Used to instruct a bank to transfer funds from the account of the ordering customer to a beneficiary.
    /// </summary>
    public class MT101
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
        /// Field 32B: Currency and Amount (mandatory)
        /// Specifies the currency and the amount to be transferred.
        /// Format: 3-letter currency code followed by the amount.
        /// Example: "USD1000,00"
        /// </summary>
        [FieldTag("32B")]
        [FieldName("Currency and Amount")]
        [RegularExpression(@"^[A-Z]{3}[0-9,]{1,15}$")]
        public string CurrencyAndAmount { get; set; }

        /// <summary>
        /// Field 50A: Ordering Customer (mandatory)
        /// Provides details of the customer initiating the transfer.
        /// Example: "John Doe, Account 0011223344"
        /// </summary>
        [FieldTag("50A")]
        [FieldName("Ordering Customer")]
        public string OrderingCustomer { get; set; }

        /// <summary>
        /// Field 59: Beneficiary Customer (mandatory)
        /// Specifies the account to which the funds should be credited.
        /// Example: "Jane Smith, Account 5566778899"
        /// </summary>
        [FieldTag("59")]
        [FieldName("Beneficiary Customer")]
        public string BeneficiaryCustomer { get; set; }

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