namespace Mamey.Mifos.Commands.Clients
{
    public class UndoRejectClient : MifosCommand
    {
        public UndoRejectClient(DateTime reopenedDate, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            ReopenedDate = reopenedDate.ToString(dateFormat);
        }
        public string ReopenedDate { get; }
    }

}

