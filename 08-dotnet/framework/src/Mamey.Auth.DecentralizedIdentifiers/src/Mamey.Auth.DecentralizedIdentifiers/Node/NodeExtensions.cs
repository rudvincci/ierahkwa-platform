using System.Diagnostics;

namespace Mamey.Auth.DecentralizedIdentifiers.Node;

public static class NodeExtensions
{
    // Call: node ion-crypto.js canonicalize payload.json
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