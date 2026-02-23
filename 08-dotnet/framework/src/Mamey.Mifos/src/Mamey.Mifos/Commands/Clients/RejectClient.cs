namespace Mamey.Mifos.Commands.Clients
{
    public class RejectClient : MifosCommand
    {
        public RejectClient(DateTime rejectionDate, int rejectionReasonId, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            RejectionDate = rejectionDate.ToString(dateFormat);
            RejectionReasonId = rejectionReasonId;
        }
        public string RejectionDate { get; }
        public int RejectionReasonId { get; }
    }

}

