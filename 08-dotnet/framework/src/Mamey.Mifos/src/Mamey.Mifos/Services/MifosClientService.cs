using Mamey.Mifos.Commands.Clients;
using Mamey.Mifos.Results;

namespace Mamey.Mifos.Services
{
    public class MifosClientService : IMifosClientService
    {
        private string _url;
        private readonly IMifosApiClient _mifosApiClient;

        public MifosClientService(MifosOptions options, IMifosApiClient mifosApiClient)
        {
            _url = $"{options.HostUrl}/api/v1/clients";
            _mifosApiClient = mifosApiClient;
        }

        public Task AcceptTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<IMifosResult<ActivateClientResponse>> ActivateAsync(ActivateClient command)
            => await _mifosApiClient.SendAsync<ActivateClient, ActivateClientResponse>(new MifosRequest<ActivateClient>(_url, command));

        public Task AddChargeAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task AssignStaffAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task CreateAccountTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task CreateAddressAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<IMifosResult<CreateClientResponse>> CreateAsync(CreateClient command)
            => await _mifosApiClient.SendAsync<CreateClient, CreateClientResponse>(new MifosRequest<CreateClient>(_url, command));

        public Task CreateIdentifierAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task CreateStandingInstructionAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteStaindingInstructionAsync(int standingInstructionId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IMifosClient>> GetClientsAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetIdentifiersAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetTransactionsAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task ModifyIdentifierAsync(int clientId, int identifierId)
        {
            throw new NotImplementedException();
        }

        public Task ModifyStandingInstructionAsync(int standingInstructionId)
        {
            throw new NotImplementedException();
        }

        public Task PayChargeAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task ProposeAndAcceptTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task ProposeTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task ReactivateAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task RefundActiveLoanByTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task RejectApplicationAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task RejectTransferAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task RetrieveAccountsOverviewAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task<IMifosClient> RetrieveAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task RetrieveIdentifierAsync(int clientId, int identifierId)
        {
            throw new NotImplementedException();
        }

        public Task RetrieveStandingInstructionAsync(int standingInstructionId)
        {
            throw new NotImplementedException();
        }

        public Task UnassignStaffAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task UndoRejectAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task UndoTransactionAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDefaultAccountAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task UploadImageAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task WaiveChargeAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawApplicationAsync(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}

