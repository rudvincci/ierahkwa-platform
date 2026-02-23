namespace Mamey.Mifos.Commands.Clients
{
    public class UndoWithdrawClient : MifosCommand
    {
        public UndoWithdrawClient(DateTime reopenedDate, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            ReopenedDate = reopenedDate.ToString(dateFormat);
        }
        public string ReopenedDate { get; }
    }

}

