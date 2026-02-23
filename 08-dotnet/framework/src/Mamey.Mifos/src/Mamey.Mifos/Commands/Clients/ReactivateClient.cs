namespace Mamey.Mifos.Commands.Clients
{
    public class ReactivateClient : MifosCommand
    {
        public ReactivateClient(DateTime reactivationDate, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            ReactivationDate = reactivationDate.ToString(dateFormat);
        }
        public string ReactivationDate { get; }
    }

}

