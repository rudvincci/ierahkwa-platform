namespace Mamey.Mifos
{
    public interface IMifosResult<T> where T : IMifosResponse
    {
        public object? Error { get; }
        public bool Successful { get; }
        public T Response { get; }
    }
}

