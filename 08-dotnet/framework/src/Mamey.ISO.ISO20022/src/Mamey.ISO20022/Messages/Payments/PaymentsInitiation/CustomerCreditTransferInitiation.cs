using System.Xml.Serialization;
using Mamey.ISO20022.Messages.Envelope;

namespace Mamey.ISO20022.Messages.Payments.PaymentsInitiation;

/// <summary>
/// Represents the root element of the CustomerCreditTransferInitiation message.
/// </summary>
[XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.12")]
public class CustomerCreditTransferInitiation
{
    /// <summary>
    /// Contains the Customer Credit Transfer Initiation information.
    /// </summary>
    [XmlElement("CstmrCdtTrfInitn")]
    public CustomerCreditTransferInitiationContent Content { get; set; }
}

/// <summary>
/// Represents the content of the Customer Credit Transfer Initiation message.
/// </summary>
public class CustomerCreditTransferInitiationContent
{
    /// <summary>
    /// Group Header information shared by all individual transactions in the message.
    /// </summary>
    [XmlElement("GrpHdr")]
    public GroupHeader GroupHeader { get; set; }

    /// <summary>
    /// Payment information containing details about the debit side of transactions.
    /// </summary>
    [XmlElement("PmtInf")]
    public List<PaymentInformation> PaymentInformation { get; set; }
}

/// <summary>
/// Represents the group header of the message.
/// </summary>
public class GroupHeader
{
    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    [XmlElement("MsgId")]
    public string MessageIdentification { get; set; }

    /// <summary>
    /// Date and time the message was created.
    /// </summary>
    [XmlElement("CreDtTm")]
    public DateTime CreationDateTime { get; set; }

    /// <summary>
    /// Number of transactions in the message.
    /// </summary>
    [XmlElement("NbOfTxs")]
    public string NumberOfTransactions { get; set; }

    /// <summary>
    /// Total of all individual amounts included in the message.
    /// </summary>
    [XmlElement("CtrlSum")]
    public decimal? ControlSum { get; set; }

    /// <summary>
    /// Party initiating the payment.
    /// </summary>
    [XmlElement("InitgPty")]
    public PartyIdentification InitiatingParty { get; set; }
}

/// <summary>
/// Represents payment information containing details about the transactions.
/// </summary>
public class PaymentInformation
{
    /// <summary>
    /// Unique identification for the payment information block.
    /// </summary>
    [XmlElement("PmtInfId")]
    public string PaymentInformationIdentification { get; set; }

    /// <summary>
    /// Specifies the means of payment.
    /// </summary>
    [XmlElement("PmtMtd")]
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Indicates whether batch booking is requested.
    /// </summary>
    [XmlElement("BtchBookg")]
    public bool? BatchBooking { get; set; }

    /// <summary>
    /// Number of individual transactions in the payment information block.
    /// </summary>
    [XmlElement("NbOfTxs")]
    public string NumberOfTransactions { get; set; }

    /// <summary>
    /// Total of all individual amounts in the block.
    /// </summary>
    [XmlElement("CtrlSum")]
    public decimal? ControlSum { get; set; }

    /// <summary>
    /// Date at which the initiating party requests the clearing agent to process the payment.
    /// </summary>
    [XmlElement("ReqdExctnDt")]
    public DateTime RequestedExecutionDate { get; set; }

    /// <summary>
    /// Party that owes an amount of money.
    /// </summary>
    [XmlElement("Dbtr")]
    public PartyIdentification Debtor { get; set; }

    /// <summary>
    /// Account of the debtor to be debited.
    /// </summary>
    [XmlElement("DbtrAcct")]
    public CashAccount DebtorAccount { get; set; }

    /// <summary>
    /// Financial institution servicing the debtor's account.
    /// </summary>
    [XmlElement("DbtrAgt")]
    public FinancialInstitutionIdentification DebtorAgent { get; set; }

    /// <summary>
    /// Credit transfer transaction information.
    /// </summary>
    [XmlElement("CdtTrfTxInf")]
    public List<CreditTransferTransactionInformation> CreditTransferTransactionInformation { get; set; }
}



/// <summary>
/// Represents details of a cash account.
/// </summary>
public class CashAccount
{
    /// <summary>
    /// Account identification.
    /// </summary>
    [XmlElement("Id")]
    public string Identification { get; set; }

    /// <summary>
    /// Name of the account.
    /// </summary>
    [XmlElement("Nm")]
    public string Name { get; set; }
}

/// <summary>
/// Represents a financial institution identification.
/// </summary>
public class FinancialInstitutionIdentification
{
    /// <summary>
    /// Business Identifier Code (BIC) of the financial institution.
    /// </summary>
    [XmlElement("BICFI")]
    public string BICFI { get; set; }
}

/// <summary>
/// Represents details of a credit transfer transaction.
/// </summary>
public class CreditTransferTransactionInformation
{
    /// <summary>
    /// Unique identification for the transaction.
    /// </summary>
    [XmlElement("PmtId")]
    public string PaymentIdentification { get; set; }

    /// <summary>
    /// Amount of money to be transferred.
    /// </summary>
    [XmlElement("Amt")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Party to which an amount of money is due.
    /// </summary>
    [XmlElement("Cdtr")]
    public PartyIdentification Creditor { get; set; }

    /// <summary>
    /// Account of the creditor to be credited.
    /// </summary>
    [XmlElement("CdtrAcct")]
    public CashAccount CreditorAccount { get; set; }
}