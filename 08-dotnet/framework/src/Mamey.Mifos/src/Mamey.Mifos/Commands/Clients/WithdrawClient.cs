namespace Mamey.Mifos.Commands.Clients
{
    public class WithdrawClient : MifosCommand
    {
        public WithdrawClient(DateTime withdrawalDate, int withdrawalReasonId, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            WithdrawalDate = withdrawalDate.ToString(dateFormat);
            WithdrawalReasonId = withdrawalReasonId;
        }
        public string WithdrawalDate { get; }
        public int WithdrawalReasonId { get; }

    }

}

