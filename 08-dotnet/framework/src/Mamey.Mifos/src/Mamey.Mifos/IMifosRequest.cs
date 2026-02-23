using Mamey.Mifos.Commands;

namespace Mamey.Mifos
{
    public interface IMifosRequest<T> where T : IMifosCommand
    {
        public string Url { get; }
        public T Command { get; }   
    };
}

