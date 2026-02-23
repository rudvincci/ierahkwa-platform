using Mamey.Mifos.Commands;

namespace Mamey.Mifos
{
    public class MifosRequest<T> : IMifosRequest<T>
        where T : IMifosCommand
    {
        public MifosRequest(string url, T command)
        {
            Url = url;
            Command = command;
        }
        public string Url { get; private set; }
        public T Command { get; private set; }
    }

    public record MifosResult<T>(bool Successful, T? Response, object? Error) : IMifosResult<T>
        where T : IMifosResponse;
}

