namespace Mamey.Mifos
{
    public interface IMifosQueryHandler<T> where T : IMifosQuery
    {
        public string Url { get; }
        public T Query { get; }
    };
}

