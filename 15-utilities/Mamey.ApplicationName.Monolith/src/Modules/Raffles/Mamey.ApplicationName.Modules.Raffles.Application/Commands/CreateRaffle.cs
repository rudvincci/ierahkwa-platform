using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Raffles.Application.Commands;

public class CreateRaffle : ICommand
{
    public string Name { get; set; }
    public string VariantType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DrawDate { get; set; }
    public int CategoryId { get; set; }
    public VariantConfig VariantConfiguration { get; set; }

    public class VariantConfig
    {
        public int NumTiers { get; set; }
        public string FirstPrizeDescription { get; set; }
        public string SecondPrizeDescription { get; set; }
        public string ThirdPrizeDescription { get; set; }
    }
}