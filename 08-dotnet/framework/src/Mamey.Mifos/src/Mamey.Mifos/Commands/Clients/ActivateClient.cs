namespace Mamey.Mifos.Commands.Clients
{
    public class ActivateClient : MifosCommand
    {
        public ActivateClient(DateTime activationTime, string dateFormat = "dd MMMM yyyy",
            string locale = "en")
            : base(dateFormat, locale)
        {
            ActivationTime = activationTime;
        }
        public DateTime ActivationTime { get; private set; }
    }
} 

