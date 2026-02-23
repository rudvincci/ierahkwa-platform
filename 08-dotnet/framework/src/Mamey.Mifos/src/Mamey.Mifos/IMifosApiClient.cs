using Mamey.Mifos.Commands;

namespace Mamey.Mifos
{
    public interface IMifosApiClient
    {
        Task<IMifosResult<U>> SendAsync<T, U>(IMifosRequest<T> request)
            where T : IMifosCommand
            where U : IMifosResponse;
    }
}

