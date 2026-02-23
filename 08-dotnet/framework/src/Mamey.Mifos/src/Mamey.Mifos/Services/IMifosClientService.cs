using Mamey.Mifos.Commands.Clients;
using Mamey.Mifos.Results;

namespace Mamey.Mifos.Services
{
    public interface IMifosClientService
    {
        Task<IReadOnlyList<IMifosClient>> GetClientsAsync();
        Task<IMifosResult<CreateClientResponse>> CreateAsync(CreateClient command);
        Task UpdateAsync(int clientId);
        Task<IMifosClient> RetrieveAsync(int clientId);
        Task<IMifosResult<ActivateClientResponse>> ActivateAsync(ActivateClient command);
        Task CloseAsync(int clientId);
        Task RejectApplicationAsync(int clientId);
        Task WithdrawApplicationAsync(int clientId);
        Task ReactivateAsync(int clientId);
        Task UndoRejectAsync(int clientId);
        Task AssignStaffAsync(int clientId);
        Task UnassignStaffAsync(int clientId);
        Task ProposeTransferAsync(int clientId);
        Task AcceptTransferAsync(int clientId);
        Task RejectTransferAsync(int clientId);
        Task UpdateDefaultAccountAsync(int clientId);
        Task ProposeAndAcceptTransferAsync(int clientId);
        Task RetrieveAccountsOverviewAsync(int clientId);
        Task CreateAddressAsync(int clientId);

        Task GetIdentifiersAsync();
        Task RetrieveIdentifierAsync(int clientId, int identifierId);
        Task ModifyIdentifierAsync(int clientId, int identifierId);
        Task CreateIdentifierAsync(int clientId);
        Task UploadImageAsync(int clientId);

        Task CreateStandingInstructionAsync();
        Task RetrieveStandingInstructionAsync(int standingInstructionId);
        Task ModifyStandingInstructionAsync(int standingInstructionId);
        Task DeleteStaindingInstructionAsync(int standingInstructionId);

        Task CreateAccountTransferAsync(int clientId);
        Task RefundActiveLoanByTransferAsync(int clientId);
        Task AddChargeAsync(int clientId);
        Task PayChargeAsync(int clientId);
        Task WaiveChargeAsync(int clientId);
        Task GetTransactionsAsync(int clientId);
        Task UndoTransactionAsync(int clientId);
    }

}

