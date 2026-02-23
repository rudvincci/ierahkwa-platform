namespace Mamey.Mifos.Results
{
    public class CreateClientResponse : ResourceResponse, IMifosResponse
    {
        public int ClientId { get; set; }
        public int SavingsId { get; set; }
    }
}

