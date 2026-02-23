namespace Mamey.Binimoy.Services;

public interface IICPNonFinancialService
{
    /// <summary>
    /// Invoked by Participant Core Banking System to register their respective users.
    /// </summary>
    /// <returns></returns>
    Task RegisterIDTPUserAsync();
    /// <summary>
    /// Invoked by core banking system to validate a user from BINIMOY.
    /// </summary>
    /// <returns></returns>
    Task ValidateIDTPUserAsync();
    /// <summary>
    /// Invoked by FI to get the Registered Govt Entity list that are registered in BINIMOY.
    /// </summary>
    /// <returns></returns>
    Task GetRegisteredGovtEntityListAsync();
    /// <summary>
    /// Invoked by FI to get the Registered Financial Institution list that are registered in BINIMOY.
    /// </summary>
    /// <returns></returns>
    Task GetRegisteredFIListAsync();
    /// <summary>
    /// Invoked by a Payee to generate a QR-code based on Payee’s Virtual ID and payment amount.
    /// </summary>
    /// <returns></returns>
    Task GenerateQRCodeAsync();
    /// <summary>
    /// Invoked by core banking system to get the Clock Time of BINIMOY.
    /// </summary>
    /// <returns></returns>
    Task GetIDTPClockTimeAsync();
    /// <summary>
    /// Invoked by core banking system to get Registered User List of FI.
    /// </summary>
    /// <returns></returns>
    Task GetFIUserListAsync();
    /// <summary>
    /// Invoked by BINIMOY app or White label app or FI app to register a device.
    /// </summary>
    /// <returns></returns>
    Task RegisterDeviceAsync();
    /// <summary>
    /// Invoked by app to validate an app user credentials from BINIMOY.
    /// </summary>
    /// <returns></returns>
    Task ValidateAppUserCredentialsAsync();
    /// <summary>
    /// Invoked by mobile app or core banking system to get Registration Status.
    /// </summary>
    /// <returns></returns>
    Task CheckRegistrationStatusAsync();
    /// <summary>
    /// Invoked by mobile app or core banking system to set default FI Account.
    /// </summary>
    /// <returns></returns>
    Task SetDefaultAccountAsync();
    /// <summary>
    /// Invoked by mobile app or core banking system to get default FI Accounts
    /// (FI default debit account, global credit account, global RTP receiving FI)
    /// if these are set to the calling FI.
    /// </summary>
    /// <returns></returns>
    Task GetDefaultAccountsAsync();
    /// <summary>
    /// Invoked by core banking system to get FI User Info.
    /// </summary>
    /// <returns></returns>
    Task GetFIUserInfo();
    /// <summary>
    /// Invoked by FI user to change BINIMOY PIN.
    /// </summary>
    /// <returns></returns>
    Task ChangeIDTPPINAsync();
    /// <summary>
    /// Invoked by FI to reset user BINIMOY PIN.
    /// </summary>
    /// <returns></returns>
    Task ResetIDTPPINAsync();
    /// <summary>
    /// Invoked by FI to get misc info from BINIMOY via ICP and return to SDK.
    /// </summary>
    /// <returns></returns>
    Task GetMiscInfo1Async();
    /// <summary>
    /// Invoked by FI to get misc info from BINIMOY via ICP and return to SDK.
    /// </summary>
    /// <returns></returns>
    Task GetMiscInfo2Async();
    /// <summary>
    /// Invoked by Participant Core Banking System to pre-register their respective users.
    /// </summary>
    /// <returns></returns>
    Task PreRegisterIDTPUserAsync();
    /// <summary>
    /// Invoked by Participant Core Banking System to get their pre-registered users info.
    /// </summary>
    /// <returns></returns>
    Task GetPreRegisteredUserInfoAsync();
    /// <summary>
    /// Invoked by Participant Core Banking System to get their user’s VirtualID.
    /// </summary>
    /// <returns></returns>
    Task GetVIDFromUserInfoAsync();
    /// <summary>
    /// Invoked by Participant Core Banking System to get their user’s info. This
    /// method have replaced “GetIDTPUserInfo” since Prod V1.2. “GetIDTPUserInfo”
    /// is no longer available.
    /// </summary>
    /// <returns></returns>
    Task GetIDTPUserProfileAndAccountInfoAsync();
    /// <summary>
    /// Invoked by Participant Core Banking System to get WebServer Token.
    /// </summary>
    /// <returns></returns>
    Task GetWebServerTokenAsync();

}
