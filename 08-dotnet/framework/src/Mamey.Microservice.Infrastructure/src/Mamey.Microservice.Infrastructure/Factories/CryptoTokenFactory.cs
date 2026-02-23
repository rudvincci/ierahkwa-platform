using System.Security.Cryptography;

namespace Mamey.Microservice.Infrastructure.Factories
{
    internal static class CryptoTokenFactory
    {
        public static string Create()
        {
            var rngCryptoServiceProvider = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[256];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}