namespace Mamey.Security;

public interface IHasher
{
    string Hash(string data);
    byte[] Hash(byte[] data);
    byte[] HashToBytes(string data);
}
