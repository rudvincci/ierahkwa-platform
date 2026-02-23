using System.ComponentModel.DataAnnotations;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt.Message.Category7;

    /// <summary>
    /// Represents an MT760 message: Guarantee/Standby Letter of Credit.
    /// This message is sent by a bank to issue a guarantee or standby letter of credit.
    /// </summary>
    public class MT760
    {
        /// <summary>
        /// Field 20: Transaction Reference Number (mandatory)
        /// Format: 16 alphanumeric characters
        /// </summary>
        [FieldTag("20")]
        [FieldName("Transaction Reference Number")]
        [RegularExpression(@"^[A-Z0-9]{16}$", ErrorMessage = "Field 20 must be 16 alphanumeric characters.")]
        public string TransactionReference { get; set; }

        /// <summary>
        /// Field 23: Further Identification (optional)
        /// Format: Up to 4 alphanumeric characters
        /// </summary>
        [FieldTag("23")]
        [FieldName("Further Identification")]
        [RegularExpression(@"^[A-Z0-9]{0,4}$", ErrorMessage = "Field 23 must be up to 4 alphanumeric characters.")]
        public string FurtherIdentification { get; set; }

        /// <summary>
        /// Field 30: Date of Issue (mandatory)
        /// Format: YYMMDD
        /// </summary>
        [FieldTag("30")]
        [FieldName("Date of Issue")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Field 30 must be in YYMMDD format.")]
        public string DateOfIssue { get; set; }

        /// <summary>
        /// Field 40C: Applicable Rules (optional)
        /// Format: Code specifying the rules governing the guarantee.
        /// Possible values: URDG, ISP
        /// </summary>
        [FieldTag("40C")]
        [FieldName("Applicable Rules")]
        [RegularExpression(@"^(URDG|ISP)?$", ErrorMessage = "Field 40C must be 'URDG' or 'ISP'.")]
        public string ApplicableRules { get; set; }

        /// <summary>
        /// Field 77C: Details of Guarantee (mandatory)
        /// Free text format describing the guarantee.
        /// </summary>
        [FieldTag("77C")]
        [FieldName("Details of Guarantee")]
        public string DetailsOfGuarantee { get; set; }

        /// <summary>
        /// Field 72: Sender to Receiver Information (optional)
        /// Free text format for additional instructions.
        /// </summary>
        [FieldTag("72")]
        [FieldName("Sender to Receiver Information")]
        public string SenderToReceiverInfo { get; set; }

        /// <summary>
        /// Field 79: Narrative (optional)
        /// Free text format for any additional narrative information.
        /// </summary>
        [FieldTag("79")]
        [FieldName("Narrative")]
        public string Narrative { get; set; }
    }