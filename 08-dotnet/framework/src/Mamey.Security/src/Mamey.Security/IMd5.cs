namespace Mamey.Security;

public interface IMd5
{
    string Calculate(string value);
    string Calculate(Stream inputStream);
}
