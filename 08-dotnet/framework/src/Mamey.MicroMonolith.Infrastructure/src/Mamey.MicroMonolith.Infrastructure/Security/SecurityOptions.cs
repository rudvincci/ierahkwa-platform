namespace Mamey.MicroMonolith.Infrastructure.Security;

internal sealed class SecurityOptions
{
    public EncryptionOptions Encryption { get; set; }

    public class EncryptionOptions
    {
        public bool Enabled { get; set; }
        public string Key { get; set; }
    }
}