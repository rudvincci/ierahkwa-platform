using Mamey.Emails.MailKit;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Templates.Models;

public class AccountDeactivationModel(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Account Deactivation", companyName, companyAddress, recipientName, supportUrl )
{

}
public class AccountClosureConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Account Closure Confirmation", companyName, companyAddress, recipientName, supportUrl )
{

}
public class AccountLockedDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Account Locked Confirmation", companyName, companyAddress, recipientName, supportUrl )
{

}
public class AccountReactivationConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Account Reactivation Confirmation", companyName, companyAddress, recipientName, supportUrl )
{

}
public class AccountStatementAvailableDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Account Statement Available", companyName, companyAddress, recipientName, supportUrl )
{
    public string StatementPeriod { get; set; } = string.Empty;
    public string StatementUrl { get; set; } = string.Empty;
}
public class AddressChangeConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Address Change Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Address NewAddress { get; set; }
    public DateTime DateUpdated { get; set; }
}
public class BeneficiaryAddedDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Beneficiary Added", companyName, companyAddress, recipientName, supportUrl )
{
    public Name BeneficiaryName { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
}
public class CardActivationConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Card Activation Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public string CardLastFour { get; set; } = string.Empty;
    
}
public class CardDeclinedDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Card Declined Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public string CardLastFour { get; set; } = string.Empty;
    public string Merchant { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
}
public class CreditLimitIncreaseDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Credit Limit Increase Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount NewLimit { get; set; } = Amount.Zero;
    public Amount PreviousLimit { get; set; } = Amount.Zero;
    public DateTime EffectiveDate { get; set; }
}
public class EmailAddressChangeConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Email Address Change Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public string NewEmail { get; set; } = string.Empty;
}
public class LoanApprovalDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Loan Approval Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount LoanAmount { get; set; } = Amount.Zero;
    public decimal InterestRate { get; set; }
    public string RepaymentTerm { get; set; } = string.Empty;
    public Amount MonthlyPayment { get; set; } = Amount.Zero;
}
public class LoanPaymentReminderDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Loan Payment Reminder", companyName, companyAddress, recipientName, supportUrl )
{
    public DateTime DueDate { get; set; }
    public Amount PaymentAmount { get; set; } = Amount.Zero;
    public string LoanId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
}
public class OverdraftAlertDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Overdraft Alert", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount OverdraftAmount { get; set; } = Amount.Zero;
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string DepositUrl { get; set; } = string.Empty;
}
public class PasswordChangeConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Password Change Confirmation", companyName, companyAddress, recipientName, supportUrl )
{

}
public class PaymentConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Payment Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount Amount { get; set; } = Amount.Zero;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}
public class PaymentFailureDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Payment Failure", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount Amount { get; set; } = Amount.Zero;
    public string Reason { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}
public class PaymentReceivedConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Payment Received Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount PaymentAmount { get; set; } = Amount.Zero;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
}
public class RecurringPaymentConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Recurring Payment Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount PaymentAmount { get; set; } = Amount.Zero;
    public DateTime NextPaymentDate { get; set; }
    public string PlanName { get; set; } = string.Empty;
}
public class ScheduledMaintenanceDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Scheduled Maintenance", companyName, companyAddress, recipientName, supportUrl )
{
    public DateTime MaintenanceDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
public class SecurityAlertDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Security Alert", companyName, companyAddress, recipientName, supportUrl )
{
    public string Device { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
public class ServiceSuspensionDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Service Suspension", companyName, companyAddress, recipientName, supportUrl )
{
    public string Reason { get; set; }
    public Amount Balance { get; set; } = Amount.Zero;
}
public class SubscriptionRenewalConfirmationDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Subscription Renewal Confirmation", companyName, companyAddress, recipientName, supportUrl )
{
    public string PlanName { get; set; } = string.Empty;
    public Amount AmountCharged { get; set; } = Amount.Zero;
    public DateTime RenewalDate { get; set; }
    public DateTime NextBillingDate { get; set; }
}
public class SuccessfulDepositDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Successful Deposit", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount Amount { get; set; } = Amount.Zero;
    public string AccountNumber { get; set; } = string.Empty;
    private string TransactionId { get; set; } = string.Empty;
    private DateTime Date { get; set; }
}
public class TransactionAlertDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Transaction Alert", companyName, companyAddress, recipientName, supportUrl )
{
    public Amount Amount { get; set; } = Amount.Zero;
    public string TransactionType { get; set; } = string.Empty;
    public DateTime Time { get; set;  }
}
public class TwoFactorEnabledDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Two Factor Enabled", companyName, companyAddress, recipientName, supportUrl )
{

}
public class UnusualActivityDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Unusual Activity", companyName, companyAddress, recipientName, supportUrl )
{
    public string Location { get; set; }
    public string Activity { get; set; }
    public DateTime Time { get; set; }
}
internal class WelcomeBackDto(string companyName, Address companyAddress, Name recipientName, string supportUrl) 
    : BusinessEmailTemplate("Welcome Back", companyName, companyAddress, recipientName, supportUrl )
{

}