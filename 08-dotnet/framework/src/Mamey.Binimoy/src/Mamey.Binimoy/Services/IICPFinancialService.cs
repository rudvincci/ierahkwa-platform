using Mamey.Binimoy.Requests;

namespace Mamey.Binimoy.Services;

public interface IICPFinancialService
{
    /// <summary>
    /// Invoked by Participant Core Banking System to obtain authorization for a transaction.
    /// </summary>
    /// <returns></returns>
    Task GetPaymentAuthorizationAsync(GetPaymentAuthorizationRequest request);
    /// <summary>
    /// Invoked by Participant Core Banking System to report completion of a transaction.
    /// </summary>
    /// <returns></returns>
    Task RecordPaymentAsync();
    /// <summary>
    /// Pay Govt dues such as utility bills, fees, taxes etc.
    /// </summary>
    /// <returns></returns>
    Task PayGovernmentDuesAsync();
    /// <summary>
    /// Pay Govt dues such as utility bills, fees, taxes etc using ISO 20022 message format.
    /// </summary>
    /// <returns></returns>
    Task PayGovernmentDueISOAsync();
    /// <summary>
    /// API to initiate transfer funds request from participant core banking system via Direct Credit Push with non-ISO message.
    /// </summary>
    /// <returns></returns>
    Task TransferFundsAsync();
    /// <summary>
    /// API to initiate transfer funds request from participant core banking system via Direct Credit Push in ISO 20022 message format.
    /// </summary>
    /// <returns></returns>
    Task TransferFundsISOAsync();

    /// <summary>
    /// Invoked by core banking system to transfer fund to another FI.
    /// </summary>
    /// <returns></returns>
    Task TransferFundsFItoFIAsync();
    /// <summary>
    /// Invoked by core banking system to transfer fund to another FI using ISO message format.
    /// </summary>
    /// <returns></returns>
    Task TransferFundsFItoFIISOAsync();
    /// <summary>
    /// Invoked by Payee to request payment from Payer.
    /// </summary>
    /// <returns></returns>
    Task CreateRTPAsync(CreateRTPRequest request);
    /// <summary>
    /// Invoked by Payee to request payment from Payer.
    /// </summary>
    /// <returns></returns>
    Task CreateRTPISOAsync();
    /// <summary>
    /// Invoked by Payer in response to the RTP request received from Payee.
    /// </summary>
    /// <returns></returns>
    Task SendRTPDeclinedResponseAsync();
    /// <summary>
    /// Invoked by Payer in response to the RTP request received from Payee.
    /// </summary>
    /// <returns></returns>
    Task SendRTPDeclinedResponseISOAsync();
    /// <summary>
    /// Invoked by a FI to distribute Govt funds to specified recipients.
    /// </summary>
    /// <returns></returns>
    Task DisburseGovtFundsAsync();
    /// <summary>
    /// Invoked by a FI to distribute salary payment to specified recipients.
    /// </summary>
    /// <returns></returns>
    Task DisburseSalaryAsync();
    /// <summary>
    /// Pay utility bills.
    /// </summary>
    /// <returns></returns>
    Task PayUtilityBillsAsync();
    /// <summary>
    /// Pay utility bills.
    /// </summary>
    /// <returns></returns>
    Task PayUtilityBillsISOAsync();
    /// <summary>
    /// Invoked by a FI to get Interoperable Fee, Platform Fee, Platform VAT based on the instruction type, transaction type and transaction amount.
    /// </summary>
    /// <returns></returns>
    Task GetIDTPFeeAsync();
    /// <summary>
    /// Invoked by FI app to get the transactions of current date.
    /// </summary>
    /// <returns></returns>
    Task GetTransactionsbyFIAsync();
    /// <summary>
    /// Invoked by FI Core Banking System to get the transaction history for a particular user.
    /// </summary>
    /// <returns></returns>
    Task GetTransactionsbyUserAsync();
    /// <summary>
    /// Invoked by FI to get RTP sent list for a particular user.
    /// </summary>
    /// <returns></returns>
    Task GetRTPListSentAsync();
    /// <summary>
    /// Invoked by FI to get the RTP received list for a particular user.
    /// </summary>
    /// <returns></returns>
    Task GetRTPListReceivedAsync();
    /// <summary>
    /// Invoked by core banking system to get the status of the transaction.
    /// </summary>
    /// <returns></returns>
    Task GetTransactionStatusAsync();
    /// <summary>
    /// Invoked by core banking system to get the status of the RTP.
    /// </summary>
    /// <returns></returns>
    Task GetRTPStatusAsync();
    /// <summary>
    /// Invoked by core banking system to get the list of RTP.
    /// </summary>
    /// <returns></returns>
    Task GetRTPsbyFIAsync();
    /// <summary>
    /// Invoked by core banking system to update the Disbursement status.
    /// </summary>
    /// <returns></returns>
    Task UpdateDisbursementStatusAsync();
}
