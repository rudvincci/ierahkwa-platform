namespace Mamey.Mifos.Commands
{
    public abstract class MifosCommand : IMifosCommand
    {
        public MifosCommand(string dateFormat = "dd MMMM yyyy",
            string locale = "en")
        {
            DateFormat = dateFormat;
            Locale = locale;
        }
        public string DateFormat { get; }
        public string Locale { get; }
    }

}

