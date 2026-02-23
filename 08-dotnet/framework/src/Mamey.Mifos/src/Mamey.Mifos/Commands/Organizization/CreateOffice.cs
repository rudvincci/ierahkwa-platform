namespace Mamey.Mifos.Commands.Organizization
{
    public class CreateOffice : IMifosCommand
    {
        public CreateOffice(string name, int? parentId, DateTime openingDate, string externalId, string dateFormat = "dd MMMM yyyy", string locale = "en")
        {
            Name = name;
            ParentId = parentId;
            OpeningDate = openingDate.ToString(dateFormat);
            ExternalId = externalId;
            DateFormat = dateFormat;
            Locale = locale;
        }

        public string Name { get; private set; }
        public int? ParentId { get; private set; }
        public string OpeningDate { get; private set; }
        public string ExternalId { get; private set; }
        public string DateFormat { get; private set; }
        public string Locale { get; private set; }
    }
}

