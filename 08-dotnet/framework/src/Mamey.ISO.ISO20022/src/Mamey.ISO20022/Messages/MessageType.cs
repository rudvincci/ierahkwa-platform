using System.ComponentModel.DataAnnotations;

namespace Mamey.ISO20022.Messages;

public enum MessageType
{
    [Display(Name = "Customer Credit Transfer Initiation", GroupName = "Payments.PaymentsInitiation", ShortName = "pain.001.001.12")]
    CustomerCreditTrasnferInitiation,
    [Display(Name = "Customer Payment Status Report", GroupName = "Payments.PaymentsInitiation", ShortName = "pain.002.001.14")]
    CustomerPaymentStatusReport,
    [Display(Name = "Customer Payment Reversal", GroupName = "Payments.PaymentsInitiation", ShortName = "pain.007.001.12")]
    CustomerPaymentReversal,
    [Display(Name = "Customer Direct Debit Initiation", GroupName = "Payments.PaymentsInitiation", ShortName = "pain.008.001.11")]
    CustomerDirectDebitInitiation,
}