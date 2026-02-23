using System.Diagnostics;
using System.Text.Json;

namespace Mamey.Identity.Decentralized.Methods.Ion;

public static class IonExtensions
{
    // Call: node ion-crypto.js jwsSign payload.json privateKey.json
    public static string JwsSignWithNode(string payloadJson, string privateKeyJwkJson)
    {
        string payloadFile = Path.GetTempFileName();
        string keyFile = Path.GetTempFileName();
        File.WriteAllText(payloadFile, payloadJson);
        File.WriteAllText(keyFile, privateKeyJwkJson);

        var psi = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"ion-crypto.js jwsSign {payloadFile} {keyFile}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = Process.Start(psi);
        string output = proc.StandardOutput.ReadToEnd();
        string error = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        File.Delete(payloadFile);
        File.Delete(keyFile);

        if (proc.ExitCode != 0)
            throw new Exception($"ION JWS signing failed: {error}");

        return output.Trim();
    }

    private static string Canonicalize(string json) => CanonicalizeWithNode(json);

    private static string ComputeCommitment(IDictionary<string, object> jwk)
    {
        // 1. Serialize JWK to temp file
        string jwkJson = JsonSerializer.Serialize(jwk);
        string jwkFile = Path.GetTempFileName();
        File.WriteAllText(jwkFile, jwkJson);

        // 2. Call the Node.js script
        var psi = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"ion-crypto.js commitment {jwkFile}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var proc = Process.Start(psi);
        string output = proc.StandardOutput.ReadToEnd();
        string error = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        File.Delete(jwkFile);

        if (proc.ExitCode != 0)
            throw new Exception($"ION commitment computation failed: {error}");

        return output.Trim();
    }
    public static string CanonicalizeWithNode(string json)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, json);

        var psi = new ProcessStartInfo
        {
            FileName = "node",
            Arguments = $"ion-crypto.js canonicalize {tempFile}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var proc = Process.Start(psi);
        string output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        File.Delete(tempFile);
        return output.Trim();
    }
    
}