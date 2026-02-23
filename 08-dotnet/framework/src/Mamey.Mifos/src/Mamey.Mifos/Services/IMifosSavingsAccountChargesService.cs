namespace Mamey.Mifos.Services
{
    public interface IMifosSavingsAccountChargesService
    {
		Task GetChargesAsync(int accountId, int chargeId);
		Task AddChargeAsync(int accountId, int chargeId);
		Task RetrieveChargeAsync(int accountId, int chargeId);
		Task ModifyChargeAsync(int accountId, int chargeId);
		Task DeleteChargeAsync(int accountId, int chargeId);
		Task PayCharge(int accountId, int chargeId);
		Task WaiveCharge(int accountId, int chargeId);
		Task InactivateCharge(int accountId, int chargeId);
	}

	
}

