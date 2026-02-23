namespace Mamey.Security
{
    public interface IPrivateKeyResult<T> where T : IPrivateKey
    {
        T PrivateKey { get; init; }
        string? Key { get; init; }

        void Deconstruct(out T PrivateKey, out string? Key);
        bool Equals(object? obj);
        bool Equals(PrivateKeyResult<T>? other);
        int GetHashCode();
        string ToString();
    }
}