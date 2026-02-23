using Mamey.Identity.Decentralized.Abstractions;
using Nethereum.Signer;

namespace Mamey.Identity.Decentralized.Services;

public class MetaMaskService : IMetaMaskService
{
    public Task<bool> VerifyEthSignatureAsync(string challenge, string signature, string ethAddress)
    {
        var signer = new EthereumMessageSigner();
        // MetaMask uses personal_sign, which prefixes the message automatically
        var recoveredAddress = signer.EncodeUTF8AndEcRecover(challenge, signature);
        return Task.FromResult(StringEqualsIgnoreCaseEth(recoveredAddress, ethAddress));
    }

    private static bool StringEqualsIgnoreCaseEth(string a, string b)
    {
        if (a == null || b == null) return false;
        return NormalizeEthAddress(a) == NormalizeEthAddress(b);
    }

    private static string NormalizeEthAddress(string addr)
    {
        if (string.IsNullOrWhiteSpace(addr)) return "";
        addr = addr.ToLowerInvariant();
        if (!addr.StartsWith("0x")) addr = "0x" + addr;
        return addr;
    }
}