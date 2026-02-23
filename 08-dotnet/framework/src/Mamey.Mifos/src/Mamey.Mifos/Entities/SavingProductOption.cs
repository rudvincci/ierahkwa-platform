namespace Mamey.Mifos.Entities
{
    public record SavingProductOption(int Id, string Name, bool WithdrawalFeeForTransfers,
        bool AllowOverdraft);
}

