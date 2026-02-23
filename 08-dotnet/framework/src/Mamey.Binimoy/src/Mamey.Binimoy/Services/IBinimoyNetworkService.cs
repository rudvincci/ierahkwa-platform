namespace Mamey.Binimoy.Services;

public interface IBinimoyNetworkService
{
    /// <summary>
    /// Invoked by BINIMOY App to get the account balance of users
    /// </summary>
    /// <returns></returns>
    Task GetAccountBalanceAsync();
    /// <summary>
    /// Invoked by FI App on Payer (Sending) FI to initiate a Direct Pay transaction
    /// </summary>
    /// <returns></returns>
    Task InitiateFundTransferAsync();
    /// <summary>
    /// Invoked by BINIMOY on Payer (Sending) FI to initiate a Direct Pay transaction
    /// </summary>
    /// <returns></returns>
    Task InitiateFundTransferISOAsync();
    /// <summary>
    /// Invoked by BINIMOY on Receiving FI to accept a fund transfer
    /// </summary>
    /// <returns></returns>
    Task ProcessFundTransferRequestAsync();
    /// <summary>
    /// Invoked in BINIMOY on Payer (Sending) FI to transmit a RTP message to Payer
    /// </summary>
    /// <returns></returns>
    Task ProcessRTPRequestAsync();
    /// <summary>
    /// Invoked by BINIMOY on Payee (Receiving) FI to transmit RTP Declined message to Payee
    /// </summary>
    /// <returns></returns>
    Task ProcessRTPDeclinedResponseAsync();
    /// <summary>
    /// Invoked by BINIMOY on Payer (Sending) FI to debit funds for bulk payments (such as salary payments or fund disbursements) initiated from the BINIMOY portal.
    /// </summary>
    /// <returns></returns>
    Task ProcessIDTPBulkPaymentRequestAsync();
    /// <summary>
    /// Invoked by BINIMOY to validate a FI user from FI.
    /// </summary>
    /// <returns></returns>
    Task ValidateUserAsync();
    /// <summary>
    /// Invoked by BINIMOY components to request FI to initiate an OTP to the user.
    /// </summary>
    /// <returns></returns>
    Task SendOTPAsync();
    /// <summary>
    /// Invoked by BINIMOY components to request FI to validate an OTP that is sent by FI to the user.
    /// </summary>
    /// <returns></returns>
    Task ValidateOTPAsync();
    /// <summary>
    /// Invoked by BINIMOY on Receiving FI to accept a FI to FI fund transfer
    /// </summary>
    /// <returns></returns>
    Task ProcessFIToFIFundTransferRequestAsync();
    /// <summary>
    /// Invoked by BINIMOY App Server to notify white label app events to FI.
    /// </summary>
    /// <returns></returns>
    Task NotifyWhiteLabelEventsAsync();

    // TODO: clarify duplicate method, is there a difference?
    /// <summary>
    /// DUPLICATE : 
    /// </summary>
    /// <returns></returns>
    Task ProcessFundTransferRequestAsync1();

}