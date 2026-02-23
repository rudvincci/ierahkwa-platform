using System.Security.Cryptography;
using System.Text;

namespace Mamey.Security;

public sealed class Md5 : IMd5
{
    public string Calculate(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        using var md5Generator = MD5.Create();
        var hash = md5Generator.ComputeHash(Encoding.ASCII.GetBytes(value));
        var stringBuilder = new StringBuilder();
        foreach (var @byte in hash)
        {
            stringBuilder.Append(@byte.ToString("X2"));
        }

        return stringBuilder.ToString().ToLowerInvariant();
    }
    public string Calculate(Stream inputStream)
    {
        if (inputStream == null)
            throw new ArgumentNullException(nameof(inputStream));

        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(inputStream);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLowerInvariant();
        }
    }
}
