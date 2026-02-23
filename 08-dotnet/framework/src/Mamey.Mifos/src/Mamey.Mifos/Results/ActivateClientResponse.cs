namespace Mamey.Mifos.Results
{
    public class ActivateClientResponse : IMifosResponse
    {
        public int OfficeId { get; set; }
        public int ClientId { get; set; }
        public int ResourceId { get; set; }
    }
}

