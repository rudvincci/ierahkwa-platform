namespace Mamey.Mifos.Commands.Clients
{
    public class CloseClient : MifosCommand
    {
        public CloseClient(int closureReasonId, DateTime closureDate, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            ClosureReasonId = closureReasonId;
            ClosureDate = closureDate.ToString(dateFormat);
        }
        public int ClosureReasonId { get; }
        public string ClosureDate { get; }
    }

}

